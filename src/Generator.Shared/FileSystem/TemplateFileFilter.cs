using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator.Shared.FileSystem
{
	public class TemplateFileFilter : FileWalkerFilter
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
			var projectFiles = Directory.EnumerateFiles(root, "*.vstemplate", SearchOption.AllDirectories);
			Ignores = new HashSet<Uri>(projectFiles.Select(d => new Uri(d, UriKind.Absolute)));
		}

		private HashSet<Uri> Ignores;

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return !Ignores.Contains(new Uri(file, UriKind.Absolute));
		}
	}
}