using System;
using System.Threading;
using Generator.Shared.Template;

namespace Generator.Shared.Transformation
{
	public struct ProjectRewriteContext
	{
		public ProjectRewriteCache Cache { get; }

		public string ProjectPath { get; }

		public CancellationToken CancellationToken { get; }

		public Configuration Configuration { get; }

		public SolutionExplorer Explorer { get; }

		public ProjectRewriteContext(ProjectRewriteCache cache, string projectPath, CancellationToken cancellationToken, Configuration configuration, SolutionExplorer explorer)
		{
			Cache = cache ?? throw new ArgumentNullException(nameof(cache));
			ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
			CancellationToken = cancellationToken;
			Configuration = configuration;
			Explorer = explorer;
		}
	}
}