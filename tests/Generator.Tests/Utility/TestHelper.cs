using System;
using System.IO;
using System.Threading.Tasks;

namespace Generator.Tests.Utility
{
	public static class TestHelper
	{
		public static async Task<string> GetTestFileContentAsync(string name)
		{
			var fixedName = $"Generator.Tests.{name}";
			using (var stream = typeof(TestHelper).Assembly.GetManifestResourceStream(fixedName))
			{
				if(stream == null)
					throw new Exception($"Manifest resource {name} not found.");

				using (var reader = new StreamReader(stream))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}
	}
}