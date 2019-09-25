using System;
using System.Collections.Generic;
using System.Linq;

namespace Generator.Shared.Transformation
{
	public static class AssemblyNameAligner
	{
		public static string Execute(string request, IList<string> options)
		{
			if (string.IsNullOrEmpty(request))
				throw new ArgumentNullException(nameof(request));
			if (options == null || options.Count == 0)
				throw new ArgumentNullException(nameof(options));

			var sep = new[] { '.' };
			var parts = options
				.Select(s => s.Split(sep, StringSplitOptions.RemoveEmptyEntries))
				.OrderByDescending(d => d.Length)
				.ToList();

			if (parts.Count == 1)
				return string.Empty;

			var shortest = parts[parts.Count - 1].Length;

			var longestMatch = 0;

			for (int col = 0; col < shortest; col++)
			{
				var first = parts[0][col];
				var rowValues = GetRowValues(col, parts);
				if (!rowValues.All(d => string.Equals(d, first)))
					break;

				longestMatch++;
			}

			return string.Join(".", request.Split(sep, StringSplitOptions.RemoveEmptyEntries).Skip(longestMatch));
		}

		private static IEnumerable<string> GetRowValues(int col, List<string[]> parts)
		{
			return parts.Select(d => d[col]);
		}
	}
}