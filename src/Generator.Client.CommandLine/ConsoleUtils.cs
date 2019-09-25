using System;
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
				configurationManager = storage == null ? ConfigurationManager.Default() : ConfigurationManager.FromPath(storage);
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