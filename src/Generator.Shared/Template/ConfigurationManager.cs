using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Generator.Shared.Template
{
	public static class ConfigurationManager
	{
		public static string ConfigurationStorePath => Path.Combine(ApplicationSettings.Default.ConfigurationStorePath, "storage.json");

		public static async Task<Configuration[]> LoadConfigurationsAsync()
		{
			if (string.IsNullOrEmpty(ApplicationSettings.Default.ConfigurationStorePath) || !File.Exists(ConfigurationStorePath))
				return Array.Empty<Configuration>();

			using (var stream = new StreamReader(new FileStream(ConfigurationStorePath, FileMode.Open)))
			{
				var settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Auto;
				var items = JsonConvert.DeserializeObject<Configuration[]>(await stream.ReadToEndAsync(), settings);
				return items;
			}
		}

		public static async Task SaveConfigurationsAsync(IEnumerable<Configuration> configurations)
		{
			var settings = new JsonSerializerSettings();
			settings.TypeNameHandling = TypeNameHandling.Auto;
			var serialized = JsonConvert.SerializeObject(configurations, Formatting.Indented, settings);
			using (var stream = new StreamWriter(new FileStream(ConfigurationStorePath, FileMode.Create)))
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