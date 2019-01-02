using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;

namespace Generator.Shared.FileSystem
{
	public static class FileHelper
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(FileHelper));

		public static string FindSolutionAsync(string folder)
			=> Directory.EnumerateFiles(folder, "*.sln", SearchOption.AllDirectories).FirstOrDefault();
		
		public static void CopyFolderContents(CancellationToken cancellationToken, string source, string destination)
		{
			Log.Info($"Copying files from \"{source}\" to \"{destination}\".");
			var sourcePaths = Directory
				.EnumerateFiles(source, "*", SearchOption.AllDirectories);

			foreach (var sourcePath in sourcePaths)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				var relativePath = sourcePath
					.Substring(source.Length)
					.TrimStart(Path.DirectorySeparatorChar);
				var newPath = Path.Combine(destination, relativePath);
				var fileInfo = new FileInfo(newPath);
				if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}
				File.Copy(sourcePath, newPath);
			}

			Log.Info($"Copy complete.");
		}

		private static readonly Regex XmlRegex = new Regex("\\<\\?xml[^>]+\\>\\r?\\n?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static bool RemoveXmlMarker(string filePath)
		{
			if(string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath), $"{nameof(filePath)}");
			if(!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			var content = File.ReadAllText(filePath);
			if (XmlRegex.IsMatch(content))
			{
				var replaced = XmlRegex.Replace(content, string.Empty);
				File.WriteAllText(filePath, replaced);
				return true;
			}

			return false;
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
						Log.Info($"Directory copy: {directory.Name}");
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