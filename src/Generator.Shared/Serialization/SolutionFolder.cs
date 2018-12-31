using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "SolutionFolder", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	[DebuggerDisplay("{Name} ({Children.Count})")]
	public class SolutionFolder : NestableContent
	{
		/// <inheritdoc />
		public SolutionFolder(string name, IEnumerable<NestableContent> children)
		{
			Children = children.ToList();
			Name = name;
		}

		/// <inheritdoc />
		public SolutionFolder()
		{
		}

		[XmlElement(typeof(SolutionFolder), ElementName = "SolutionFolder")]
		[XmlElement(typeof(ProjectTemplateLink), ElementName = "ProjectTemplateLink")]
		public List<NestableContent> Children { get; set; }

		/// <remarks/>
		[XmlAttribute]
		public string Name { get; set; }

		/// <inheritdoc />
		public override int HasPrimaryProject(string primaryNamespace)
		{
			return Children.Max(d => d.HasPrimaryProject(primaryNamespace));
		}
	}
}