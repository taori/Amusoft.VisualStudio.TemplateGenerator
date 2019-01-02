using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using Generator.Shared.Template;
using Generator.Shared.Transformation;
using NLog;

namespace Generator.Client.CommandLine
{
	public class ConsoleApplication
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConsoleApplication));

		[ApplicationMetadata(
			Description = "Tries to build a template according the the specified configuration",
			Name = "build",
			Syntax = "build [configName] [option: -s pathToStorage]"
		)]
		public async Task<int> Build(
			[Argument(
				Name = "config",
				Description = "Name of configuration within configuration store")]
			string configurationName,

			[Option(
				Description = "Path to configuration store",
				ShortName = "s",
				LongName = "storage")]
			string configurationStorePath)
		{
			Log.Info($"Trying to build [{configurationName}].");

			if(!string.IsNullOrEmpty(configurationStorePath))
			Log.Info($"Loading configurations from [{configurationStorePath}].");
			var configurations = await ConfigurationManager.LoadStorageContentAsync();
			if (configurations.Length == 0)
			{
				Log.Error($"Cannot build. No configurations available.");
				return 2;
			}
			var configuration = configurations.FirstOrDefault(d => string.Equals(d.Name, configurationName, StringComparison.OrdinalIgnoreCase));
			if (configuration == null)
			{
				Log.Error($"Found no configuration called [{configurationName}].");
				return 3;
			}

			var rewriteTool = new RewriteTool(configuration);
			try
			{
				Log.Info($"Executing build tool.");
				var result = await rewriteTool.ExecuteAsync(CancellationToken.None, new Progress<string>(message => Console.WriteLine(message)));
				return result ? 0 : 4;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return 1;
			}
		}

		public int Get(params string[] args)
		{
			var runner = new AppRunner<GetApplication>();
			return runner.Run(args);
		}
	}

	public class GetApplication
	{
			
	}
}