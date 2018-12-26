namespace Generator.Client.Desktop.ViewModels
{
	public abstract class ScreenViewModel : ViewModelBase
	{
		private ContentViewModel _content;

		public ContentViewModel Content
		{
			get => _content;
			set => SetValue(ref _content, value, nameof(Content));
		}
	}
}