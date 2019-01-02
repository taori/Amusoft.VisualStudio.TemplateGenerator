using System;
using System.IO;
using System.Windows.Forms;
using Generator.Shared.DependencyInjection;
using Generator.Shared.Template;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class FilePathProvider : IFilePathProvider
	{
		/// <inheritdoc />
		public bool RequestFolderName(out string folder, string description, string suggestedFolder)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				dialog.Description = description;
				dialog.ShowNewFolderButton = true;
				if (Directory.Exists(ConfigurationManager.GetConfigurationFolder()))
				{
					dialog.SelectedPath = ConfigurationManager.GetConfigurationFolder();
				}

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					folder = dialog.SelectedPath;
					return true;
				}
			}

			folder = null;
			return false;
		}
		
		/// <inheritdoc />
		public bool RequestFileName(out string filePath, string description, string suggestedFolder)
		{
			throw new NotImplementedException();
		}
	}
}