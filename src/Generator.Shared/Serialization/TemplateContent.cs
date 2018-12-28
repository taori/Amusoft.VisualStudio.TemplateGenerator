using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "TemplateContent", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class TemplateContent
	{
		/// <summary>
		/// <para>supports:</para>
		/// <para>ProjectTemplateLink</para>
		/// <para>SolutionFolder</para>
		/// <para>ProjectCollection</para>
		/// <para>Project</para>
		/// </summary>
		[XmlElement(typeof(ProjectTemplateLink))]
		[XmlElement(typeof(SolutionFolder))]
		[XmlElement(typeof(ProjectCollection))]
		[XmlElement(typeof(Project))]
		public NestableContent[] Children { get; set; }
	}
}