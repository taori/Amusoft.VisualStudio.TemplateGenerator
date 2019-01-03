using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.DependencyInjection;
using Generator.Shared.Template;
using Generator.Shared.Transformation;

namespace Generator.Shared.ViewModels
{
	public class ConfigurationOverviewViewModel : ContentViewModel
	{
		public ConfigurationOverviewViewModel()
		{
			Commands = new ObservableCollection<TextCommand>(
				new[]
				{
					new TextCommand("New configuration", new TaskCommand(NewConfigurationExecute, d => ConfigurationManager.CanOperate())),
				}
			);

			EditConfigurationCommand = new TaskCommand<ConfigurationViewModel>(EditConfigurationExecute);
			DeleteConfigurationCommand = new TaskCommand<ConfigurationViewModel>(DeleteConfigurationExecute);
			BuildTemplateCommand = new TaskCommand<ConfigurationViewModel>(BuildTemplateExecute, d => d?.CanBuild() ?? false);

			ReloadConfigurationsAsync(null);
		}

		private async Task BuildTemplateExecute(ConfigurationViewModel arg)
		{
			EventHandler ProgressOnCanceled(CancellationTokenSource cts)
			{
				return (sender, args) =>
					cts.Cancel();
			}
			using (var cts = new CancellationTokenSource())
			{
				if(!ServiceLocator.TryGetService(out IUIService uiService))
					throw new Exception($"{nameof(IUIService)} not available.");

				var progress = await uiService.ShowProgressAsync($"Building templates", "loading...", true);
				try
				{
					var rewriter = new RewriteTool(arg.Model);
					progress.Canceled += ProgressOnCanceled(cts);
					await Task.Delay(1000, cts.Token);
					progress.SetIndeterminate();
					await rewriter.ExecuteAsync(cts.Token, new Progress<string>(p => progress.SetMessage(p)));
				}
				catch (TaskCanceledException)
				{
				}
				catch (OperationCanceledException)
				{
				}
				finally
				{
					progress.Canceled -= ProgressOnCanceled(cts);
					await progress.CloseAsync();
				}
			}
		}

		private async Task DeleteConfigurationExecute(ConfigurationViewModel arg)
		{
			if (!ServiceLocator.TryGetService(out IUIService uiService))
				throw new Exception($"{nameof(IUIService)} not available.");

			if (uiService.GetYesNo("Delete for sure?", "Question") 
				&& await ConfigurationManager.DeleteConfigurationAsync(arg.Id))
				await ReloadConfigurationsAsync(null);
		}

		private Task EditConfigurationExecute(ConfigurationViewModel arg)
		{
			if (!ServiceLocator.TryGetService(out IViewModelPresenter viewModelPresenter))
				throw new Exception($"{nameof(IViewModelPresenter)} missing.");
			
			viewModelPresenter.Present(arg);
			return Task.CompletedTask;
		}

		private async void ConfigurationSaved(ConfigurationViewModel obj)
		{
			await ReloadConfigurationsAsync(null);
		}

		private async Task ReloadConfigurationsAsync(object arg)
		{
			var configurations = await ConfigurationManager.LoadStorageContentAsync();
			Items = ConvertItems(configurations);
			foreach (var item in Items)
			{
				item.WhenSaved.Subscribe(ConfigurationSaved);
			}
		}

		private ObservableCollection<ConfigurationViewModel> ConvertItems(Configuration[] configurations)
		{
			return new ObservableCollection<ConfigurationViewModel>(configurations.Select(s => new ConfigurationViewModel(s)));
		}
		
		private async Task NewConfigurationExecute(object arg)
		{
			var configurations = new List<Configuration>(await ConfigurationManager.LoadStorageContentAsync());
			configurations.Add(new Configuration()
			{
				Id = Guid.NewGuid(),
				FileCopyBlacklist = "*.user",
				ConfigurationName = "New configuration",
				ZipContents = true
			});
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

		private ICommand _copyConfigurationCommand;

		public ICommand CopyConfigurationCommand
		{
			get => _copyConfigurationCommand ?? (_copyConfigurationCommand = new TaskCommand<ConfigurationViewModel>(CopyConfigurationExecute));
			set => SetValue(ref _copyConfigurationCommand, value, nameof(CopyConfigurationCommand));
		}

		private async Task CopyConfigurationExecute(ConfigurationViewModel arg)
		{
			if (await ConfigurationManager.CopyConfigurationAsync(arg.Model))
				await ReloadConfigurationsAsync(null);
		}
	}
}