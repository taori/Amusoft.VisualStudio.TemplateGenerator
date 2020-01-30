using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet;
using CommandDotNet.Models;
using Generator.Client.CommandLine.Dependencies;
using Generator.Shared.DependencyInjection;
using NLog;
using NLog.Config;

namespace Generator.Client.CommandLine
{
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				MSBuildInitializer.Initialize();
				ServiceLocatorInitializer.Initialize();
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
			catch (Exception e)
			{
				Console.Out.WriteLine(e);
				LogManager.Flush(TimeSpan.FromSeconds(10));
				return -1;
			}
		}

		private static int RunApplication(string[] args)
		{
			var runner = RunnerFactory.Create<ConsoleApplication>();
			return runner.Run(args);
		}
	}
}
