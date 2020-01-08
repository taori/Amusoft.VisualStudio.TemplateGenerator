using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Generator.Shared.Extensions;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;

namespace Generator.Shared.FileSystem
{
	public class BuildArtifactsFilter : FileWalkerFilter, IIgnoreFiles
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
			Ignored.Clear();

			var projectFiles = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories);
			var projects = projectFiles.Select(GetEvaluationProject);
			foreach (var project in projects)
			{
				var bin = GetBinPath(project);
				var obj = GetObjPath(project)
					+ Path.DirectorySeparatorChar;

				Ignored.Add(new Uri(bin));
				Ignored.Add(new Uri(obj));
			}
		}

		private static string GetObjPath(Project project)
		{
			return Path.Combine(project.DirectoryPath, project.GetPropertyValue("BaseIntermediateOutputPath")
				.TrimEnd(Path.DirectorySeparatorChar));
		}

		private static string GetBinPath(Project project)
		{
			string binPath = string.Empty;

			if (string.IsNullOrEmpty(binPath) && project.TryGetPropertyValue("BaseOutputPath", out var bin1))
				binPath = bin1;

			if (string.IsNullOrEmpty(binPath) && project.TryGetPropertyValue("OutputPath", out var bin2))
			{
				binPath = bin2;
				if (project.TryGetPropertyValue("Configuration", out var configuration))
				{
					// turns "bin/Debug" into "bin" or "bin/Release" into "bin"
					if (binPath.EndsWith(configuration, StringComparison.OrdinalIgnoreCase)
					|| binPath.EndsWith(configuration + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
						binPath = string.Join(Path.DirectorySeparatorChar.ToString(), binPath.Split(Path.DirectorySeparatorChar).Reverse().Skip(2).Reverse());
				}
			}

			if(binPath == null)
				throw new Exception($"There is no output path present for the project {project.FullPath}.");

			return Path.Combine(project.DirectoryPath, binPath.TrimEnd(Path.DirectorySeparatorChar))
			       + Path.DirectorySeparatorChar;
		}

		private static Project GetEvaluationProject(string file)
		{
			return ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(d => d.FullPath == file)
				   ?? Project.FromFile(file, new ProjectOptions());
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			var uri = new Uri(file, UriKind.Absolute);
			return Ignored.All(ignored => !ignored.IsBaseOf(uri));
		}

		/// <inheritdoc />
		public HashSet<Uri> Ignored { get; } = new HashSet<Uri>();
	}
}