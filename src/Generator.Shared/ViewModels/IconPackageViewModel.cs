using System;
using Generator.Shared.Serialization;

namespace Generator.Shared.ViewModels
{
	public abstract class IconPackageViewModel : ViewModelBase
	{
		/// <inheritdoc />
		protected IconPackageViewModel(IconPackageReference model)
		{
			_model = model;
		}

		public static IconPackageViewModel Create(IconPackageReference reference)
		{
			reference = reference ?? new VisualStudioIcon();

			if(reference is VisualStudioIcon vsIcon)
				return new VisualStudioIconViewModel(vsIcon);

			throw new NotSupportedException($"Unexpected datatype {reference.GetType()}.");
		}

		private IconPackageReference _model;

		public IconPackageReference Model
		{
			get => _model;
			set => SetValue(ref _model, value, nameof(Model));
		}

		public abstract void UpdateModel();
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
			if (model is VisualStudioIcon vsIcon)
			{
				Package = vsIcon.Package;
				Id = vsIcon.Id;
				return;
			}

			throw new Exception($"{model.GetType().FullName} not supported for {nameof(VisualStudioIconViewModel)}.");
		}

		/// <inheritdoc />
		public override void UpdateModel()
		{
			if (Model is VisualStudioIcon model)
			{
				model.Id = Id;
				model.Package = Package;
			}
		}
	}
}