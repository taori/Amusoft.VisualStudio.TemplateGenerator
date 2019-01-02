using System.Collections.Generic;
using System.Xml.Serialization;

namespace Generator.Shared.Template
{
	public class Folder : TemplateHierarchyElement
	{
		[XmlElement(typeof(Folder), ElementName = "Folder")]
		[XmlElement(typeof(Project), ElementName = "Project")]
		public List<TemplateHierarchyElement> Items { get; set; } = new List<TemplateHierarchyElement>();

		[XmlAttribute]
		public bool IsRoot { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		public IEnumerable<Project> GetProjectsRecursive()
		{
			foreach (var child in Items)
			{
				if (child is Folder folder)
				{
					foreach (var subProject in folder.GetProjectsRecursive())
					{
						yield return subProject;
					}
				}

				if (child is Project project)
					yield return project;
			}
		}
	}
}