using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
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

		public static IconManageViewModel Create(IconPackageReference reference, string artifactName)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));

			IconManageViewModel vm = new IconManageViewModel();
			vm.VsIcon = new VisualStudioIconViewModel(new VisualStudioIcon());
			vm.AbsoluteIcon = new AbsolutePathIconViewModel(new AbsolutePathIcon(), artifactName);

			if (reference is VisualStudioIcon vsIcon)
			{
				vm.VsIcon = new VisualStudioIconViewModel(vsIcon);
				vm.CurrentIcon = vm.VsIcon;
				vm.Type = IconType.VisualStudioIcon;
			}

			if (reference is AbsolutePathIcon pathIcon)
			{
				vm.AbsoluteIcon = new AbsolutePathIconViewModel(pathIcon, artifactName);
				vm.CurrentIcon = vm.AbsoluteIcon;
				vm.Type = IconType.Path;
			}

			return vm;
		}

		public IconPackageReference GetModel()
		{
			if (CurrentIcon is VisualStudioIconViewModel vsIcon)
			{
				return new VisualStudioIcon(vsIcon.Package, vsIcon.Id);
			}
			if (CurrentIcon is AbsolutePathIconViewModel absolutePathIcon)
			{
				return new AbsolutePathIcon(absolutePathIcon.AbsolutePath);
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
			if (model is VisualStudioIcon vsIcon)
			{
				Package = vsIcon.Package;
				Id = vsIcon.Id;
				return;
			}

			throw new Exception($"{model.GetType().FullName} not supported for {nameof(VisualStudioIconViewModel)}.");
		}
	}

	public class AbsolutePathIconViewModel : IconPackageViewModel
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(IconPackageViewModel));

		public string ArtifactName { get; }

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
			using (var dialog = new OpenFileDialog())
			{
				dialog.CheckFileExists = true;
				dialog.Multiselect = false;
				dialog.Filter = "ICO-File|*.ico|PNG-File|*.png";

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var targetName = GetSuggestedIconPath(ArtifactName);
					var targetFileInfo = new FileInfo(targetName);
					if(targetFileInfo.Directory == null)
						throw new Exception($"FileInfo.Directory unavailable.");

					if (!targetFileInfo.Directory.Exists)
					{
						Log.Debug($"Creating foder for {targetName}.");
						targetFileInfo.Directory.Create();
					}
					
					Log.Debug($"Copying file from {dialog.FileName} to {targetName}.");
					File.Copy(dialog.FileName, targetName, true);
					AbsolutePath = targetName;
				}
			}

			return Task.CompletedTask;
		}

		public static string GetSuggestedIconPath(string artifactName) => FileHelper.GetDomainFile("Images", artifactName, "Icon.png");

		/// <inheritdoc />
		public AbsolutePathIconViewModel(IconPackageReference model, string artifactName)
		{
			ArtifactName = artifactName;

			if (model is AbsolutePathIcon icon)
			{
				if (string.IsNullOrEmpty(icon.Path) && !string.IsNullOrEmpty(artifactName))
				{
					var suggestedPath = GetSuggestedIconPath(artifactName);
					if (File.Exists(suggestedPath))
						icon.Path = suggestedPath;
				}
				AbsolutePath = icon.Path;
				return;
			}

			throw new Exception($"{model.GetType().FullName} not supported for {nameof(AbsolutePathIconViewModel)}.");
		}
	}
}