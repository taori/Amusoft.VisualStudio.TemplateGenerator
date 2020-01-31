// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace Generator.Shared.DependencyInjection
{
	public static class MSBuildInitializer
	{
		public static void Initialize()
		{
			MSBuildLocator.RegisterInstance(MSBuildLocator.RegisterDefaults());
		}
	}
}