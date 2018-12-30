using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator.Shared.FileSystem
{
	public static class FileWalker
	{
		public static IEnumerable<string> FromDirectory(string directory, params FileWalkerFilter[] filters)
		{
			foreach (var filter in filters)
			{
				filter.Initialize(directory);
			}

			foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
			{
				if (filters.All(d => d.IsValid(file)))
					yield return file;
			}
		}

		public static IEnumerable<string> FromFile(string path, params FileWalkerFilter[] filters)
		{
			return FromDirectory(Path.GetDirectoryName(path), filters);
		}
	}
}