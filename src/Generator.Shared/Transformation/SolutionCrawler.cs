using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Generator.Shared.Transformation
{
	public class SolutionCrawler
	{
		private Solution Solution;

		public string SolutionPath { get; }

		public SolutionCrawler(string solutionPath)
		{
			SolutionPath = solutionPath;
		}

		public async Task ExecuteAsync(IProgress<string> progress, CancellationToken cancellationToken)
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				Solution = await workspace.OpenSolutionAsync(SolutionPath, new Progress<ProjectLoadProgress>(d => UpdateProgress(progress, d)), cancellationToken);
			}
		}

		private void UpdateProgress(IProgress<string> progress, ProjectLoadProgress plp)
		{
			progress?.Report($"{plp.Operation} -> {plp.FilePath}");
			Debug.WriteLine($"{plp.Operation} -> {plp.FilePath}");
		}
	}
}