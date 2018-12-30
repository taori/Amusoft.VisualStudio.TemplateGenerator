using System.Collections.Generic;
using System.IO;

namespace Generator.Shared.FileSystem
{
	public static class FileLister
	{
		public static IEnumerable<string> FromDirectory(string directory, FileListerOptions options)
		{
			foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
			{
				if (options.IsValid(file))
					yield return file;
			}
		}

		public static IEnumerable<string> FromSolutionFile(string path, FileListerOptions options)
		{
			foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(path), "*", SearchOption.AllDirectories))
			{
				if (options.IsValid(file))
					yield return file;
			}
		}
	}
}