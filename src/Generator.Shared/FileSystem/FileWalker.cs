using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace Generator.Shared.FileSystem
{
	public static class FileWalker
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(FileWalker));

		public static IEnumerable<string> FromDirectory(string directory, params FileWalkerFilter[] filters)
		{
			foreach (var filter in filters)
			{
				filter.Initialize(directory);
			}

			foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
			{
				foreach (var filter in filters)
				{
					if (!filter.IsValid(file))
					{
						Log.Debug($"Filter [{filter.GetType().Name}] skipped file [{file}].");
						goto skipFile;
					}
				}

				yield return file;

				skipFile:
#pragma warning disable 219
				var noop = 0;
#pragma warning restore 219
			}
		}

		public static IEnumerable<string> FromFile(string path, params FileWalkerFilter[] filters)
		{
			return FromDirectory(Path.GetDirectoryName(path), filters);
		}
	}
}