using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.DependencyInjection;
using Generator.Shared.FileSystem;
using Generator.Shared.Serialization;
using NLog;

namespace Generator.Shared.ViewModels
{
	public enum IconType
	{
		VisualStudioIcon,
		Path
	}

	public class IconManageViewModel : ViewModelBase
	{
		private IconType _type;

		public IconType Type
		{
			get => _type;
			set => SetValue(ref _type, value, nameof(Type));
		}

		private IconPackageViewModel _currentIcon;

		public IconPackageViewModel CurrentIcon
		{
			get => _currentIcon;
			set => SetValue(ref _currentIcon, value, nameof(CurrentIcon));
		}

		/// <inheritdoc />
		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			if (propertyName == nameof(Type))
			{
				switch (Type)
				{
					case IconType.VisualStudioIcon:
						CurrentIcon = VsIcon;
						break;
					case IconType.Path:
						CurrentIcon = AbsoluteIcon;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private VisualStudioIconViewModel VsIcon;
		private AbsolutePathIconViewModel AbsoluteIcon;

		public static IconManageViewModel Create(IconPackageReference reference, ConfigurationViewModel configuration)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));

			IconManageViewModel vm = new IconManageViewModel();
			vm.VsIcon = new VisualStudioIconViewModel(new IconPackageReference(reference.Package, reference.Id));
			vm.AbsoluteIcon = new AbsolutePathIconViewModel(new IconPackageReference(reference.Path), configuration);

			if (reference.IsVisualStudioIcon())
			{
				vm.VsIcon = new VisualStudioIconViewModel(reference);
				vm.CurrentIcon = vm.VsIcon;
				vm.Type = IconType.VisualStudioIcon;
				return vm;
			}

			if (reference.IsPathIcon())
			{
				vm.AbsoluteIcon = new AbsolutePathIconViewModel(reference, configuration);
				vm.CurrentIcon = vm.AbsoluteIcon;
				vm.Type = IconType.Path;
				return vm;
			}

			vm.Type = IconType.Path;
			vm.CurrentIcon = vm.AbsoluteIcon;
			return vm;
		}

		public IconPackageReference GetModel()
		{
			if (CurrentIcon is VisualStudioIconViewModel vsIcon)
			{
				return new IconPackageReference(vsIcon.Package, vsIcon.Id);
			}
			if (CurrentIcon is AbsolutePathIconViewModel absolutePathIcon)
			{
				return new IconPackageReference(absolutePathIcon.AbsolutePath);
			}

			throw new NotSupportedException($"Unexpected datatype {CurrentIcon.GetType()}.");
		}
	}

	public abstract class IconPackageViewModel : ViewModelBase
	{
	}

	public class VisualStudioIconViewModel : IconPackageViewModel
	{
		private string _package;

		public string Package
		{
			get => _package;
			set => SetValue(ref _package, value, nameof(Package));
		}

		private ushort _id;

		public ushort Id
		{
			get => _id;
			set => SetValue(ref _id, value, nameof(Id));
		}

		/// <inheritdoc />
		public VisualStudioIconViewModel(IconPackageReference model)
		{
			Package = model.Package;
			Id = model.Id;
		}
	}

	public class AbsolutePathIconViewModel : IconPackageViewModel
	{
		private readonly IFileDialogService _fileDialogService;
		private readonly IUIService _uiService;
		private static readonly ILogger Log = LogManager.GetLogger(nameof(IconPackageViewModel));

		public ConfigurationViewModel ConfigurationViewModel { get; }

		private string _absolutePath;

		public string AbsolutePath
		{
			get => _absolutePath;
			set => SetValue(ref _absolutePath, value, nameof(AbsolutePath));
		}

		private ICommand _pickImageCommand;

		public ICommand PickImageCommand
		{
			get => _pickImageCommand ?? (_pickImageCommand = new TaskCommand(PickImageExecute));
			set => SetValue(ref _pickImageCommand, value, nameof(PickImageCommand));
		}

		private Task PickImageExecute(object arg)
		{
			if (string.IsNullOrEmpty(ConfigurationViewModel.ArtifactName))
			{
				_uiService.DisplayError($"{nameof(ConfigurationViewModel.ArtifactName)} can not be empty.", "Error");
				return Task.CompletedTask;
			}

			if (_fileDialogService.OpenFileDialog(out var path, "PNG-File|*.png|ICO-File|*.ico", checkFileExists: true))
			{
				var targetName = GetSuggestedIconPath(ConfigurationViewModel.ArtifactName);
				var targetFileInfo = new FileInfo(targetName);
				if (targetFileInfo.Directory == null)
					throw new Exception($"FileInfo.Directory unavailable.");

				if (!targetFileInfo.Directory.Exists)
				{
					Log.Debug($"Creating foder for {targetName}.");
					targetFileInfo.Directory.Create();
				}

				Log.Debug($"Copying file from {path} to {targetName}.");
				File.Copy(path, targetName, true);

				AbsolutePath = targetName;
			}

			return Task.CompletedTask;
		}

		public static string GetSuggestedIconPath(string artifactName)
		{
			return FileHelper.GetDomainFile("Images", artifactName, "Icon.png");
		}

		/// <inheritdoc />
		public AbsolutePathIconViewModel(IconPackageReference model, ConfigurationViewModel configurationViewModel)
		{
			if(!ServiceLocator.TryGetService(out _fileDialogService))
				throw new Exception($"Service {nameof(IFileDialogService)} is unavailable.");
			if(!ServiceLocator.TryGetService(out _uiService))
				throw new Exception($"Service {nameof(IUIService)} is unavailable.");

			ConfigurationViewModel = configurationViewModel;

			if (string.IsNullOrEmpty(model.Path) && !string.IsNullOrEmpty(configurationViewModel.ArtifactName))
			{
				var suggestedPath = GetSuggestedIconPath(configurationViewModel.ArtifactName);
				if (File.Exists(suggestedPath))
					model.Path = suggestedPath;
			}
			AbsolutePath = model.Path;
		}
	}
}