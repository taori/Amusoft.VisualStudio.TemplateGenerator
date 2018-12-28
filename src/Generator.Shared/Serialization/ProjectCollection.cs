using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
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
}