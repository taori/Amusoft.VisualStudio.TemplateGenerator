using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Generator.Shared.Resources;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;

namespace Generator.Shared.Transformation
{
	public class SolutionExplorer
	{
		static SolutionExplorer()
		{
			var additionalCopyExtensions = ConfigurationManager.AppSettings[Constants.Configuration.AdditionalFileCopyExtensions] ?? string.Empty;
			AdditionalCopyExtensions = new HashSet<string>(additionalCopyExtensions.Split(','));
		}

		public static HashSet<string> AdditionalCopyExtensions { get; set; }

		private static readonly ILogger Log = LogManager.GetLogger(nameof(SolutionExplorer));

		public static async Task<SolutionExplorer> CreateAsync(string solutionPath, IProgress<string> progress, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(solutionPath)) throw new ArgumentNullException(nameof(solutionPath));
			if (!File.Exists(solutionPath)) throw new FileNotFoundException(solutionPath);

			Log.Debug($"Creating {nameof(SolutionExplorer)} for {solutionPath}.");
			var crawler = new SolutionExplorer(solutionPath);
			await crawler.ExecuteAsync(progress, cancellationToken);
			return crawler;
		}

		public Solution Solution { get; private set; }

		public string SolutionPath { get; }

		public SolutionExplorer(string solutionPath)
		{
			SolutionPath = solutionPath;
		}

		public static IEnumerable<string> GetReferencedProjectPaths(string solutionPath)
		{
			if (string.IsNullOrEmpty(solutionPath))
				throw new ArgumentNullException(nameof(solutionPath), $"{nameof(solutionPath)}");
			if (!File.Exists(solutionPath))
				throw new FileNotFoundException(solutionPath);

			var solutionFile = SolutionFile.Parse(solutionPath);

			return solutionFile
				.ProjectsByGuid
				.Values
				.Where(d => d.ProjectType != SolutionProjectType.SolutionFolder)
				.Select(s => s.AbsolutePath);
		}

		public static string GetAssemblyName(string projectFile)
		{
			if (string.IsNullOrEmpty(projectFile))
				throw new ArgumentNullException(nameof(projectFile), $"{nameof(projectFile)}");
			if (!File.Exists(projectFile))
				throw new FileNotFoundException(projectFile);

			var evaluationProject = GetEvaluationProject(projectFile);
//			var props = evaluationProject.Properties.Where(d => d.EvaluatedValue.Contains("ViewModels")).ToArray();
			return evaluationProject.GetPropertyValue("RootNamespace") ?? string.Empty;
		}

		private async Task ExecuteAsync(IProgress<string> progress, CancellationToken cancellationToken)
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				Solution = await workspace.OpenSolutionAsync(SolutionPath, new Progress<ProjectLoadProgress>(d => UpdateProgress(progress, d)), cancellationToken);
			}

			ProjectsLookup = Solution
				.Projects
				.ToLookup(d => d.FilePath);
		}

		/// <summary>
		/// Path to project -> 1+ projects
		/// </summary>
		public ILookup<string, Project> ProjectsLookup { get; private set; }

		private void UpdateProgress(IProgress<string> progress, ProjectLoadProgress plp)
		{
			Log.Debug($"{plp.Operation.ToString().PadLeft(8)} : {Path.GetFileName(plp.FilePath)}{ToFrameworkString(plp)}");
			progress?.Report($"{plp.Operation.ToString().PadLeft(8)} : {Path.GetFileName(plp.FilePath)}{ToFrameworkString(plp)}");
		}

		private string ToFrameworkString(ProjectLoadProgress plp)
		{
			if (string.IsNullOrEmpty(plp.TargetFramework))
				return null;

			return $" ({plp.TargetFramework})";
		}

		public IEnumerable<string> GetAllReferencedDocuments()
		{
			foreach (var pathToProject in ProjectsLookup)
			{
				foreach (var filePath in GetReferencedDocuments(pathToProject.Key))
				{
					yield return filePath;
				}
			}
		}

		public IEnumerable<string> GetReferencedDocuments(string projectFilePath)
		{
			foreach (var project in ProjectsLookup[projectFilePath])
			{
				foreach (var document in project.Documents)
				{
					yield return document.FilePath;
				}
				foreach (var document in project.AdditionalDocuments)
				{
					yield return document.FilePath;
				}
			}
		}

		public IEnumerable<string> GetAllAdditiontalDocuments()
		{
			return ProjectsLookup.Select(s => s.Key).SelectMany(GetAdditiontalDocuments);
		}

		public IEnumerable<string> GetAdditiontalDocuments(string projectFilePath)
		{
			var folders = new HashSet<string>(
				GetReferencedDocuments(projectFilePath).Select(Path.GetDirectoryName)
					.Concat(Directory.GetDirectories(Path.GetDirectoryName(projectFilePath), "*", SearchOption.AllDirectories))
			);
			folders = FilterArtifacts(projectFilePath, folders);

			foreach (var folder in folders)
			{
				foreach (var file in Directory.GetFiles(folder))
				{
					var extension = Path.GetExtension(file);
					switch (extension)
					{
						case ".resx":
						case ".xaml":
						case ".settings":
						case ".txt":
						case ".html":
						case ".css":
						case ".js":
						case ".tsx":
						case ".ruleset":
						case ".tt":
						case ".json":
						case ".editorconfig":
						case ".ts":
						case ".cshtml":
						case ".sass":
						case ".less":
						case ".config":
						case ".xsd":
						case ".ico":
						case ".png":
						case ".gif":
						case ".jpg":
						case ".jpeg":
						case ".bmp":
						case ".svg":
							yield return file;
							break;

						// included by default, thus not "additional"
						case ".cs":
						case ".csproj":
						case ".vb":
						case ".vbproj":
						case ".vstemplate":
							break;

						default:
							if (AdditionalCopyExtensions.Contains(extension))
							{
								yield return file;
							}
							else
							{
								Log.Warn($"Extension {extension} is currently not supported. You can add more values in the AppSettings for {Constants.Configuration.AdditionalFileCopyExtensions}, or suggest that extension on GitHub to be added.");
							}

							break;
					}
				}
			}
		}

		private static HashSet<string> FilterArtifacts(string projectFilePath, HashSet<string> folders)
		{
			var evaluation = GetEvaluationProject(projectFilePath);
			var bin = Path.Combine(evaluation.DirectoryPath, evaluation.GetPropertyValue("BaseOutputPath").TrimEnd(Path.DirectorySeparatorChar));
			var obj = Path.Combine(evaluation.DirectoryPath, evaluation.GetPropertyValue("BaseIntermediateOutputPath").TrimEnd(Path.DirectorySeparatorChar));
			folders = new HashSet<string>(folders.Where(d => !d.Contains(bin) && !d.Contains(obj)));

			return folders;
		}

		private static Microsoft.Build.Evaluation.Project GetEvaluationProject(string projectFilePath)
		{
			return Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(d => d.FullPath == projectFilePath) ??
				Microsoft.Build.Evaluation.Project.FromFile(projectFilePath, new ProjectOptions());
		}

		public IEnumerable<string> GetAllProjectFiles()
		{
			return this.ProjectsLookup.Select(s => s.Key);
		}
	}
}