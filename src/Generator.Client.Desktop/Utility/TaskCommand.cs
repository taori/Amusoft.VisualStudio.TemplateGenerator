using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Generator.Client.Desktop.Utility
{
	public class TaskCommand : TaskCommand<object>
	{
		/// <inheritdoc />
		public TaskCommand(Func<object, Task> execute) : base(execute)
		{
		}

		/// <inheritdoc />
		public TaskCommand(Func<object, Task> execute, Predicate<object> canExecute) : base(execute, canExecute)
		{
		}
	}

	public class TaskCommand<T> : ICommand
	{
		readonly Func<T, Task> _execute = null;
		readonly Predicate<T> _canExecute = null;

		public TaskCommand(Func<T, Task> execute)
			: this(execute, null)
		{
		}

		public TaskCommand(Func<T, Task> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public async void Execute(object parameter)
		{
			await _execute((T)parameter);
		}
	}
}