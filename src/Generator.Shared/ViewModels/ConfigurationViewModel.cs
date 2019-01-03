using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.DependencyInjection;
using Generator.Shared.Serialization;
using Generator.Shared.Template;
using Generator.Shared.Transformation;
using NLog;
using Folder = Generator.Shared.Template.Folder;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Generator.Shared.ViewModels
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

			ProjectNamespaces = new ObservableCollection<string>(referencedNamespaces);

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
			var viewModel = new ManageOpenInEditorReferencesViewModel(this);
			if(ServiceLocator.TryGetService(out IViewModelPresenter presenter))
				presenter.Present(viewModel);

			await Task.Delay(500);
			await viewModel.LoadAsync();
		}

		private Task RemoveOpenInEditorReferenceExecute(string arg)
		{
			if (string.IsNullOrEmpty(arg))
				return Task.CompletedTask;

			if (MessageBox.Show($"Remove for sure?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
				OpenInEditorReferences.Remove(arg);

			return Task.CompletedTask;
		}
		
		private Task AddOutputFolderExecute(object arg)
		{
			if(!ServiceLocator.TryGetService(out IViewModelPresenter presenter))
				throw new Exception($"{nameof(IViewModelPresenter)} missing.");

			var viewModel = new SelectOutputFolderViewModel();
			viewModel.WhenFolderSelected.Subscribe(folder => OutputFolders.Add(folder));
			presenter.Present(viewModel);

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

		private Subject<ConfigurationViewModel> _whenSaved = new Subject<ConfigurationViewModel>();
		public IObservable<ConfigurationViewModel> WhenSaved => _whenSaved;

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

		public ObservableCollection<string> ProjectNamespaces
		{
			get => _startupProjectOptions ?? (_startupProjectOptions = new ObservableCollection<string>());
			set => SetValue(ref _startupProjectOptions, value, nameof(ProjectNamespaces));
		}

		private IconPackageViewModel _icon;

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

		private bool _zipContents;

		public bool ZipContents
		{
			get => _zipContents;
			set => SetValue(ref _zipContents, value, nameof(ZipContents));
		}

		private string _artifactName;

		public string ArtifactName
		{
			get => _artifactName;
			set => SetValue(ref _artifactName, value, nameof(ArtifactName));
		}

		private string _fileCopyBlacklist;

		public string FileCopyBlacklist
		{
			get => _fileCopyBlacklist;
			set => SetValue(ref _fileCopyBlacklist, value, nameof(FileCopyBlacklist));
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
		
		private ICommand _commitChangesCommand;

		public ICommand CommitChangesCommand
		{
			get => _commitChangesCommand ?? (_commitChangesCommand = new TaskCommand(CommitChangesExecute));
			set => SetValue(ref _commitChangesCommand, value, nameof(CommitChangesCommand));
		}

		private async Task CommitChangesExecute(object arg)
		{
			await SaveAsync();
			_whenConfirm.OnNext(this);
			_whenConfirm.OnCompleted();
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
			_whenConfirm.OnCompleted();
			_whenDiscard.OnCompleted();
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

		private ICommand _removeOpenInEditorReferenceCommand;

		public ICommand RemoveOpenInEditorReferenceCommand
		{
			get => _removeOpenInEditorReferenceCommand;
			set => SetValue(ref _removeOpenInEditorReferenceCommand, value, nameof(RemoveOpenInEditorReferenceCommand));
		}

		private ICommand _removeHierarchyFolderCommand;

		public ICommand RemoveHierarchyFolderCommand
		{
			get => _removeHierarchyFolderCommand ?? (_removeHierarchyFolderCommand = new TaskCommand<TemplateHierarchyViewModel>(RemoveHierarchyFolderExecute));
			set => SetValue(ref _removeHierarchyFolderCommand, value, nameof(RemoveHierarchyFolderCommand));
		}

		private Task RemoveHierarchyFolderExecute(TemplateHierarchyViewModel item)
		{
			RemoveHierarchyElement(TemplateHierarchy, item);
			return Task.CompletedTask;
		}

		private void RemoveHierarchyElement(FolderViewModel folderViewModel, TemplateHierarchyViewModel item)
		{
			var children = folderViewModel.Items.ToArray();
			foreach (var child in children)
			{
				if (object.ReferenceEquals(child, item))
				{
					folderViewModel.Items.Remove(item);
					return;
				}

				if(child is FolderViewModel subFolder)
					RemoveHierarchyElement(subFolder, item);
			}
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

		private FolderViewModel _templateHierarchy;

		public FolderViewModel TemplateHierarchy
		{
			get => _templateHierarchy;
			set => SetValue(ref _templateHierarchy, value, nameof(TemplateHierarchy));
		}

		public void UpdateModel()
		{
			if (Model.IconPackageReference == null)
				Model.IconPackageReference = new VisualStudioIcon();
			Icon.UpdateModel();
			Model.IconPackageReference = Icon.Model;

			Model.ConfigurationName = ConfigurationName;
			Model.Id = Id;
			Model.IconPackageReference = Icon.Model;
			Model.SolutionPath = SolutionPath;
			Model.CreateInPlace = CreateInPlace;
			Model.CreateNewFolder = CreateNewFolder;
			Model.ZipContents = ZipContents;
			Model.ArtifactName = ArtifactName;
			Model.FileCopyBlacklist = FileCopyBlacklist;
			Model.DefaultName = DefaultName;
			Model.Description = Description;
			Model.CodeLanguage = CodeLanguage;
			Model.ProvideDefaultName = ProvideDefaultName;
			Model.PrimaryProject = PrimaryProject;
			Model.Name = Name;
			Model.OutputFolders = new List<string>(OutputFolders);
			Model.OpenInEditorReferences = new List<string>(OpenInEditorReferences);

			TemplateHierarchy.UpdateModel();
			Model.TemplateHierarchy = TemplateHierarchy.Model as Folder;
		}

		public void UpdateFromModel()
		{
			ConfigurationName = Model.ConfigurationName;
			Id = Model.Id;
			Icon = IconPackageViewModel.Create(Model.IconPackageReference);
			SolutionPath = Model.SolutionPath;
			CreateInPlace = Model.CreateInPlace;
			CreateNewFolder = Model.CreateNewFolder;
			ZipContents = Model.ZipContents;
			ArtifactName = Model.ArtifactName;
			FileCopyBlacklist = Model.FileCopyBlacklist;
			DefaultName = Model.DefaultName;
			Description = Model.Description;
			CodeLanguage = Model.CodeLanguage;
			ProvideDefaultName = Model.ProvideDefaultName;
			Name = Model.Name;
			PrimaryProject = Model.PrimaryProject;
			OutputFolders = new ObservableCollection<string>(Model.OutputFolders);
			OpenInEditorReferences = new ObservableCollection<string>(Model.OpenInEditorReferences);
			TemplateHierarchy = new FolderViewModel(Model.TemplateHierarchy);
		}

		public bool CanBuild()
		{
			_errors.Clear();

			if (!ValidateSolutionFile())
				return false;
			if (!ValidateOutputFolders())
				return false;
			if (!ValidateArtifactName())
				return false;
			if (!ValidateFileCopyBlacklist())
				return false;

			return true;
		}

		private bool ValidateFileCopyBlacklist()
		{
			if (
				(!string.IsNullOrEmpty(FileCopyBlacklist)
				|| !string.IsNullOrWhiteSpace(FileCopyBlacklist))
				&& !Configuration.FileCopyBlacklistRegex.IsMatch(FileCopyBlacklist))
			{
				AddError(nameof(FileCopyBlacklist), $"Invalid values for FileCopyBlacklist {FileCopyBlacklist}.");
				return false;
			}

			return true;
		}

		private static readonly Regex ArtifactNameRegex = new Regex(@"[\w_\d-\.]+");
		private bool ValidateArtifactName()
		{
			if (
				string.IsNullOrEmpty(ArtifactName)
				|| string.IsNullOrWhiteSpace(ArtifactName)
				|| !ArtifactNameRegex.IsMatch(ArtifactName))
			{
				AddError(nameof(ArtifactName), $"Invalid artifact name.");
				return false;
			}

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
			
			Log.Debug($"Validation error {property}: {message}.");
			errors.Add(message);
			OnPropertyChanged(nameof(ValidationErrors));
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
		}

		private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

		/// <inheritdoc />
		public IEnumerable GetErrors(string propertyName)
		{
			if(!string.IsNullOrEmpty(propertyName) && _errors.TryGetValue(propertyName, out var errors))
				return errors;

			return _errors.TryGetValue(string.Empty, out var globalErrors) ? (IEnumerable<string>)globalErrors : Array.Empty<string>();
		}

		public IEnumerable<string> ValidationErrors
		{
			get { return _errors.Values.SelectMany(s => s); }
		}

		/// <inheritdoc />
		public bool HasErrors => _errors.Values.Any(d => d.Count > 0);

		/// <inheritdoc />
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public async Task<bool> SaveAsync()
		{
			Log.Info($"Saving configuration {Id}.");
			UpdateModel();

			if (await ConfigurationManager.UpdateConfigurationAsync(Model))
			{
				Log.Info($"Update successful.");
				_whenSaved.OnNext(this);
				_whenSaved.OnCompleted();
				return true;
			}

			return false;
		}

		public void NotifySaved()
		{
			_whenSaved.OnNext(this);
		}
	}
}