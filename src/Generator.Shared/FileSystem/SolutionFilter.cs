using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Generator.Shared.FileSystem
{
	public class SolutionFilter : FileWalkerFilter, IIgnoreFiles
	{
		public Solution Solution { get; }

		public bool FilterDocuments { get; set; }

		public SolutionFilter(Solution solution)
		{
			Solution = solution;
		}

		/// <inheritdoc />
		public override void Initialize(string root)
		{
			if (FilterDocuments)
			{
				foreach (var uri in Solution
					.Projects
					.SelectMany(s => s.Documents.Select(d => new Uri(d.FilePath, UriKind.Absolute))))
				{
					Ignored.Add(uri);
				}
			}
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return !Ignored.Contains(new Uri(file));
		}

		/// <inheritdoc />
		public HashSet<Uri> Ignored { get; } = new HashSet<Uri>();
	}
}