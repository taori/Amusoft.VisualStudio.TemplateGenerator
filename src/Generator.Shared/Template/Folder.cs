using System.Collections.Generic;

namespace Generator.Shared.Template
{
	public class Folder : TemplateHierarchyElement
	{
		public List<TemplateHierarchyElement> Items { get; set; } = new List<TemplateHierarchyElement>();

		public bool IsRoot { get; set; }

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