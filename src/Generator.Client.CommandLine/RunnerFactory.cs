using CommandDotNet;
using CommandDotNet.Models;

namespace Generator.Client.CommandLine
{
	public static class RunnerFactory
	{
		public static AppRunner<TApplication> Create<TApplication>() where TApplication : class => new AppRunner<TApplication>(new AppSettings(){ Case = Case.LowerCase});
	}
}