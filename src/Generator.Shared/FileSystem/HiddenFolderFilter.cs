using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator.Shared.FileSystem
{
	public class HiddenFolderFilter : FileWalkerFilter
	{
		public readonly HashSet<Uri> HiddenFolders = new HashSet<Uri>();

		/// <inheritdoc />
		public override void Initialize(string root)
		{
			HiddenFolders.Clear();
			var pathString = new string(new[] { Path.DirectorySeparatorChar });
			foreach (var uri in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
				.Select(s => new Uri(s.TrimEnd(Path.DirectorySeparatorChar).Insert(s.Length, pathString), UriKind.Absolute)))
			{
				var directoryInfo = new DirectoryInfo(uri.OriginalString);
				if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
					HiddenFolders.Add(uri);
			}
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			var uri = new Uri(file);
			return HiddenFolders.All(folder =>
			{
				var isBaseOf = folder.IsBaseOf(uri);
				return !isBaseOf;
			});
		}
	}
}