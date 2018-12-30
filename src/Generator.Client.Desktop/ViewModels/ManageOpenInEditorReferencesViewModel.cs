using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.FileSystem;
using NLog;
using Reactive.Bindings;

namespace Generator.Client.Desktop.ViewModels
{
	public class ManageOpenInEditorReferencesViewModel : ScreenViewModel
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ManageOpenInEditorReferencesViewModel));

		public ConfigurationViewModel Configuration { get; }

		private Subject<ConfigurationViewModel> _whenConfirmed = new Subject<ConfigurationViewModel>();
		public IObservable<ConfigurationViewModel> WhenConfirmed => _whenConfirmed;

		private Subject<ConfigurationViewModel> _whenDiscarded = new Subject<ConfigurationViewModel>();
		public IObservable<ConfigurationViewModel> WhenDiscarded => _whenDiscarded;

		public ManageOpenInEditorReferencesViewModel(ConfigurationViewModel configuration)
		{
			Title = "Manage OpenInEditor references.";
			Configuration = configuration;
			Filter = new ReactiveProperty<string>();
			Filter
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Subscribe(FilterChanged);
		}

		public async Task LoadAsync()
		{
			Title = "Loading ...";

			_allItems.AddRange(await Task.Run(BuildAllViewModelsAsync));
			ItemsSubset = new ObservableCollection<OpenInEditorToggleViewModel>(_allItems);

			Title = "Manage OpenInEditor references.";
		}

		private IEnumerable<OpenInEditorToggleViewModel> BuildAllViewModelsAsync()
		{
			Log.Info("Loading files.");

			var solutionPath = Configuration.SolutionPath;
			var filters = new FileWalkerFilter[]
			{
				new HiddenFolderFilter(),
				new SolutionFileFilter(), 
				new BuildArtifactsFilter(), 
				new NugetFolderFilter(), 
			};
			var allFiles = FileWalker.FromFile(solutionPath, filters);
			var solutionUri = new Uri(solutionPath, UriKind.Absolute);
			var currentReferences = new HashSet<string>(Configuration.OpenInEditorReferences);
			var viewModels = allFiles.Select(file =>
			{
				var vm = new OpenInEditorToggleViewModel(file, solutionUri);
				vm.Included = currentReferences.Contains(vm.RelativePath.OriginalString);
				return vm;
			});

			Log.Info("Files loaded.");

			return viewModels;
		}

		private void FilterChanged(string filterValue)
		{
			Log.Info($"{nameof(FilterChanged)}: {filterValue}");
			if (string.IsNullOrEmpty(Filter.Value) || string.IsNullOrWhiteSpace(Filter.Value))
			{
				ItemsSubset = new ObservableCollection<OpenInEditorToggleViewModel>(_allItems);
			}
			else
			{
				ItemsSubset = new ObservableCollection<OpenInEditorToggleViewModel>(FilterItems(_allItems, filterValue));
			}
		}

		private List<OpenInEditorToggleViewModel> FilterItems(List<OpenInEditorToggleViewModel> items, string filterValue)
		{
			return items
				.Where(d => d.RelativePath.OriginalString.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0)
				.ToList();
		}

		public ReactiveProperty<string> Filter { get; } 

		private List<OpenInEditorToggleViewModel> _allItems = new List<OpenInEditorToggleViewModel>();

		private ObservableCollection<OpenInEditorToggleViewModel> _itemsSubset = new ObservableCollection<OpenInEditorToggleViewModel>();

		public ObservableCollection<OpenInEditorToggleViewModel> ItemsSubset
		{
			get => _itemsSubset;
			set => SetValue(ref _itemsSubset, value, nameof(ItemsSubset));
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
			var selectedItems = _allItems.Where(d => d.Included).Select(d => d.RelativePath.OriginalString);
			Configuration.OpenInEditorReferences = new ObservableCollection<string>(selectedItems);
			_whenConfirmed.OnNext(this.Configuration);
			_whenConfirmed.OnCompleted();
			_whenDiscarded.OnCompleted();
			return Task.CompletedTask;
		}

		private ICommand _undoChangesCommand;

		public ICommand UndoChangesCommand
		{
			get => _undoChangesCommand ?? (_undoChangesCommand = new TaskCommand(UndoChangesExecute));
			set => SetValue(ref _undoChangesCommand, value, nameof(UndoChangesCommand));
		}

		private Task UndoChangesExecute(object arg)
		{
			_whenDiscarded.OnNext(this.Configuration);
			_whenDiscarded.OnCompleted();
			_whenConfirmed.OnCompleted();
			return Task.CompletedTask;
		}

		private Task ToggleSolutionFileExecute(OpenInEditorToggleViewModel arg)
		{
			arg.Included = !arg.Included;
			return Task.CompletedTask;
		}
	}
}