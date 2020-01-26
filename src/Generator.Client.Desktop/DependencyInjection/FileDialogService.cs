using System.Windows.Forms;
using Generator.Shared.DependencyInjection;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class FileDialogService : IFileDialogService
	{
		/// <inheritdoc />
		public bool OpenFileDialog(out string path, string filter, bool @readonly = false, bool multiSelect = false,
			bool checkFileExists = false)
		{
			using var dialog = new OpenFileDialog();
			dialog.Filter = filter;
			dialog.ReadOnlyChecked = @readonly;
			dialog.Multiselect = multiSelect;
			dialog.CheckFileExists = checkFileExists;

			if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
			{
				path = dialog.FileName;
				return true;
			}

			path = null;
			return false;
		}

		/// <inheritdoc />
		public bool SaveFileDialog(out string path, string filter, bool addExtension)
		{
			using var dialog = new SaveFileDialog();
			dialog.Filter = filter;
			dialog.AddExtension = addExtension;

			if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
			{
				path = dialog.FileName;
				return true;
			}

			path = null;
			return false;
		}

		/// <inheritdoc />
		public bool OpenFolderDialog(out string path, string description, string presetFolder = null, bool newFolderOption = false)
		{
			using var dialog = new FolderBrowserDialog();
			dialog.Description = description;

			if (presetFolder != null)
				dialog.SelectedPath = presetFolder;
			dialog.ShowNewFolderButton = newFolderOption;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				path = dialog.SelectedPath;
				return true;
			}

			path = null;
			return false;
		}
	}
}