using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Generator.Shared.FileSystem;
using Generator.Shared.Template;
using Generator.Shared.Utilities;
using Generator.Shared.ViewModels;
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

		public async Task<bool> ExecuteAsync(CancellationToken cancellationToken, IProgress<string> progress)
		{
			ConfigurationViewModel viewModel;
			try
			{
				viewModel = new ConfigurationViewModel(Configuration);
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
			
			if (!viewModel.CanBuild())
			{
				Log.Error($"cannot build the configuration [{Configuration.Name}] because of validation errors.");
				foreach (var error in viewModel.ValidationErrors)
				{
					Log.Error(error);
				}

				return false;
			}
			if (Configuration.OutputFolders.Count == 0)
			{
				Log.Error($"No output paths specified.");
				progress.Report($"No output paths specified.");
				await Task.Delay(3000, cancellationToken);
				return false;
			}

			var tempFolder = CreateTempFolder();
			var context = new SolutionRewriteContext(cancellationToken, progress, Configuration);
			if (!Directory.Exists(tempFolder))
			{
				progress.Report($"Failed to create temp folder");
				await Task.Delay(3000, cancellationToken);
				Log.Error($"Failed to create temp folder");
				return false;
			}

			var explorer = await SolutionExplorer.CreateAsync(SolutionPath, progress, cancellationToken);
			await CopySolutionAsync(context, explorer, tempFolder);

			if (!await RewriteSolutionAsync(context, tempFolder))
			{
				progress.Report($"Solution rewrite of {SolutionPath} failed.{Environment.NewLine}See logs for details.");
				return false;
			}

			await DistributeArtifacts(cancellationToken, context, tempFolder);
			Log.Info($"Deleting temporary working folder {tempFolder}.");
			Directory.Delete(tempFolder, true);

			Log.Info($"Execution finished succesfully.");

			return true;
		}

		private string CreateTempFolder()
		{
			var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			var info = new DirectoryInfo(folder);
			info.Create();
			return info.FullName;
		}

		private async Task DistributeArtifacts(CancellationToken cancellationToken, SolutionRewriteContext context, string tempFolder)
		{
			Log.Info($"Deploying to the following destinations: {Environment.NewLine}{string.Join(Environment.NewLine, Configuration.OutputFolders)}");

			if (Configuration.ZipContents)
			{
				Log.Info("Zip distribution is enabled.");
				var zipName = Path.GetTempFileName();
				try
				{
					Log.Debug($"Creating zip file with contents of {tempFolder} at {zipName}.");

					ZipHelper.ZipFolderContents(tempFolder, zipName);

					foreach (var destinationFolder in Configuration.OutputFolders)
					{
						var destinationName = Path.Combine(destinationFolder, $"{Configuration.ArtifactName}.zip");
						Log.Info($"Copying file {zipName} to {destinationName}.");
						File.Copy(zipName, destinationName, true);
						await Task.Delay(20, cancellationToken);
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
					throw;
				}
				finally
				{
					Log.Info($"Deleting tmp file {zipName}.");
					File.Delete(zipName);
				}
			}
			else
			{
				foreach (var folder in Configuration.OutputFolders)
				{
					var destination = Path.Combine(folder, Configuration.ArtifactName);
					FileHelper.CopyFolderContents(context.CancellationToken, tempFolder, destination);
					await Task.Delay(20, cancellationToken);
				}
			}
		}

		private class EndsWithComparer : IEqualityComparer<string>
		{
			/// <inheritdoc />
			public bool Equals(string x, string y)
			{
				if (x == null && y == null)
					return true;
				if (x == null || y == null)
					return false;

				return y.EndsWith(x);
			}

			/// <inheritdoc />
			public int GetHashCode(string obj)
			{
				return obj.GetHashCode();
			}
		}
		private Task CopySolutionAsync(SolutionRewriteContext context, SolutionExplorer sourceExplorer, string destinationPath)
		{
			Log.Debug($"Copying solution from  \"{sourceExplorer.SolutionPath}\" to \"{destinationPath}\".");
			context.CancellationToken.ThrowIfCancellationRequested();

			var endsWithComparer = new EndsWithComparer();
			var blackList = Configuration.GetFileCopyBlackListElements().Select(s => s.TrimStart('*')).ToImmutableHashSet();
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
				if (blackList.Contains(fileInfo.Name, endsWithComparer))
					continue;

				if (!fileInfo.Directory.Exists)
					fileInfo.Directory.Create();
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