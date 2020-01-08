using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Generator.Shared.Extensions;

namespace Generator.Shared.FileSystem
{
	public class TemplateFileFilter : FileWalkerFilter, IIgnoreFiles
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
			var projectFiles = Directory.EnumerateFiles(root, "*.vstemplate", SearchOption.AllDirectories);
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