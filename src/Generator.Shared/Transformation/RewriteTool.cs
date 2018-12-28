using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Generator.Shared.Template;
using Generator.Shared.Utilities;
using NLog;

namespace Generator.Shared.Transformation
{
	public class RewriteTool
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(RewriteTool));

		public string SolutionPath { get; }

		public Configuration Configuration { get; }

		public RewriteTool(Configuration configuration)
		{
			if (string.IsNullOrEmpty(configuration.SolutionPath)) throw new ArgumentException($"{nameof(SolutionPath)} cannot be null or empty.", nameof(configuration.SolutionPath));
			SolutionPath = configuration.SolutionPath;
			Configuration = configuration;
		}

		public async Task ExecuteAsync(IList<string> destinationFolders, CancellationToken cancellationToken, IProgress<string> progress)
		{
			if (destinationFolders.Count == 0)
				return;

			var context = new SolutionRewriteContext(cancellationToken, progress, Configuration);

			foreach (var folder in destinationFolders)
			{
				Log.Info($"Clearing \"{folder}\".");
				if (!FileHelper.TryClearFolder(context.CancellationToken, folder))
				{
					progress.Report($"Failed to clear \"{folder}\". {Environment.NewLine}See logs for details.");
					await Task.Delay(3000, cancellationToken);
					return;
				}
			}

			if (destinationFolders.Count >= 1)
			{
				var explorer = await SolutionExplorer.CreateAsync(SolutionPath, progress, cancellationToken);
				await CopySolutionAsync(context, explorer, destinationFolders[0]);
				if (!await RewriteSolutionAsync(context, destinationFolders[0]))
				{
					progress.Report($"Solution rewrite of {SolutionPath} failed.{Environment.NewLine}See logs for details.");
					return;
				}

				if (destinationFolders.Count > 1)
					Log.Debug($"Copying to {destinationFolders.Count-1} additional output folders.");

				for (int i = 1; i < destinationFolders.Count; i++)
				{
					FileHelper.CopyFolderContents(context.CancellationToken, destinationFolders[0], destinationFolders[i]);
					await Task.Delay(20, cancellationToken);
				}
			}
		}

		private Task CopySolutionAsync(SolutionRewriteContext context, SolutionExplorer sourceExplorer, string destinationPath)
		{
			Log.Debug($"Copying solution from  \"{sourceExplorer.SolutionPath}\" to \"{destinationPath}\".");
			context.CancellationToken.ThrowIfCancellationRequested();

			var sourceFiles = new HashSet<string>(
				sourceExplorer
					.GetAllReferencedDocuments()
					.Concat(sourceExplorer.GetAllAdditiontalDocuments())
					.Concat(sourceExplorer.GetAllProjectFiles())
					.Concat(new []{ sourceExplorer.SolutionPath })
				);
			
			foreach (var sourceFile in sourceFiles)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return Task.CompletedTask;

				var relativePath = GetRelativePath(SolutionPath, sourceFile);
				var destFileName = Path.Combine(destinationPath, relativePath);
				var fileInfo = new FileInfo(destFileName);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				File.Copy(sourceFile, destFileName);
			}

			return Task.CompletedTask;
		}

		private string GetRelativePath(string solutionPath, string sourceFile)
		{
			var directoryName = Path.GetDirectoryName(solutionPath);
			return sourceFile
				.Substring(directoryName.Length)
				.TrimStart(Path.DirectorySeparatorChar);
		}
		
		private async Task<bool> RewriteSolutionAsync(SolutionRewriteContext context, string destinationFolder)
		{
			var rewriter = new SolutionRewriter(context, destinationFolder);
			return await rewriter.RewriteAsync();
		}
	}
}