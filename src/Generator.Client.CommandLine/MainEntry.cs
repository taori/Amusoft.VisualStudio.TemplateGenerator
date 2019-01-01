using System;
using CommandDotNet.Attributes;

namespace Generator.Client.CommandLine
{
	public class MainEntry
	{
		[ApplicationMetadata(
			Description = "Tries to build a template according the the specified configuration"
		)]
		public void Build(string configurationName)
		{
			Console.WriteLine($"Trying to build {configurationName}.");
		}
	}
}