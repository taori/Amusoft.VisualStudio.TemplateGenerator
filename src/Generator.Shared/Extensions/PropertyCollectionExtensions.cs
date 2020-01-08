using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace Generator.Shared.Extensions
{
	public static class PropertyCollectionExtensions
	{
		public static bool TryGetPropertyValue(this Project source, string key, out string value)
		{
			value = source.GetPropertyValue(key);
			return !string.IsNullOrEmpty(value);
		}
	}
}