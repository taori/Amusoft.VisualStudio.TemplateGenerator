// Copyright (c) 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using Generator.Shared.Template;
using Generator.Shared.Transformation;
using Newtonsoft.Json;
using NLog;

namespace Generator.Client.CommandLine
{
	[ApplicationMetadata(
		Name = "amusoft.vs.tg")]
	public class ConsoleApplication
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConsoleApplication));

		[ApplicationMetadata(
			Description = "Tries to build a template according the the specified configuration.",
			Syntax = "build [configName] [option: -s pathToStorage]")]
		public async Task<int> Build(
			[Argument(
				Description = "id or name of the configuration to build.")]
			string configurationId,
			[Option(
				Description = "Path to storage.",
				ShortName = "s",
				LongName = "storage")]
			string storage = null)
		{
			if (!ConsoleUtils.TryGetManager(storage, out var manager))
			{
				return 3;
			}

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
				var result = await rewriteTool.ExecuteAsync(
					CancellationToken.None,
					new Progress<string>(message => Console.WriteLine(message)));
				return result ? 0 : 4;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return 1;
			}
		}

		[SubCommand]
		[ApplicationMetadata(Description = "Entry point for configurations.")]
		public class Configuration
		{
			[ApplicationMetadata(Description = "Renames the configuration if it can be found through the given id.")]
			public async Task<int> Rename(
				[Argument(
					Description = "id of configuration which can be the position of the configuration, its guid, or the configuration name")]
				string id,
				[Argument(
					Description = "new name of the configuration")]
				string newName,
				[Option(
					Description = "Path to storage.",
					ShortName = "s",
					LongName = "storage")]
				string storage = null)
			{
				if (!ConsoleUtils.TryGetManager(storage, out var manager))
				{
					return 1;
				}

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

			[ApplicationMetadata(Description = "Saves the local configuration to the specified location.")]
			public async Task<int> Export(
				[Argument(
					Description = "export path")]
				string newPath)
			{
				if (!ConsoleUtils.TryGetManager(null, out var manager))
				{
					Log.Error($"Failed to obtain local storage manager.");
					return 1;
				}

				try
				{
					var configurations = await manager.LoadStorageContentAsync();
					if (!await manager.SaveConfigurationsAsync(configurations, newPath))
					{
						Log.Error($"Failed to save configurations to path [{newPath}].");
						return 3;
					}

					return 0;
				}
				catch (Exception e)
				{
					Log.Error(e.Message);
					return 2;
				}
			}

			[SubCommand]
			[ApplicationMetadata(Description = "Get actions.")]
			public class Get
			{
				[ApplicationMetadata(Description = "Retrieves a list of all configurations contained in the storage.")]
				public async Task<int> All(
					[Option(
						Description = "Path to storage.",
						ShortName = "s",
						LongName = "storage")]
					string storage = null)
				{
					if (!ConsoleUtils.TryGetManager(storage, out var manager))
					{
						return 1;
					}

					var configurations = await manager.LoadStorageContentAsync();
					for (var index = 0; index < configurations.Length; index++)
					{
						var configuration = configurations[index];
						Console.WriteLine($"(#{index + 1:00}): {configuration.ConfigurationName}");
					}

					return 0;
				}

				[ApplicationMetadata(Description = "Gets the json of a configuration identified by the given ID")]
				public async Task<int> Json(
					[Argument(
						Description = "id of configuration which can be the position of the configuration, its guid, or the configuration name")]
					string id,
					[Option(
						Description = "Path to storage.",
						ShortName = "s",
						LongName = "storage")]
					string storage = null)
				{
					if (!ConsoleUtils.TryGetManager(storage, out var manager))
					{
						return 1;
					}

					try
					{
						var configuration = await manager.GetConfigurationByIdAsync(id);
						var serialized = JsonConvert.SerializeObject(configuration, Formatting.Indented);
						Console.WriteLine(serialized);
						return 0;
					}
					catch (Exception e)
					{
						Log.Error(e.Message);
						return 2;
					}
				}
			}

			[SubCommand]
			[ApplicationMetadata(Description = "Import actions.")]
			public class Import
			{
				[ApplicationMetadata(Description = "Updates the local storage from a given path.")]
				public async Task<int> FromFile(
					[Argument(
						Description = "Path to storage file.")]
					string storagePath)
				{
					if (!ConsoleUtils.TryGetManager(null, out var currentManager))
					{
						Log.Error("Failed to obtain local manager instance.");
						return 1;
					}

					if (!ConsoleUtils.TryGetManager(storagePath, out var remoteManager))
					{
						Log.Error($"Failed to obtain remote manager instance using path [{storagePath}].");
						return 2;
					}

					var remoteContents = await remoteManager.LoadStorageContentAsync();
					if (!await currentManager.SaveConfigurationsAsync(remoteContents))
					{
						Log.Error($"Failed to update local manager instance.");
						return 3;
					}

					return 0;
				}
			}
		}
	}
}