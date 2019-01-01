using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet;
using CommandDotNet.Models;
using NLog.Config;

namespace Generator.Client.CommandLine
{
	class Program
	{
		static int Main(string[] args)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				string input;
				do
				{
					Console.WriteLine("Waiting for user input.");
					input = Console.ReadLine();
					if (string.IsNullOrEmpty(input))
						return 0;

					Console.Clear();
					var userArgs = input.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
					var code = RunApplication(userArgs);
					Console.WriteLine(code);
				} while (input != "exit");

				return 0;
			}
			else
			{
				return RunApplication(args);
			}
#else
				return RunApplication(args);
#endif
		}

		private static int RunApplication(string[] args)
		{
			var runner = new AppRunner<ConsoleApplication>(new AppSettings() {Case = Case.LowerCase});
			return runner.Run(args);
		}
	}
}
