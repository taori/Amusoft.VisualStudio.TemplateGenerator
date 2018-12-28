using System;
using System.Threading;
using Generator.Shared.Template;

namespace Generator.Shared.Transformation
{
	public struct SolutionRewriteContext
	{
		public SolutionRewriteContext(CancellationToken cancellationToken, IProgress<string> progress, Configuration configuration)
		{
			CancellationToken = cancellationToken;
			Progress = progress;
			Configuration = configuration;
		}

		public CancellationToken CancellationToken { get; set; }

		public IProgress<string> Progress { get; set; }

		public Configuration Configuration { get; }
	}
}