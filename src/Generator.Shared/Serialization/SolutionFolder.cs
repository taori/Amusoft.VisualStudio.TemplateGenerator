using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
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
}