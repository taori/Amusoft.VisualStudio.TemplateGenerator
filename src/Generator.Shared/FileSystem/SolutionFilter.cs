using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Generator.Shared.FileSystem
{
	public class SolutionFilter : FileWalkerFilter
	{
		public Solution Solution { get; }

		public bool FilterDocuments { get; set; }

		public SolutionFilter(Solution solution)
		{
			Solution = solution;
		}

		private HashSet<Uri> Ignored = new HashSet<Uri>();

		/// <inheritdoc />
		public override void Initialize(string root)
		{
			Ignored = new HashSet<Uri>();
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
	}
}