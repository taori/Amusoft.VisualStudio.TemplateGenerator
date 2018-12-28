using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.Serialization;

namespace Generator.Client.Desktop.ViewModels
{
	public class MainViewModel : ScreenViewModel
	{
		public MainViewModel()
		{

			Title = "Multi Project Template Generator";
			Content = new ConfigurationOverviewViewModel();
			ShowVersionsCommand = new TaskCommand(ShowVersionsExecute);
		}

		private Task ShowVersionsExecute(object arg)
		{
			var app = typeof(Generator.Client.Desktop.App).Assembly;
			var shared = typeof(VsTemplate).Assembly;
			var message = $"app: {FileVersionInfo.GetVersionInfo(app.Location).FileVersion}{Environment.NewLine}"
			              + $"shared: {FileVersionInfo.GetVersionInfo(shared.Location).FileVersion}";

			MessageBox.Show(message, "Versions");
			return Task.CompletedTask;
		}

		private ICommand _showVersionsCommand;

		public ICommand ShowVersionsCommand
		{
			get => _showVersionsCommand;
			set => SetValue(ref _showVersionsCommand, value, nameof(ShowVersionsCommand));
		}
	}
}