// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using CommandDotNet;
using CommandDotNet.Models;

namespace Generator.Client.CommandLine
{
	internal static class RunnerFactory
	{
		/// <summary>
		/// Creates App runner instance.
		/// </summary>
		/// <typeparam name="TApplication">Type of application.</typeparam>
		/// <returns>App runner instance.</returns>
		public static AppRunner<TApplication> Create<TApplication>()
			where TApplication : class
		{
			var settings = new AppSettings();
			settings.Case = Case.LowerCase;
			settings.Help.TextStyle = HelpTextStyle.Detailed;
			var runner = new AppRunner<TApplication>(settings);
			return runner;
		}
	}
}