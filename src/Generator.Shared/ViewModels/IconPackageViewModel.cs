using Generator.Shared.Serialization;

namespace Generator.Shared.ViewModels
{
	public class IconPackageViewModel : ViewModelBase
	{
		/// <inheritdoc />
		public IconPackageViewModel(IconPackageReference model)
		{
			_model = model;
		}

		public static IconPackageViewModel Create(IconPackageReference reference)
		{
			var viewModel = new IconPackageViewModel(reference);
			return viewModel;
		}

		private IconPackageReference _model;

		public IconPackageReference Model
		{
			get => _model;
			set => SetValue(ref _model, value, nameof(Model));
		}
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
		public VisualStudioIconViewModel(IconPackageReference model) : base(model)
		{
		}
	}
}