using System;
using System.Collections.Generic;
using System.Linq;
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
		public ProjectCollection(IEnumerable<NestableContent> children)
		{
			Children = new List<NestableContent>(children);
		}

		/// <inheritdoc />
		public ProjectCollection()
		{
		}

		[XmlElement(typeof(ProjectTemplateLink))]
		[XmlElement(typeof(SolutionFolder))]
		public List<NestableContent> Children { get; set; }

		/// <inheritdoc />
		public override int HasPrimaryProject(string primaryNamespace)
		{
			return Children.Max(d => d.HasPrimaryProject(primaryNamespace));
		}
	}
}