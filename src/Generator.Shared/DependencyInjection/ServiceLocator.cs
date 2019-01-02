using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Generator.Shared.DependencyInjection
{
	public static class ServiceLocator
	{
		public static void Build(Action<ServiceCollection> configure)
		{
			var collection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
			configure(collection);
			Services = collection.BuildServiceProvider();
		}

		public static ServiceProvider Services { get; private set; }

		public static bool TryGetService<TService>(out TService instance)
		{
			if(Services == null)
				throw new Exception($"{nameof(Build)} needs to be called before consuming services.");

			instance = Services.GetService<TService>();
			return instance != null;
		}
	}
}
