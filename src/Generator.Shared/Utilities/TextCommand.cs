using System.Windows.Input;

namespace Generator.Client.Desktop.Utility
{
	public class TextCommand
	{
		/// <inheritdoc />
		public TextCommand(string text, ICommand command)
		{
			Text = text;
			Command = command;
		}

		public string Text { get; set; }

		public ICommand Command { get; set; }
	}
}