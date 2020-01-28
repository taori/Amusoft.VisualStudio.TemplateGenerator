using System;

namespace Generator.Shared.DependencyInjection
{
	public class CommandManagerDelegate
	{
		public static CommandManagerDelegate Instance = new CommandManagerDelegate();

		protected virtual event EventHandler OnRequerySuggested;

		public static event EventHandler RequerySuggested
		{
			add { Instance.OnRequerySuggested += value; }
			remove { Instance.OnRequerySuggested += value; }
		}
	}
}