using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;

namespace Generator.Shared.Transformation
{
	public class SolutionExplorer
	{
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

		public IEnumerable<string> GetReferencedDocuments()
		{
			yield return SolutionPath;
			foreach (var projectGroup in ProjectsLookup)
			{
				yield return projectGroup.Key;
				var project = projectGroup.FirstOrDefault();
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

		public IEnumerable<string> GetAdditiontalDocuments()
		{
			var folders = new HashSet<string>(GetReferencedDocuments().Select(Path.GetDirectoryName));
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
						case ".css":
						case ".js":
						case ".ts":
						case ".cshtml":
						case ".sass":
						case ".less":
							yield return file;
							break;
					}
				}
			}
		}
	}
}