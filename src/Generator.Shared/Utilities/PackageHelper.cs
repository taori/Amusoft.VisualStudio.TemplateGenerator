using System;
using Generator.Shared.Serialization;
using Generator.Shared.Template;

namespace Generator.Shared.Utilities
{
	internal static class PackageHelper
	{
		public static IconPackageReference GetConfigurationIcon(Configuration configuration)
		{
			if (configuration.Icon.Id == 0 || String.IsNullOrEmpty(configuration.Icon.Package))
				return new IconPackageReference("{b3bae735-386c-4030-8329-ef48eeda4036}", 4602);

			return configuration.Icon;
		}
	}
}