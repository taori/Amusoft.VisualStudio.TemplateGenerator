using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Generator.Client.Desktop.Utility;
using Generator.Client.Desktop.Views;
using Generator.Shared.Configuration;
using Microsoft.Xaml.Behaviors;

namespace Generator.Client.Desktop.ViewModels
{
	public class ConfigurationOverviewViewModel : ContentViewModel
	{
		public ConfigurationOverviewViewModel()
		{
			Commands = new ObservableCollection<TextCommand>(
				new []
				{
					new TextCommand("New configuration", new TaskCommand(NewConfigurationExecute, d => ConfigurationManager.CanOperate())), 
					new TextCommand("Open Configuration folder", new TaskCommand(OpenConfigurationFolderExecute, d => ConfigurationManager.CanOperate())), 
					new TextCommand("Change configuration store", new TaskCommand(SetConfigurationStoreExecute)), 
				}
			);

			EditConfigurationCommand = new TaskCommand<ConfigurationViewModel>(EditConfigurationExecute);
			DeleteConfigurationCommand = new TaskCommand<ConfigurationViewModel>(DeleteConfigurationExecute);
			BuildTemplateCommand = new TaskCommand<ConfigurationViewModel>(BuildTemplateExecute, d => d?.CanBuild() ?? false);
			
			ReloadConfigurationsAsync(null);
		}

		private Task OpenConfigurationFolderExecute(object arg)
		{
			if (ConfigurationManager.GetConfigurationFolder() is var folder && Directory.Exists(folder))
				Process.Start(folder);
			return Task.CompletedTask;
		}

		private Task BuildTemplateExecute(ConfigurationViewModel arg)
		{
			return Task.CompletedTask;
		}

		private async Task DeleteConfigurationExecute(ConfigurationViewModel arg)
		{
			if (System.Windows.MessageBox.Show("Delete for sure?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes 
			    && await ConfigurationManager.DeleteConfigurationAsync(arg.Id))
				await ReloadConfigurationsAsync(null);
		}

		private Task EditConfigurationExecute(ConfigurationViewModel arg)
		{
			var window = new ConfigurationEditWindow();
			Interaction.GetBehaviors(window).Add(new CloseOnDoubleEscapeBehavior());
			var editModel = new ConfigurationViewModel(arg.Model);
			window.DataContext = editModel;
			editModel.WhenConfirm.Subscribe(SaveConfiguration);
			editModel.WhenConfirm.Subscribe(_ => window.Close());
			editModel.WhenDiscard.Subscribe(_ => window.Close());
			window.Show();
			return Task.CompletedTask;
		}

		private async void SaveConfiguration(ConfigurationViewModel obj)
		{
			Debug.WriteLine($"Saving configuration {obj.Id}.");
			obj.UpdateModel();

			if (await ConfigurationManager.UpdateConfigurationAsync(obj.Model))
			{
				Debug.WriteLine($"Update successful.");
				await ReloadConfigurationsAsync(null);
			}
		}

		private async Task ReloadConfigurationsAsync(object arg)
		{
			var configurations = await ConfigurationManager.LoadConfigurationsAsync();
			Items = ConvertItems(configurations);
		}

		private ObservableCollection<ConfigurationViewModel> ConvertItems(Configuration[] configurations)
		{
			return new ObservableCollection<ConfigurationViewModel>(configurations.Select(s => new ConfigurationViewModel(s)));
		}

		private async Task SetConfigurationStoreExecute(object arg)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				dialog.Description = "Pick a folder for the storage to be located.";
				dialog.ShowNewFolderButton = true;
				if (Directory.Exists(ConfigurationManager.GetConfigurationFolder()))
				{
					dialog.SelectedPath = ConfigurationManager.GetConfigurationFolder();
				}

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					ConfigurationManager.SetConfigurationStore(dialog.SelectedPath);
					await Task.Delay(50);
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}

		private async Task NewConfigurationExecute(object arg)
		{
			var configurations = new List<Configuration>(await ConfigurationManager.LoadConfigurationsAsync());
			configurations.Add(new Configuration(){Id = Guid.NewGuid(), ConfigurationName = "New configuration"});
			await ConfigurationManager.SaveConfigurationsAsync(configurations);
			await ReloadConfigurationsAsync(null);
		}

		private ObservableCollection<ConfigurationViewModel> _items;

		public ObservableCollection<ConfigurationViewModel> Items
		{
			get => _items ?? (_items = new ObservableCollection<ConfigurationViewModel>());
			set => SetValue(ref _items, value, nameof(Items));
		}

		private ObservableCollection<TextCommand> _commands;

		public ObservableCollection<TextCommand> Commands
		{
			get => _commands ?? (_commands = new ObservableCollection<TextCommand>());
			set => SetValue(ref _commands, value, nameof(Commands));
		}

		private ICommand _editConfigurationCommand;

		public ICommand EditConfigurationCommand
		{
			get => _editConfigurationCommand;
			set => SetValue(ref _editConfigurationCommand, value, nameof(EditConfigurationCommand));
		}

		private ICommand _deleteConfigurationCommand;

		public ICommand DeleteConfigurationCommand
		{
			get => _deleteConfigurationCommand;
			set => SetValue(ref _deleteConfigurationCommand, value, nameof(DeleteConfigurationCommand));
		}

		private ICommand _buildTemplateCommand;

		public ICommand BuildTemplateCommand
		{
			get => _buildTemplateCommand;
			set => SetValue(ref _buildTemplateCommand, value, nameof(BuildTemplateCommand));
		}
	}
}