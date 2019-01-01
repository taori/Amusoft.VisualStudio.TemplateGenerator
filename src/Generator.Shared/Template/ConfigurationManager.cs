using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace Generator.Shared.Template
{
	public static class ConfigurationManager
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConfigurationManager));

		private static string BuildStoreFilePath(string storeDirectory)
		{
			return Path.Combine(storeDirectory, "storage.json");
		}

		private static string GetChosenStorage(string storageFile)
		{
			var chosenFile = storageFile ?? BuildStoreFilePath(ApplicationSettings.Default.ConfigurationStorePath);
			return chosenFile;
		}

		public static async Task<Configuration[]> LoadConfigurationsAsync(string storageFile = null)
		{
			var chosenFile = GetChosenStorage(storageFile);
			if (string.IsNullOrEmpty(chosenFile) || !File.Exists(chosenFile))
			{
				Log.Error($"No configuration storage located at {chosenFile}.");
				return Array.Empty<Configuration>();
			}

			using (var stream = new StreamReader(new FileStream(chosenFile, FileMode.Open)))
			{
				var settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Auto;
				var items = JsonConvert.DeserializeObject<Configuration[]>(await stream.ReadToEndAsync(), settings);
				return items;
			}
		}

		public static async Task SaveConfigurationsAsync(IEnumerable<Configuration> configurations, string storageFile = null)
		{
			var chosenFile = GetChosenStorage(storageFile);
			var settings = new JsonSerializerSettings();
			settings.TypeNameHandling = TypeNameHandling.Auto;
			var serialized = JsonConvert.SerializeObject(configurations, Formatting.Indented, settings);
			using (var stream = new StreamWriter(new FileStream(chosenFile, FileMode.Create)))
			{
				await stream.WriteAsync(serialized);
			}
		}

		public static void SetConfigurationStore(string path)
		{
			ApplicationSettings.Default.ConfigurationStorePath = path;
			ApplicationSettings.Default.Save();
		}

		public static string GetConfigurationFolder()
		{
			return ApplicationSettings.Default.ConfigurationStorePath;
		}

		public static async Task<bool> DeleteConfigurationAsync(Guid id)
		{
			var configurations = await LoadConfigurationsAsync();
			var filtered = configurations.Where(d => d.Id != id).ToArray();
			await SaveConfigurationsAsync(filtered);
			return configurations.Length != filtered.Length;
		}

		public static async Task<bool> UpdateConfigurationAsync(Configuration configuration)
		{
			var configurations = (await LoadConfigurationsAsync()).ToList();
			var index = configurations.FindIndex(d => d.Id == configuration.Id);

			if(index >= 0)
			{
				configurations.RemoveAt(index);
				configurations.Insert(index, configuration);
				await SaveConfigurationsAsync(configurations);
				return true;
			}

			return false;
		}

		public static bool CanOperate()
		{
			return !string.IsNullOrEmpty(ApplicationSettings.Default.ConfigurationStorePath)
			       && Directory.Exists(ApplicationSettings.Default.ConfigurationStorePath);
		}

		public static async Task<bool> CopyConfigurationAsync(Configuration configuration)
		{
			var configurations = (await LoadConfigurationsAsync()).ToList();
			var index = configurations.FindIndex(d => d.Id == configuration.Id);

			if (index >= 0)
			{
				var item = configurations[index];
				var clone = item.Clone() as Configuration;
				clone.Id = Guid.NewGuid();
				configurations.Insert(index+1, clone);
				await SaveConfigurationsAsync(configurations);
				return true;
			}

			return false;
		}
	}
}