using System;
using System.Reflection;
using System.Windows;
using Generator.Client.Desktop.DependencyInjection;
using Generator.Client.Desktop.Utility;
using Generator.Client.Desktop.Views;
using Generator.Shared.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using NLog.Config;
using Path = System.IO.Path;

namespace Generator.Client.Desktop
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <inheritdoc />
		protected override void OnStartup(StartupEventArgs e)
		{
			CommandManagerDelegate.Instance = new WpfCommandManager();
			ServiceLocatorInitializer.Initialize();
			base.OnStartup(e);
		}
	}
}
