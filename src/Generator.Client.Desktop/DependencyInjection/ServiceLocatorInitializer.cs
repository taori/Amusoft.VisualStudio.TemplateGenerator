using Generator.Shared.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Generator.Client.Desktop.DependencyInjection
{
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
			services.AddSingleton<IFileDialogService, FileDialogService>();
		}
	}
}