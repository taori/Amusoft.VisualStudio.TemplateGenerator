// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using System;
using System.IO;
using Generator.Shared.Template;
using NLog;

namespace Generator.Client.CommandLine
{
	internal static class ConsoleUtils
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConsoleUtils));

		public static bool TryGetManager(string storage, out ConfigurationManager configurationManager)
		{
			try
			{
				if (!string.IsNullOrEmpty(storage) && !File.Exists(storage))
				{
					Log.Error($"File [{storage}] does not exist.");
					configurationManager = null;
					return false;
				}

				configurationManager = storage == null
					? ConfigurationManager.Default()
					: ConfigurationManager.FromPath(storage);
				return true;
			}
			catch (Exception e)
			{
				Log.Error($"Failed to load {nameof(ConfigurationManager)}");
				Log.Error(e);
				configurationManager = null;
				return false;
			}
		}
	}
}