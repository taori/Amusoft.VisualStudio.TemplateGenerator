using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Client.Desktop.Views;
using Generator.Shared;
using Generator.Shared.Serialization;
using Generator.Shared.Template;
using Generator.Shared.Transformation;
using NLog;
using Folder = Generator.Shared.Template.Folder;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Generator.Client.Desktop.ViewModels
{
	public class ConfigurationViewModel : ViewModelBase, INotifyDataErrorInfo
	{
		public Configuration Model { get; }

		private static readonly ILogger Log = LogManager.GetLogger(nameof(ConfigurationViewModel));

		public ConfigurationViewModel(Configuration model)
		{
			SelectSolutionCommand = new TaskCommand(SelectSolutionExecute);
			AddOutputFolderCommand = new TaskCommand(AddOutputFolderExecute);
			RemoveOutputFolderCommand = new TaskCommand(RemoveOutputFolderExecute);
			RemoveOpenInEditorReferenceCommand = new TaskCommand<string>(RemoveOpenInEditorReferenceExecute);
			ManageOpenInEditorReferencesCommand = new TaskCommand(ManageOpenInEditorReferencesExecute);
			Model = model;
			UpdateFromModel();
		}

		/// <inheritdoc />
		protected override async void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
			if (propertyName == nameof(SolutionPath))
				await LoadStartupProjectOptionsAsync();
		}

		private Task LoadStartupProjectOptionsAsync()
		{
			var solutionPath = Model?.SolutionPath;
			if (string.IsNullOrEmpty(solutionPath) || !File.Exists(solutionPath))
				return Task.CompletedTask;

			var referencedNamespaces = GetAssemblyNamesOfSolution(solutionPath)
				.ToArray();

			StartupProjectOptions = new ObservableCollection<string>(referencedNamespaces);

			return Task.CompletedTask;
		}

		private static IOrderedEnumerable<string> GetAssemblyNamesOfSolution(string solutionPath)
		{
			return SolutionExplorer
				.GetReferencedProjectPaths(solutionPath)
				.Select(SolutionExplorer.GetAssemblyName)
				.OrderBy(d => d);
		}

		private Task RemoveOutputFolderExecute(object arg)
		{
			if (arg is string folder)
			{
				OutputFolders.Remove(folder);
			} 

			return Task.CompletedTask;
		}

		private async Task ManageOpenInEditorReferencesExecute(object arg)
		{
			var window = new ManageOpenInEditorReferencesWindow();
			var viewModel = new ManageOpenInEditorReferencesViewModel(this);
			viewModel.WhenConfirmed.Subscribe(model => window.Close());
			viewModel.WhenDiscarded.Subscribe(model => window.Close());
			window.DataContext = viewModel;
			window.Loaded += WindowOnLoaded;
			window.Show();
		}

		private async void WindowOnLoaded(object sender, RoutedEventArgs e)
		{
			if (sender is Window window)
			{
				window.Loaded -= WindowOnLoaded;
				await Task.Delay(500);
				if (window.DataContext is ManageOpenInEditorReferencesViewModel vm)
					await vm.LoadAsync();
			}
		}

		private Task RemoveOpenInEditorReferenceExecute(string arg)
		{
			if (string.IsNullOrEmpty(arg))
				return Task.CompletedTask;

			if (MessageBox.Show($"Remove for sure?", "Question") == DialogResult.OK)
				OpenInEditorReferences.Remove(arg);

			return Task.CompletedTask;
		}
		
		private Task AddOutputFolderExecute(object arg)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				dialog.Description = "Select an output folder";
				dialog.ShowNewFolderButton = true;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					OutputFolders.Add(dialog.SelectedPath);
				}
			}

			return Task.CompletedTask;
		}

		private Task SelectSolutionExecute(object arg)
		{
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "VisualStudio solution|*.sln";
				dialog.Multiselect = false;
				dialog.ReadOnlyChecked = true;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					SolutionPath = dialog.FileName;
				}
			}

			return Task.CompletedTask;
		}

		private Subject<ConfigurationViewModel> _whenConfirm = new Subject<ConfigurationViewModel>();
		public IObservable<ConfigurationViewModel> WhenConfirm => _whenConfirm;

		private Subject<ConfigurationViewModel> _whenDiscard = new Subject<ConfigurationViewModel>();
		public IObservable<ConfigurationViewModel> WhenDiscard => _whenDiscard;

		private string _solutionPath;

		public string SolutionPath
		{
			get => _solutionPath;
			set => SetValue(ref _solutionPath, value, nameof(SolutionPath));
		}

		private ObservableCollection<string> _outputFolders;

		public ObservableCollection<string> OutputFolders
		{
			get => _outputFolders ?? (_outputFolders = new ObservableCollection<string>());
			set => SetValue(ref _outputFolders, value, nameof(OutputFolders));
		}

		private ObservableCollection<string> _startupProjectOptions;

		public ObservableCollection<string> StartupProjectOptions
		{
			get => _startupProjectOptions ?? (_startupProjectOptions = new ObservableCollection<string>());
			set => SetValue(ref _startupProjectOptions, value, nameof(StartupProjectOptions));
		}

		private IconPackageViewModel _icon = new IconPackageViewModel();

		public IconPackageViewModel Icon
		{
			get => _icon;
			set => SetValue(ref _icon, value, nameof(Icon));
		}

		private string _configurationName;

		public string ConfigurationName
		{
			get => _configurationName;
			set => SetValue(ref _configurationName, value, nameof(ConfigurationName));
		}

		private Guid _id;

		public Guid Id
		{
			get => _id;
			set => SetValue(ref _id, value, nameof(Id));
		}

		private string _name;

		public string Name
		{
			get => _name;
			set => SetValue(ref _name, value, nameof(Name));
		}

		private string _description;

		public string Description
		{
			get => _description;
			set => SetValue(ref _description, value, nameof(Description));
		}

		private string _codeLanguage;

		public string CodeLanguage
		{
			get => _codeLanguage;
			set => SetValue(ref _codeLanguage, value, nameof(CodeLanguage));
		}

		private bool _createNewFolder;

		public bool CreateNewFolder
		{
			get => _createNewFolder;
			set => SetValue(ref _createNewFolder, value, nameof(CreateNewFolder));
		}

		private bool _createInPlace;

		public bool CreateInPlace
		{
			get => _createInPlace;
			set => SetValue(ref _createInPlace, value, nameof(CreateInPlace));
		}

		private string _defaultName;

		public string DefaultName
		{
			get => _defaultName;
			set => SetValue(ref _defaultName, value, nameof(DefaultName));
		}

		private bool _provideDefaultName;

		public bool ProvideDefaultName
		{
			get => _provideDefaultName;
			set => SetValue(ref _provideDefaultName, value, nameof(ProvideDefaultName));
		}

		private string _primaryProject;

		public string PrimaryProject
		{
			get => _primaryProject;
			set => SetValue(ref _primaryProject, value, nameof(PrimaryProject));
		}

		private ICommand _newFolderCommand;

		public ICommand NewRootFolderCommand
		{
			get => _newFolderCommand ?? (_newFolderCommand = new TaskCommand(NewRootFolderExecute));
			set => SetValue(ref _newFolderCommand, value, nameof(NewRootFolderCommand));
		}

		private Task NewRootFolderExecute(object arg)
		{
			TemplateHierarchy.Add(new FolderViewModel(new Folder()));
			return Task.CompletedTask;
		}

		private ICommand _commitChangesCommand;

		public ICommand CommitChangesCommand
		{
			get => _commitChangesCommand ?? (_commitChangesCommand = new TaskCommand(CommitChangesExecute));
			set => SetValue(ref _commitChangesCommand, value, nameof(CommitChangesCommand));
		}

		private Task CommitChangesExecute(object arg)
		{
			_whenConfirm.OnNext(this);
			return Task.CompletedTask;
		}

		private ICommand _undoChangesCommand;

		public ICommand UndoChangesCommand
		{
			get => _undoChangesCommand ?? (_commitChangesCommand = new TaskCommand(UndoChangesExecute));
			set => SetValue(ref _undoChangesCommand, value, nameof(UndoChangesCommand));
		}

		private Task UndoChangesExecute(object arg)
		{
			_whenDiscard.OnNext(this);
			return Task.CompletedTask;
		}

		private ICommand _selectSolutionCommand;

		public ICommand SelectSolutionCommand
		{
			get => _selectSolutionCommand;
			set => SetValue(ref _selectSolutionCommand, value, nameof(SelectSolutionCommand));
		}

		private ICommand _addOutputFolderCommand;

		public ICommand AddOutputFolderCommand
		{
			get => _addOutputFolderCommand;
			set => SetValue(ref _addOutputFolderCommand, value, nameof(AddOutputFolderCommand));
		}

		private ICommand _removeOutputFolderCommand;

		public ICommand RemoveOutputFolderCommand
		{
			get => _removeOutputFolderCommand;
			set => SetValue(ref _removeOutputFolderCommand, value, nameof(RemoveOutputFolderCommand));
		}

		private ICommand _removeRootFolderCommand;

		public ICommand RemoveRootFolderCommand
		{
			get => _removeRootFolderCommand ?? (_removeRootFolderCommand = new TaskCommand<FolderViewModel>(RemoveRootFolderExecute));
			set => SetValue(ref _removeRootFolderCommand, value, nameof(RemoveRootFolderCommand));
		}

		private Task RemoveRootFolderExecute(FolderViewModel arg)
		{
			TemplateHierarchy.Remove(arg);
			return Task.CompletedTask;
		}

		private ICommand _removeOpenInEditorReferenceCommand;

		public ICommand RemoveOpenInEditorReferenceCommand
		{
			get => _removeOpenInEditorReferenceCommand;
			set => SetValue(ref _removeOpenInEditorReferenceCommand, value, nameof(RemoveOpenInEditorReferenceCommand));
		}

		private ICommand _manageOpenInEditorReferencesCommand;

		public ICommand ManageOpenInEditorReferencesCommand
		{
			get => _manageOpenInEditorReferencesCommand;
			set => SetValue(ref _manageOpenInEditorReferencesCommand, value, nameof(ManageOpenInEditorReferencesCommand));
		}
		
		private ObservableCollection<string> _openInEditorReferences;

		public ObservableCollection<string> OpenInEditorReferences
		{
			get => _openInEditorReferences ?? (_openInEditorReferences = new ObservableCollection<string>());
			set => SetValue(ref _openInEditorReferences, value, nameof(OpenInEditorReferences));
		}

		private ObservableCollection<TemplateHierarchyViewModel> _templateHierarchy;

		public ObservableCollection<TemplateHierarchyViewModel> TemplateHierarchy
		{
			get => _templateHierarchy ?? (_templateHierarchy = new ObservableCollection<TemplateHierarchyViewModel>());
			set => SetValue(ref _templateHierarchy, value, nameof(TemplateHierarchy));
		}

		public void UpdateModel()
		{
			if (Model.IconPackageReference == null)
				Model.IconPackageReference = new IconPackageReference();

			Model.ConfigurationName = ConfigurationName;
			Model.Id = Id;
			Model.IconPackageReference.Id = Icon.Id;
			Model.IconPackageReference.Package = Icon.Package;
			Model.SolutionPath = SolutionPath;
			Model.CreateInPlace = CreateInPlace;
			Model.CreateNewFolder = CreateNewFolder;
			Model.DefaultName = DefaultName;
			Model.Description = Description;
			Model.CodeLanguage = CodeLanguage;
			Model.ProvideDefaultName = ProvideDefaultName;
			Model.PrimaryProject = PrimaryProject;
			Model.Name = Name;
			Model.OutputFolders = new List<string>(OutputFolders);
			Model.OpenInEditorReferences = new List<string>(OpenInEditorReferences);
			Model.TemplateHierarchy = new List<TemplateHierarchyElement>(TemplateHierarchy.Select(s => s.Model));
		}

		public void UpdateFromModel()
		{
			ConfigurationName = Model.ConfigurationName;
			Id = Model.Id;
			Icon = new IconPackageViewModel()
			{
				Id = Model.IconPackageReference?.Id ?? 0,
				Package = Model.IconPackageReference?.Package
			};
			SolutionPath = Model.SolutionPath;
			CreateInPlace = Model.CreateInPlace;
			CreateNewFolder = Model.CreateNewFolder;
			DefaultName = Model.DefaultName;
			Description = Model.Description;
			CodeLanguage = Model.CodeLanguage;
			ProvideDefaultName = Model.ProvideDefaultName;
			Name = Model.Name;
			PrimaryProject = Model.PrimaryProject;
			OutputFolders = new ObservableCollection<string>(Model.OutputFolders);
			OpenInEditorReferences = new ObservableCollection<string>(Model.OpenInEditorReferences);
			TemplateHierarchy = new ObservableCollection<TemplateHierarchyViewModel>(Model.TemplateHierarchy.Select(TemplateHierarchyViewModel.Create));
		}

		public bool CanBuild()
		{
			_errors.Clear();

			if (!ValidateSolutionFile())
				return false;
			if (!ValidateOutputFolders())
				return false;

			return true;
		}

		private bool ValidateSolutionFile()
		{
			if (string.IsNullOrEmpty(SolutionPath) || string.IsNullOrWhiteSpace(SolutionPath))
			{
				AddError(nameof(SolutionPath), $"No solution file specified.");
				return false;
			}
			if (!File.Exists(SolutionPath))
			{
				AddError(nameof(SolutionPath), $"Solution file does not exist.");
				return false;
			}

			return true;
		}

		private bool ValidateOutputFolders()
		{
			if (OutputFolders == null || OutputFolders.Count == 0)
			{
				AddError(nameof(OutputFolders), $"No output folder available.");
				return false;
			}
			if (OutputFolders.All(d => !Directory.Exists(d)))
			{
				AddError(nameof(OutputFolders), $"None of the output folders exist.");
				return false;
			}

			return true;
		}

		private void AddError(string property, string message)
		{
			if (!_errors.TryGetValue(property, out var errors))
			{
				errors = new List<string>();
				_errors.Add(property, errors);
			}
			
			Log.Info($"Validation error {property}: {message}.");
			errors.Add(message);
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
		}

		private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

		/// <inheritdoc />
		public IEnumerable GetErrors(string propertyName)
		{
			if (propertyName != null && _errors.TryGetValue(propertyName, out var errors))
				return errors;

			return Enumerable.Empty<string>();
		}

		/// <inheritdoc />
		public bool HasErrors => _errors.Values.Any(d => d.Count > 0);

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
	}
}