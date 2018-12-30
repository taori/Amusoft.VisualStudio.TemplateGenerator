using System.Collections.Generic;

namespace Generator.Shared.Template
{
	public class Folder : TemplateHierarchyElement
	{
		public List<TemplateHierarchyElement> Items { get; set; } = new List<TemplateHierarchyElement>();
	}
}