using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Client.Desktop.Utility;

namespace Generator.Shared.ViewModels
{
	public class SelectOutputFolderViewModel : ScreenViewModel
	{
		public SelectOutputFolderViewModel()
		{
			Title = "Select an output folder";
			SelectManualCommand = new TaskCommand(SelectManualExecute);
			SelectWithPresetCommand = new TaskCommand<string>(SelectWithPresetExecute);
			PresetOptions = GetOptions();
		}

		private ObservableCollection<string> GetOptions()
		{
			var directories = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
			return new ObservableCollection<string>(FilterExpand(directories));
		}

		private readonly Regex FolderPattern = new Regex(@"visual studio [\d]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private IEnumerable<string> FilterExpand(string[] directories)
		{
			var baseDirectories = directories.Where(d => FolderPattern.IsMatch(d));
			foreach (var directory in baseDirectories)
			{
				var assumedPath = Path.Combine(directory, "Templates","ProjectTemplates");
				if (Directory.Exists(assumedPath))
					yield return assumedPath;
			}
		}

		private Task SelectWithPresetExecute(string arg)
		{
			return GetFolderBrowserResult(arg);
		}

		private Task SelectManualExecute(object arg)
		{
			return GetFolderBrowserResult();
		}

		private Task GetFolderBrowserResult(string presetFolder = null)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				dialog.Description = "Select an output folder";
				dialog.ShowNewFolderButton = true;
				if (presetFolder != null)
					dialog.SelectedPath = presetFolder;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					_whenFolderSelected.OnNext(dialog.SelectedPath);
				}
				else
				{
					_whenAborted.OnNext(null);
				}

				_whenAborted.OnCompleted();
				_whenFolderSelected.OnCompleted();
				return Task.CompletedTask;
			}
		}


		private ObservableCollection<string> _presetOptions;

		public ObservableCollection<string> PresetOptions
		{
			get => _presetOptions;
			set => SetValue(ref _presetOptions, value, nameof(PresetOptions));
		}

		private ICommand _selectManualCommand;

		public ICommand SelectManualCommand
		{
			get => _selectManualCommand;
			set => SetValue(ref _selectManualCommand, value, nameof(SelectManualCommand));
		}

		private ICommand _selectWithPresetCommand;

		public ICommand SelectWithPresetCommand
		{
			get => _selectWithPresetCommand;
			set => SetValue(ref _selectWithPresetCommand, value, nameof(SelectWithPresetCommand));
		}

		private Subject<string> _whenFolderSelected = new Subject<string>();
		public IObservable<string> WhenFolderSelected => _whenFolderSelected;

		private Subject<object> _whenAborted = new Subject<object>();
		public IObservable<object> WhenAborted => _whenAborted;
	}
}