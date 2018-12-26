using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Generator.Shared.Transformation
{
	public struct RewriteContext
	{
		public RewriteContext(CancellationToken cancellationToken, IProgress<string> progress)
		{
			CancellationToken = cancellationToken;
			Progress = progress;
		}

		public CancellationToken CancellationToken { get; set; }

		public IProgress<string> Progress { get; set; }
	}

	public class SolutionRewriterRunner
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(SolutionRewriterRunner));

		public string SolutionPath { get; }
		
		public SolutionRewriterRunner(string solutionPath)
		{
			if (string.IsNullOrEmpty(solutionPath)) throw new ArgumentException("Value cannot be null or empty.", nameof(solutionPath));
			SolutionPath = solutionPath;
		}

		public async Task RewriteAsync(IList<string> destinationFolders, CancellationToken cancellationToken, IProgress<string> progress)
		{
			if (destinationFolders.Count == 0)
				return;

			var context = new RewriteContext(cancellationToken, progress);

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
				var destinationSolution = await FindSolutionAsync(context, destinationFolders[0]);
				if (string.IsNullOrEmpty(destinationSolution))
				{
					Log.Error($"Failed to find destination solution in \"{destinationFolders[0]}\".");
					progress.Report($"Failed to find destination solution in \"{destinationFolders[0]}\". {Environment.NewLine}See logs for details.");
					await Task.Delay(3000, cancellationToken);
					return;
				}

				await RewriteSolutionAsync(context, destinationSolution);

				for (int i = 1; i < destinationFolders.Count; i++)
				{
					CopyFolderContents(context, destinationFolders[0], destinationFolders[i]);
				}
			}
		}

		private async Task<string> FindSolutionAsync(RewriteContext context, string destinationFolder)
		{
			return Directory.EnumerateFiles(destinationFolder, "*.sln", SearchOption.AllDirectories).FirstOrDefault();
		}

		private async Task CopySolutionAsync(RewriteContext context, SolutionExplorer sourceExplorer, string destinationPath)
		{
			Log.Debug($"Copying solution from  \"{sourceExplorer.SolutionPath}\" to \"{destinationPath}\".");
			await Task.Delay(5000);
			context.CancellationToken.ThrowIfCancellationRequested();

			var sourceFiles = new HashSet<string>(
				sourceExplorer
					.GetReferencedDocuments()
					.Concat(sourceExplorer.GetAdditiontalDocuments()
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

		private void CopyFolderContents(RewriteContext context, string sourceFolder, string destinationFolder)
		{
			Log.Debug($"Copying files from \"{sourceFolder}\" to \"{destinationFolder}\".");
			var sourcePaths = Directory
				.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories);

			foreach (var sourcePath in sourcePaths)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return;

				var relativePath = sourcePath
					.Substring(sourceFolder.Length)
					.TrimStart(Path.DirectorySeparatorChar);
				var newPath = Path.Combine(destinationFolder, relativePath);
				var fileInfo = new FileInfo(newPath);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				File.Copy(sourcePath, newPath);
			}

			Log.Debug($"Copy complete.");
		}

		private Task RewriteSolutionAsync(RewriteContext context, string solutionPath)
		{
			Log.Info($"Rewriting \"{solutionPath}\".");
			//			var crawler = new SolutionCrawler(SolutionPath);
			//			await crawler.ExecuteAsync(context.Progress, context.CancellationToken);
			return Task.CompletedTask;
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