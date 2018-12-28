using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
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
}