using System;
using CommandDotNet;
using CommandDotNet.Models;

namespace Generator.Client.CommandLine
{
	class Program
	{
		static void Main(string[] args)
		{
			var runner = new AppRunner<MainEntry>(new AppSettings() {Case = Case.LowerCase});
			runner.Run(args);
		}
	}
}
