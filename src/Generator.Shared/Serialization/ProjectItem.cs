using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "ProjectItem", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class ProjectItem : NestableContent
	{
		/// <inheritdoc />
		public ProjectItem(string targetFileName, string content, bool replaceParameters = true)
		{
			TargetFileName = targetFileName;
			Content = content;
			ReplaceParameters = replaceParameters;
		}

		/// <inheritdoc />
		public ProjectItem()
		{
		}
		// <ProjectItem ReplaceParameters = "true" TargetFileName="RegionNames.cs">RegionNames.cs</ProjectItem>

		[XmlAttribute]
		public string TargetFileName { get; set; }

		[XmlText]
		public string Content { get; set; }

		[XmlAttribute]
		public bool ReplaceParameters { get; set; }
	}
}