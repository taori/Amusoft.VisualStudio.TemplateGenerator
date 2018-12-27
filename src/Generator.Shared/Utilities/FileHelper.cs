using System;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;

namespace Generator.Shared.Utilities
{
	public static class FileHelper
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(FileHelper));

		public static string FindSolutionAsync(string folder)
			=> Directory.EnumerateFiles(folder, "*.sln", SearchOption.AllDirectories).FirstOrDefault();

		public static void CopyFolderContents(CancellationToken cancellationToken, string source, string destination)
		{
			Log.Debug($"Copying files from \"{source}\" to \"{destination}\".");
			var sourcePaths = Directory
				.EnumerateFiles(source, "*", SearchOption.AllDirectories);

			foreach (var sourcePath in sourcePaths)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				var relativePath = sourcePath
					.Substring(sourcePath.Length)
					.TrimStart(Path.DirectorySeparatorChar);
				var newPath = Path.Combine(destination, relativePath);
				var fileInfo = new FileInfo(newPath);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				File.Copy(sourcePath, newPath);
			}

			Log.Debug($"Copy complete.");
		}

		public static bool MoveContents(string source, string destination)
		{
			try
			{
				var destinationDirectory = new DirectoryInfo(destination);
				var sourceDirectory = new DirectoryInfo(source);
				if (!destinationDirectory.Exists)
				{
					destinationDirectory.Create();
				}

				foreach (var file in sourceDirectory.EnumerateFiles())
				{
					Log.Trace($"File copy: {file.Name}");
					file.MoveTo(Path.Combine(destination, file.Name));
				}
				foreach (var directory in sourceDirectory.EnumerateDirectories())
				{
					if (directory.FullName != destination)
					{
						Log.Trace($"Directory copy: {directory.Name}");
						directory.MoveTo(Path.Combine(destination, directory.Name));
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
		}
	}
}