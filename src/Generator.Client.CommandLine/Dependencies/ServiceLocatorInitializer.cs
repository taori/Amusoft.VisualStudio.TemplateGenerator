// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using Generator.Shared.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Generator.Client.CommandLine.Dependencies
{
	public class ServiceLocatorInitializer
	{
		public static void Initialize()
		{
			ServiceLocator.Build(Configure);
		}

		private static void Configure(ServiceCollection services)
		{
			services.AddSingleton<IUIService, UiService>();
			services.AddSingleton<IFileDialogService, FileDialogService>();
		}
	}
}