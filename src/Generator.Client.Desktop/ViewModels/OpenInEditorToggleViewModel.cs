using System;

namespace Generator.Client.Desktop.ViewModels
{
	public class OpenInEditorToggleViewModel : ViewModelBase
	{
		public OpenInEditorToggleViewModel(string filePath, Uri solutionPath)
		{
			FilePath = new Uri(filePath, UriKind.Absolute);
			RelativePath = solutionPath.MakeRelativeUri(FilePath);
		}

		private bool _included;

		public bool Included
		{
			get => _included;
			set => SetValue(ref _included, value, nameof(Included));
		}

		private Uri _relativePath;

		public Uri RelativePath
		{
			get => _relativePath;
			set => SetValue(ref _relativePath, value, nameof(RelativePath));
		}

		private Uri _filePath;

		public Uri FilePath
		{
			get => _filePath;
			set => SetValue(ref _filePath, value, nameof(FilePath));
		}
	}
}