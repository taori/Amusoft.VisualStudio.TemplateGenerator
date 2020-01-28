using System;
using Generator.Shared.DependencyInjection;

namespace Generator.Client.CommandLine.Dependencies
{
	public class FileDialogService : IFileDialogService
	{
		/// <inheritdoc />
		public bool OpenFileDialog(out string path, string filter, bool @readonly = false, bool multiSelect = false,
			bool checkFileExists = false)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public bool SaveFileDialog(out string path, string filter, bool addExtension)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public bool OpenFolderDialog(out string path, string description, string presetFolder = null, bool newFolderOption = false)
		{
			throw new System.NotImplementedException();
		}
	}
}