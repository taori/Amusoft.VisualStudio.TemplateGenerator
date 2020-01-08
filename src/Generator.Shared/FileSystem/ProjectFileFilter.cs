using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Generator.Shared.Extensions;

namespace Generator.Shared.FileSystem
{
	public class ProjectFileFilter : FileWalkerFilter, IIgnoreFiles
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
			var projectFiles = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
				.Concat(Directory.EnumerateFiles(root, "*.vbproj", SearchOption.AllDirectories));

			Ignored.Clear();
			Ignored.AddRange(projectFiles.Select(d => new Uri(d, UriKind.Absolute)));
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return !Ignored.Contains(new Uri(file, UriKind.Absolute));
		}

		/// <inheritdoc />
		public HashSet<Uri> Ignored { get; } = new HashSet<Uri>();
	}
}