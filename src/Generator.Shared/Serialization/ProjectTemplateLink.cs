using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "ProjectTemplateLink", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class ProjectTemplateLink : NestableContent
	{
		/// <inheritdoc />
		public ProjectTemplateLink(string projectName, string relativeTemplatePath, bool copyParameters = true)
		{
			ProjectName = projectName;
			CopyParameters = copyParameters;
			RelativeTemplatePath = relativeTemplatePath;
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
	}
}