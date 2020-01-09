using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
#pragma warning disable 67

namespace Generator.Client.Desktop.Utility
{
	public class CloseOnEscapeBehavior : Behavior<Window>, ICommand
	{
		/// <inheritdoc />
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.InputBindings.Add(new KeyBinding(this, Key.Escape, ModifierKeys.None));
		}

		/// <inheritdoc />
		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}

		/// <inheritdoc />
		void ICommand.Execute(object parameter)
		{
			AssociatedObject.Close();
		}

		/// <inheritdoc />
		private event EventHandler CanExecuteChanged;

		/// <inheritdoc />
		event EventHandler ICommand.CanExecuteChanged
		{
			add { this.CanExecuteChanged += value; }
			remove { this.CanExecuteChanged -= value; }
		}
	}
}