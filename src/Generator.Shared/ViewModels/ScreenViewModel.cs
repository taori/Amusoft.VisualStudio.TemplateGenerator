namespace Generator.Shared.ViewModels
{
	public abstract class ScreenViewModel : ViewModelBase
	{
		private string _title;

		public string Title
		{
			get => _title;
			set => SetValue(ref _title, value, nameof(Title));
		}

		private ContentViewModel _content;

		public ContentViewModel Content
		{
			get => _content;
			set => SetValue(ref _content, value, nameof(Content));
		}
	}
}