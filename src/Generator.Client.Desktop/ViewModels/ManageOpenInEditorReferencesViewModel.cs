using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.Template;
using Reactive.Bindings;

namespace Generator.Client.Desktop.ViewModels
{
	public class ManageOpenInEditorReferencesViewModel : ScreenViewModel
	{
		public ConfigurationViewModel Configuration { get; }

		public ManageOpenInEditorReferencesViewModel(ConfigurationViewModel configuration)
		{
			Configuration = configuration;
			Filter = new ReactiveProperty<string>();
			Filter
				.Throttle(TimeSpan.FromSeconds(500))
				.Subscribe(FilterChanged);
		}

		public async Task LoadAsync()
		{
			Title = "Loading ...";

			var items = await Task.Run(CreateViewModelsAsync);
			SolutionItems = new ObservableCollection<OpenInEditorToggleViewModel>(items);

			Title = "Manage OpenInEditor references.";
		}

		private OpenInEditorToggleViewModel[] CreateViewModelsAsync()
		{
			var solutionPath = Configuration.SolutionPath;
			var allFiles = Directory.GetFiles(Path.GetDirectoryName(solutionPath), "*", SearchOption.AllDirectories);
			var solutionUri = new Uri(solutionPath, UriKind.Absolute);
			var currentReferences = new HashSet<string>(Configuration.OpenInEditorReferences);
			var viewModels = allFiles.Select(file =>
			{
				var vm = new OpenInEditorToggleViewModel(file, solutionUri);
				vm.Included = currentReferences.Contains(vm.RelativePath.OriginalString);
				return vm;
			}).ToArray();

			return viewModels;
		}

		private void FilterChanged(string obj)
		{
			var view = CollectionViewSource.GetDefaultView(_solutionItems);
			if (string.IsNullOrEmpty(Filter.Value) || string.IsNullOrWhiteSpace(Filter.Value))
			{
				view.Filter = null;
			}
			else
			{
				view.Filter = FilterValue;
			}
		}

		private bool FilterValue(object obj)
		{
			if (obj is OpenInEditorToggleViewModel toggleVm)
			{
				if (toggleVm.RelativePath.OriginalString.Contains(Filter.Value))
					return true;

				return false;
			}

			throw new Exception($"Unexpected datatype {obj.GetType()}. Expecting {nameof(OpenInEditorToggleViewModel)}.");
		}

		public ReactiveProperty<string> Filter { get; } 

		private ObservableCollection<OpenInEditorToggleViewModel> _solutionItems;

		public ObservableCollection<OpenInEditorToggleViewModel> SolutionItems
		{
			get => _solutionItems;
			set => SetValue(ref _solutionItems, value, nameof(SolutionItems));
		}

		private ICommand _toggleSolutionFileCommand;

		public ICommand ToggleSolutionFileCommand
		{
			get => _toggleSolutionFileCommand ?? (_toggleSolutionFileCommand = new TaskCommand<OpenInEditorToggleViewModel>(ToggleSolutionFileExecute));
			set => SetValue(ref _toggleSolutionFileCommand, value, nameof(ToggleSolutionFileCommand));
		}

		private ICommand _commitChangesCommand;

		public ICommand CommitChangesCommand
		{
			get => _commitChangesCommand ?? (_commitChangesCommand = new TaskCommand(CommitChangesExecute));
			set => SetValue(ref _commitChangesCommand, value, nameof(CommitChangesCommand));
		}

		private Task CommitChangesExecute(object arg)
		{
			return Task.CompletedTask;
		}

		private Task ToggleSolutionFileExecute(OpenInEditorToggleViewModel arg)
		{
			arg.Included = !arg.Included;
			return Task.CompletedTask;
		}
	}
}