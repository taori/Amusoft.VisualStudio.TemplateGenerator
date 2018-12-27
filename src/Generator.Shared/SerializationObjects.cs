using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Generator.Shared
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	[XmlRoot(ElementName = "VSTemplate", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005", IsNullable = false)]

	public class VsTemplate
	{
		/// <remarks/>
		public TemplateData TemplateData { get; set; } = new TemplateData();

		/// <remarks/>
		public TemplateContent TemplateContent { get; set; } = new TemplateContent();

		/// <summary>
		/// e.g. 3.0.0
		/// </summary>
		[XmlAttribute("Version")]
		public string Version { get; set; } = "3.0.0";

		/// <summary>
		/// e.g. ProjectGroup / Project
		/// </summary>
		[XmlAttribute("Type")]
		public string Type { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "TemplateData", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class TemplateData
	{
		/// <remarks/>
		public string Name { get; set; }

		/// <remarks/>
		public string Description { get; set; }

		/// <remarks/>
		public IconPackageReference Icon { get; set; } = new IconPackageReference();

		/// <remarks/>
		[XmlElement("ProjectType")]
		public string CodeLanguage { get; set; }

		/// <remarks/>
		public bool CreateNewFolder { get; set; }

		/// <remarks/>
		public bool CreateInPlace { get; set; }

		/// <remarks/>
		public string DefaultName { get; set; }

		/// <remarks/>
		public bool ProvideDefaultName { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class IconPackageReference
	{
		/// <remarks/>
		[XmlAttribute]
		public string Package { get; set; }

		/// <remarks/>
		[XmlAttribute("ID")]
		public ushort Id { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "TemplateContent", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class TemplateContent
	{
		[XmlElement(typeof(ProjectTemplateLink))]
		[XmlElement(typeof(SolutionFolder))]
		[XmlElement(typeof(ProjectCollection))]
		public NestableContent[] Children { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "ProjectCollection", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class ProjectCollection : NestableContent
	{
		/// <inheritdoc />
		public ProjectCollection(NestableContent[] children)
		{
			Children = children;
		}

		/// <inheritdoc />
		public ProjectCollection()
		{
		}

		[XmlElement(typeof(ProjectTemplateLink))]
		[XmlElement(typeof(SolutionFolder))]
		public NestableContent[] Children { get; set; }
	}

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

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "SolutionFolder", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class SolutionFolder : NestableContent
	{
		/// <inheritdoc />
		public SolutionFolder(string name, NestableContent[] children)
		{
			Children = children;
			Name = name;
		}

		/// <inheritdoc />
		public SolutionFolder()
		{
		}

		[XmlElement(typeof(ProjectTemplateLink), ElementName = "ProjectTemplateLink")]
		public NestableContent[] Children { get; set; }

		/// <remarks/>
		[XmlAttribute]
		public string Name { get; set; }
	}

	public abstract class NestableContent
	{

	}
}
