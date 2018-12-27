using System;
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
		/// <remarks/>
		[XmlArrayItem(IsNullable = false)]
		public SolutionFolder[] ProjectCollection { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "SolutionFolder", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class SolutionFolder
	{
		/// <remarks/>
		[XmlElement("ProjectTemplateLink")]
		public ProjectLink[] ProjectTemplateLink { get; set; }

		/// <remarks/>
		[XmlAttribute]
		public string Name { get; set; }
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class ProjectLink
	{
		/// <remarks/>
		[XmlAttribute]
		public string ProjectName { get; set; }

		/// <remarks/>
		[XmlAttribute]
		public bool CopyParameters { get; set; }

		/// <remarks/>
		[XmlText]
		public string Value { get; set; }
	}
}
