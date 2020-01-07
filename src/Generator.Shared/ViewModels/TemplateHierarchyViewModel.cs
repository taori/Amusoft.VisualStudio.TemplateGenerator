using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;
using Generator.Shared.Template;

namespace Generator.Shared.ViewModels
{
	public abstract class TemplateHierarchyViewModel : ViewModelBase
	{
		/// <inheritdoc />
		protected TemplateHierarchyViewModel(TemplateHierarchyElement model)
		{
			Model = model;
		}

		public abstract void UpdateModel();

		public TemplateHierarchyElement Model { get; }

		public static TemplateHierarchyViewModel Create(TemplateHierarchyElement element)
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

		private bool _isRoot;

		public bool IsRoot
		{
			get => _isRoot;
			set => SetValue(ref _isRoot, value, nameof(IsRoot));
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
			this.FolderName = model.Name;
			this.IsRoot = model.IsRoot;
			this.Items = new ObservableCollection<TemplateHierarchyViewModel>(model.Items.Select(TemplateHierarchyViewModel.Create));
		}

		/// <inheritdoc />
		public override void UpdateModel()
		{
			if (Model is Folder folder)
			{
				folder.Name = this.FolderName;
				folder.IsRoot = this.IsRoot;
				foreach (var item in Items)
				{
					item.UpdateModel();
				}

				folder.Items = new List<TemplateHierarchyElement>(this.Items.Select(s => s.Model));
			}
		}

		public IEnumerable<FolderViewModel> GetChildren(bool recursive, bool self)
		{
			if (self)
				yield return this;

			foreach (var child in Items)
			{
				if (child is FolderViewModel folder)
				{
					yield return folder;
					if (recursive)
					{
						foreach (var subFolder in folder.GetChildren(recursive, false))
						{
							yield return subFolder;
						}
					}
				}
			}
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
			Namespace = model.Namespace;
		}

		/// <inheritdoc />
		public override void UpdateModel()
		{
			if (Model is Project project)
				project.Namespace = Namespace;
		}
	}
}