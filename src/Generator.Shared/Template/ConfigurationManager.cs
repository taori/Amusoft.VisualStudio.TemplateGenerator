using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Generator.Shared.FileSystem;
using NLog;

namespace Generator.Shared.Template
{
	public class Storage
	{
		public List<Configuration> Configurations { get; set; }
	}

	public static class ConfigurationManager
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConfigurationManager));

		private static string WorkspaceFile => FileHelper.GetDomainFile("storage.json");

		public static async Task<Configuration> GetConfigurationByIdAsync(string id)
		{
			var configurations = await LoadStorageContentAsync();
			if (int.TryParse(id, out var parsedNumber))
			{
				if (parsedNumber < 1 || parsedNumber > configurations.Length)
				{
					throw new Exception($"id out of range [1-{configurations.Length}].");
				}

				return configurations[parsedNumber-1];
			}

			if (Guid.TryParse(id, out var guid))
			{
				return configurations.FirstOrDefault(d => d.Id == guid) ?? throw new Exception($"No configuration matches id [{guid}].");
			}

			return configurations.FirstOrDefault(d => d.ConfigurationName == id) ?? throw new Exception($"No configuration name matches [{id}]");
		}
		
		public static async Task<Configuration[]> LoadStorageContentAsync()
		{
			if (string.IsNullOrEmpty(WorkspaceFile) || !File.Exists(WorkspaceFile))
			{
				Log.Error($"No configuration storage located at {WorkspaceFile}.");
				return Array.Empty<Configuration>();
			}

			using (var stream = new StreamReader(new FileStream(WorkspaceFile, FileMode.Open)))
			{
				var serializer = new XmlSerializer(typeof(Storage));
				var storage = serializer.Deserialize(stream) as Storage;
				return storage.Configurations.ToArray();
			}
		}

		public static async Task<bool> SaveConfigurationsAsync(IEnumerable<Configuration> configurations)
		{
			try
			{

				var dirInfo = new FileInfo(WorkspaceFile);
				if (!dirInfo.Directory.Exists)
					dirInfo.Directory.Create();

				var serializer = new XmlSerializer(typeof(Storage));
				using (var stream = new StreamWriter(new FileStream(WorkspaceFile, FileMode.Create)))
				{
					var storage = new Storage(){Configurations = configurations.ToList() };
					serializer.Serialize(stream, storage);
					return true;
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
		}

		public static async Task<bool> DeleteConfigurationAsync(Guid id)
		{
			var configurations = await LoadStorageContentAsync();
			var filtered = configurations.Where(d => d.Id != id).ToArray();
			await SaveConfigurationsAsync(filtered);
			return configurations.Length != filtered.Length;
		}

		public static async Task<bool> UpdateConfigurationAsync(Configuration configuration)
		{
			var configurations = (await LoadStorageContentAsync()).ToList();
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
			var configurations = (await LoadStorageContentAsync()).ToList();
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