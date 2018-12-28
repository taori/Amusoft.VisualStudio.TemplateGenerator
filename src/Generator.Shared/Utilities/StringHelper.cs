using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Generator.Shared.Utilities
{
	public static class StringHelper
	{
		public static string MultiReplace(string input, Dictionary<string, string> replacements)
		{
			var regex = new Regex(string.Join("|", replacements.Keys));
			var replaced = regex.Replace(input, m => replacements[m.Value]);
			return replaced;
		}
	}
}