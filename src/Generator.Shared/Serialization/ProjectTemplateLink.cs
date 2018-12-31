using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "ProjectTemplateLink", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	[DebuggerDisplay("{ProjectName}")]
	public class ProjectTemplateLink : NestableContent
	{
		/// <inheritdoc />
		public ProjectTemplateLink(string projectName, string relativeTemplatePath, string originalNamespace, bool copyParameters = true)
		{
			ProjectName = projectName;
			CopyParameters = copyParameters;
			RelativeTemplatePath = relativeTemplatePath;
			OriginalNamespace = originalNamespace;
		}

		/// <inheritdoc />
		public ProjectTemplateLink()
		{
		}

		/**
		 *	<ProjectTemplateLink ProjectName="$safeprojectname$.Models" CopyParameters="true">
				Content\Company.Desktop.Models\MyTemplate.vstemplate
			</ProjectTemplateLink>
		 */
		[XmlAttribute]
		public string ProjectName { get; set; }

		[XmlAttribute]
		public bool CopyParameters { get; set; }

		[XmlText]
		public string RelativeTemplatePath { get; set; }

		[XmlIgnore]
		public string OriginalNamespace { get; set; }

		/// <inheritdoc />
		public override int HasPrimaryProject(string primaryNamespace)
		{
			if (string.Equals(OriginalNamespace, primaryNamespace, StringComparison.OrdinalIgnoreCase))
				return 1;

			return 0;
		}
	}
}