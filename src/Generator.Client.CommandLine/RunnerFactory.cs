using CommandDotNet;
using CommandDotNet.Models;

namespace Generator.Client.CommandLine
{
	public static class RunnerFactory
	{
		public static AppRunner<TApplication> Create<TApplication>() where TApplication : class
		{
			var settings = new AppSettings();
			settings.Case = Case.LowerCase;
			settings.HelpTextStyle = HelpTextStyle.Detailed;
			var runner = new AppRunner<TApplication>(settings);
			return runner;
		}
	}
}