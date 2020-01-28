
using Generator.Shared.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Generator.Client.CommandLine.Dependencies
{
	public class ServiceLocatorInitializer
	{
		public static void Initialize()
		{
			ServiceLocator.Build(Configure);
		}

		private static void Configure(ServiceCollection services)
		{
			services.AddSingleton<IUIService, UiService>();
			services.AddSingleton<IFileDialogService, FileDialogService>();
		}
	}
}