using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Generator.Shared.Transformation
{
	public class SolutionRewriter
	{
		public string SolutionPath { get; }

		public SolutionRewriter(string solutionPath)
		{
			SolutionPath = solutionPath;
		}

		public async Task RewriteAsync(IEnumerable<string> destinationFolders, CancellationToken cancellationToken, IProgress<string> progress)
		{
			var crawler = new SolutionCrawler(SolutionPath);
			await crawler.ExecuteAsync(progress, cancellationToken);
		}
	}
}