using System;
using System.Reflection;
using System.Windows;
using Generator.Client.Desktop.DependencyInjection;
using Generator.Client.Desktop.Utility;
using Generator.Client.Desktop.Views;
using Generator.Shared.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
			ServiceLocatorInitializer.Initialize();
			base.OnStartup(e);
		}
	}

	public class ServiceLocatorInitializer
	{
		public static void Initialize()
		{
			ServiceLocator.Build(Configure);
		}

		private static void Configure(ServiceCollection services)
		{
			var viewModelPresenter = new StaticViewModelPresenter();
			viewModelPresenter.Build();
			services.AddSingleton<IViewModelPresenter>(viewModelPresenter);
			services.AddSingleton<IUIService, UIService>();
			services.AddSingleton<IFilePathProvider, FilePathProvider>();
		}
	}
}
