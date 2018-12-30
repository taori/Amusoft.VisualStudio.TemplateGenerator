using System;
using System.IO;
using System.Linq;

namespace Generator.Shared.FileSystem
{
	public class NugetFolderFilter : FileWalkerFilter
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
			var solutionFile = Directory.EnumerateFiles(root, "*.sln", SearchOption.AllDirectories).FirstOrDefault();
			Ignore = new Uri(Path.Combine(Path.GetDirectoryName(solutionFile), "packages" + Path.DirectorySeparatorChar), UriKind.Absolute);
		}

		public Uri Ignore { get; set; }

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return !Ignore.IsBaseOf(new Uri(file, UriKind.Absolute));
		}
	}
}