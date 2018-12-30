using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.Template;
using JetBrains.Annotations;

namespace Generator.Client.Desktop.ViewModels
{
	public abstract class TemplateHierarchyViewModel : ViewModelBase
	{
		/// <inheritdoc />
		public TemplateHierarchyViewModel(TemplateHierarchyElement model)
		{
			Model = model;
		}

		public TemplateHierarchyElement Model { get; }

		public static TemplateHierarchyViewModel Create([NotNull] TemplateHierarchyElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			if (element is Project project)
				return new ProjectViewModel(project);
			if (element is Folder folder)
				return new FolderViewModel(folder);

			throw new Exception($"Unexpected datatype {element.GetType().FullName}.");
		}
	}

	public class FolderViewModel : TemplateHierarchyViewModel
	{
		private string _folderName;

		public string FolderName
		{
			get => _folderName;
			set => SetValue(ref _folderName, value, nameof(FolderName));
		}

		private ObservableCollection<TemplateHierarchyViewModel> _items;

		public ObservableCollection<TemplateHierarchyViewModel> Items
		{
			get => _items ?? (_items = new ObservableCollection<TemplateHierarchyViewModel>());
			set => SetValue(ref _items, value, nameof(Items));
		}

		private ICommand _newFolderCommand;

		public ICommand NewFolderCommand
		{
			get => _newFolderCommand ?? (_newFolderCommand = new TaskCommand(NewFolderExecute));
			set => SetValue(ref _newFolderCommand, value, nameof(NewFolderCommand));
		}

		private Task NewFolderExecute(object arg)
		{
			var folder = new Folder();
			if (Model is Folder folderModel)
				folderModel.Items.Add(folder);

			Items.Add(new FolderViewModel(folder) { FolderName = "New Folder" });
			return Task.CompletedTask;
		}

		private ICommand _newFileCommand;

		public ICommand NewFileCommand
		{
			get => _newFileCommand ?? (_newFileCommand = new TaskCommand(NewFileExecute));
			set => SetValue(ref _newFileCommand, value, nameof(NewFileCommand));
		}

		private Task NewFileExecute(object arg)
		{
			var folder = new Project();
			if (Model is Folder folderModel)
				folderModel.Items.Add(folder);

			Items.Add(new ProjectViewModel(folder));
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public FolderViewModel(Folder model) : base(model)
		{
		}
	}

	public class ProjectViewModel : TemplateHierarchyViewModel
	{
		private string _namespace;

		public string Namespace
		{
			get => _namespace;
			set => SetValue(ref _namespace, value, nameof(Namespace));
		}

		/// <inheritdoc />
		public ProjectViewModel(Project model) : base(model)
		{
		}
	}
}