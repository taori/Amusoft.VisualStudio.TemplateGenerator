namespace Generator.Client.Desktop.ViewModels
{
	public class IconPackageViewModel : ViewModelBase
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
	}
}