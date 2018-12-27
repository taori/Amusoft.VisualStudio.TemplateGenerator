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
	public struct RewriteContext
	{
		public RewriteContext(CancellationToken cancellationToken, IProgress<string> progress, Configuration configuration)
		{
			CancellationToken = cancellationToken;
			Progress = progress;
			Configuration = configuration;
		}

		public CancellationToken CancellationToken { get; set; }

		public IProgress<string> Progress { get; set; }

		public Configuration Configuration { get; }
	}

	public class SolutionRewriterRunner
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(SolutionRewriterRunner));

		public string SolutionPath { get; }

		public Configuration Configuration { get; }

		public SolutionRewriterRunner(Configuration configuration)
		{
			if (string.IsNullOrEmpty(configuration.SolutionPath)) throw new ArgumentException($"{nameof(SolutionPath)} cannot be null or empty.", nameof(configuration.SolutionPath));
			SolutionPath = configuration.SolutionPath;
			Configuration = configuration;
		}

		public async Task RewriteAsync(IList<string> destinationFolders, CancellationToken cancellationToken, IProgress<string> progress)
		{
			if (destinationFolders.Count == 0)
				return;

			var context = new RewriteContext(cancellationToken, progress, Configuration);

			foreach (var folder in destinationFolders)
			{
				Log.Info($"Clearing \"{folder}\".");
				if (!TryClearFolder(context, folder))
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
				var destinationSolution = FileHelper.FindSolutionAsync(destinationFolders[0]);
				if (string.IsNullOrEmpty(destinationSolution))
				{
					Log.Error($"Failed to find destination solution in \"{destinationFolders[0]}\".");
					progress.Report($"Failed to find destination solution in \"{destinationFolders[0]}\". {Environment.NewLine}See logs for details.");
					await Task.Delay(3000, cancellationToken);
					return;
				}

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

		private async Task CopySolutionAsync(RewriteContext context, SolutionExplorer sourceExplorer, string destinationPath)
		{
			Log.Debug($"Copying solution from  \"{sourceExplorer.SolutionPath}\" to \"{destinationPath}\".");
			await Task.Delay(5000);
			context.CancellationToken.ThrowIfCancellationRequested();

			var sourceFiles = new HashSet<string>(
				sourceExplorer
					.GetAllReferencedDocuments()
					.Concat(sourceExplorer.GetAllAdditiontalDocuments()
			));
			
			foreach (var sourceFile in sourceFiles)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return;

				var relativePath = GetRelativePath(SolutionPath, sourceFile);
				var destFileName = Path.Combine(destinationPath, relativePath);
				var fileInfo = new FileInfo(destFileName);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				File.Copy(sourceFile, destFileName);
			}
		}

		private string GetRelativePath(string solutionPath, string sourceFile)
		{
			var directoryName = Path.GetDirectoryName(solutionPath);
			return sourceFile
				.Substring(directoryName.Length)
				.TrimStart(Path.DirectorySeparatorChar);
		}
		
		private async Task<bool> RewriteSolutionAsync(RewriteContext context, string destinationFolder)
		{
			var rewriter = new SolutionRewriter(context, destinationFolder);
			return await rewriter.RewriteAsync();
		}

		private bool TryClearFolder(RewriteContext context, string folder)
		{
			Log.Trace($"Clearing folder \"{folder}\".");
			if (!Directory.Exists(folder))
			{
				Log.Trace($"folder \"{folder}\" does not exist.");
				return true;
			}

			try
			{
				Log.Trace($"Clearing files of \"{folder}\".");
				foreach (var file in Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly))
				{
					context.CancellationToken.ThrowIfCancellationRequested();
					try
					{
						File.Delete(file);
					}
					catch (Exception e)
					{
						Log.Error($"Failed to delete {file}.");
						Log.Error(e);
						return false;
					}
				}
			}
			catch (Exception e)
			{
				Log.Error($"Failed to get files for {folder}.");
				Log.Error(e);
				return false;
			}

			try
			{
				Log.Trace($"Clearing directories of \"{folder}\".");
				foreach (var subfolder in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
				{
					context.CancellationToken.ThrowIfCancellationRequested();
					try
					{
						if (!TryClearFolder(context, subfolder))
							return false;
						Directory.Delete(subfolder);
					}
					catch (Exception e)
					{
						Log.Error($"Failed to delete {subfolder}.");
						Log.Error(e);
						return false;
					}
				}
			}
			catch (Exception e)
			{
				Log.Error($"Failed to get directories for {folder}.");
				Log.Error(e);
				return false;
			}

			return true;
		}
	}
}