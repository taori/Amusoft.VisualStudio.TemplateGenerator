using System.IO;

namespace Generator.Shared.Transformation
{
	public class ProjectRewriteCacheEntry
	{
		/// <summary>
		/// e.g. Interface\InterfaceTemplate.vstemplate
		/// </summary>
		public string RelativeVsTemplatePath { get; set; }

		/// <summary>
		/// e.g. D:\path\to\file\Interface\Generated.vstemplate
		/// </summary>
		public string AbsoluteVsTemplatePath => Path.Combine(Path.GetDirectoryName(ProjectFilePath), Path.GetFileName(RelativeVsTemplatePath));

		/// <summary>
		/// e.g $safeprojectname$.Interface
		/// </summary>
		public string RootTemplateNamespace { get; set; }

		/// <summary>
		/// e.g. D:\path\to\file.csproj
		/// </summary>
		public string ProjectFilePath { get; set; }

		/// <summary>
		/// e.g. Some.Namespace.Interface
		/// </summary>
		public string OriginalAssemblyName { get; set; }

		/// <summary>
		/// e.g. $ext_safeprojectname$.Interface
		/// </summary>
		public string ProjectTemplateNamespace { get; set; }

		/// <summary>
		/// e.g. $ext_safeprojectname$.Interface.csproj
		/// </summary>
		public string ProjectTemplateReference { get; set; }
	}
}