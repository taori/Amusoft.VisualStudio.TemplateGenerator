using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Generator.Shared.Utilities
{
	public class ZipHelper
	{
		private static IEnumerable<FileSystemInfo> AllFilesAndFolders(DirectoryInfo dir)
		{
			foreach (var f in dir.GetFiles())
				yield return f;
			foreach (var d in dir.GetDirectories())
			{
				yield return d;
				foreach (var o in AllFilesAndFolders(d))
					yield return o;
			}
		}

		public static void ZipFolderContents(string sourceFolder, string destinationName)
		{
			var ignoredFile = new FileInfo(destinationName);
			var from = new DirectoryInfo(sourceFolder);
			using (var zipToOpen = new FileStream(destinationName, FileMode.Create))
			{
				using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
				{
					foreach (var file in AllFilesAndFolders(from).Where(o => o is FileInfo && o.FullName != ignoredFile.FullName).Cast<FileInfo>())
					{
						var relPath = file.FullName.Substring(from.FullName.Length + 1);
						ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(file.FullName, relPath);
					}
				}
			}
		}
	}
}