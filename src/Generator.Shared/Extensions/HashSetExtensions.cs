using System.Collections.Generic;

namespace Generator.Shared.Extensions
{
	public static class HashSetExtensions
	{
		public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				source.Add(item);
			}
		}
	}
}