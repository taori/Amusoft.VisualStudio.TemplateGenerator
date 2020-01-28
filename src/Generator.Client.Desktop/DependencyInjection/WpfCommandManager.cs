using System;
using System.Windows.Input;
using Generator.Shared.DependencyInjection;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class WpfCommandManager : CommandManagerDelegate
	{
		/// <inheritdoc />
		protected override event EventHandler OnRequerySuggested
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}