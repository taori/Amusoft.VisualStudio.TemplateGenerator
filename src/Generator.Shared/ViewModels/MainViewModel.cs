using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.DependencyInjection;
using Generator.Shared.Serialization;

namespace Generator.Shared.ViewModels
{
	public class MainViewModel : ScreenViewModel
	{
		private readonly IUIService _uiService;

		public MainViewModel()
		{
			if (!ServiceLocator.TryGetService(out _uiService))
			{
				throw new Exception($"Failed to find service for {nameof(IUIService)}.");
			}
			Title = "Multi Project Template Generator";
			Content = new ConfigurationOverviewViewModel();
			ShowVersionsCommand = new TaskCommand(ShowVersionsExecute);
		}

		private Task ShowVersionsExecute(object arg)
		{
			var app = Assembly.GetExecutingAssembly();
			var shared = typeof(VsTemplate).Assembly;
			var message = $"app: {FileVersionInfo.GetVersionInfo(app.Location).FileVersion}{Environment.NewLine}"
			              + $"shared: {FileVersionInfo.GetVersionInfo(shared.Location).FileVersion}";

			_uiService.DisplayMessage(message, "Versions");

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