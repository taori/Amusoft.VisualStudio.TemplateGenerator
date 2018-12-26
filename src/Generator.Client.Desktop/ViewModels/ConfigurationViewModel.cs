﻿using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared;
using Generator.Shared.Configuration;

namespace Generator.Client.Desktop.ViewModels
{
	public class IconViewModel : ViewModelBase
	{
		private string _package;

		public string Package
		{
			get => _package;
			set => SetValue(ref _package, value, nameof(Package));
		}

		private ushort _id;

		public ushort Id
		{
			get => _id;
			set => SetValue(ref _id, value, nameof(Id));
		}
	}

	public class ConfigurationViewModel : ViewModelBase
	{
		public Configuration Model { get; }

		public ConfigurationViewModel(Configuration model)
		{
			SelectSolutionCommand = new TaskCommand(SelectSolutionExecute);
			Model = model;
			UpdateFromModel();
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

		private IconViewModel _icon = new IconViewModel();

		public IconViewModel Icon
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

		private string _projectType;

		public string ProjectType
		{
			get => _projectType;
			set => SetValue(ref _projectType, value, nameof(ProjectType));
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
			Model.ProjectType = ProjectType;
			Model.ProvideDefaultName = ProvideDefaultName;
			Model.Name = Name;
		}

		public void UpdateFromModel()
		{
			ConfigurationName = Model.ConfigurationName;
			Id = Model.Id;
			Icon = new IconViewModel()
			{
				Id = Model.IconPackageReference?.Id ?? 0,
				Package = Model.IconPackageReference?.Package
			};
			SolutionPath = Model.SolutionPath;
			CreateInPlace = Model.CreateInPlace;
			CreateNewFolder = Model.CreateNewFolder;
			DefaultName = Model.DefaultName;
			Description = Model.Description;
			ProjectType = Model.ProjectType;
			ProvideDefaultName = Model.ProvideDefaultName;
			Name = Model.Name;
		}

		public bool CanBuild()
		{
			return File.Exists(SolutionPath);
		}
	}
}