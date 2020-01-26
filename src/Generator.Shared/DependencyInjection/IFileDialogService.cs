namespace Generator.Shared.DependencyInjection
{
	public interface IFileDialogService
	{
		bool OpenFileDialog(out string path, string filter, bool @readonly = false, bool multiSelect = false, bool checkFileExists = false);
		bool SaveFileDialog(out string path, string filter, bool addExtension);
		bool OpenFolderDialog(out string path, string description, string presetFolder = null, bool newFolderOption = false);
	}
}