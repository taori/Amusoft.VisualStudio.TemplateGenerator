using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Generator.Shared.Utilities
{
	public static class StringHelper
	{
		public static bool TryMultiReplace(string input, Dictionary<string, string> replacements, ref string result)
		{
			var regex = new Regex(string.Join("|", replacements.Keys));
			if (regex.IsMatch(input))
			{
				result = regex.Replace(input, m => replacements[m.Value]);
				return true;
			}

			result = input;
			return false;
		}
	}
}