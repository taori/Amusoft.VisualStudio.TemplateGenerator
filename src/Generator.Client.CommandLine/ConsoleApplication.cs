using System;
using System.Collections.Generic;
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
			Syntax = "build [configName] [option: -s pathToStorage]"
		)]
		public async Task<int> Build(
			[Argument(
				Name = "config",
				Description = "Name of configuration within configuration store")]
			string configurationId,
			[Option(ShortName = "s")]
			string storage = null
			)
		{
			if (!ConsoleUtils.TryGetManager(storage, out var manager))
				return 3;

			Log.Info($"Trying to build [{configurationId}].");

			Shared.Template.Configuration configuration;
			try
			{
				configuration = await manager.GetConfigurationByIdAsync(configurationId);
			}
			catch (Exception e)
			{
				Log.Error($"No configuration called [{configurationId}] found.");
				Log.Error(e);
				return 2;
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

		[SubCommand]
		[ApplicationMetadata(Description = "Entry point for obtaining information")]
		public class Get
		{
			[ApplicationMetadata(Description = "Retrieves a list of all configurations contained in the storage.")]
			public async Task<int> Configurations(
				[Option(ShortName = "s")]
				string storage = null)
			{
				if (!ConsoleUtils.TryGetManager(storage, out var manager))
					return 1;
				var configurations = await manager.LoadStorageContentAsync();
				for (var index = 0; index < configurations.Length; index++)
				{
					var configuration = configurations[index];
					Console.WriteLine($"(#{index+1:00}): {configuration.ConfigurationName}");
				}

				return 0;
			}
		}

		[SubCommand]
		[ApplicationMetadata(Description = "Entry point for modifying configurations")]
		public class Configuration
		{
			[ApplicationMetadata(Description = "Renames the configuration if it can be found through the given id.")]
			public async Task<int> Rename(
				[Argument(Description = "id of configuration which can be the position of the configuration, its guid, or the configuration name")]
				string id, 
				[Argument(Description = "new name of the configuration")]
				string newName,
				[Option(ShortName = "s")]
				string storage = null)
			{
				if (!ConsoleUtils.TryGetManager(storage, out var manager))
					return 1;
				try
				{
					var configuration = await manager.GetConfigurationByIdAsync(id);
					configuration.Name = newName;
					return await manager.UpdateConfigurationAsync(configuration) ? 0 : 3;
				}
				catch (Exception e)
				{
					Log.Error(e.Message);
					return 2;
				}
			}
		}
	}
}