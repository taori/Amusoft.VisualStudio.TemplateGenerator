using System;
using System.Linq;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "Project", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class Project : NestableContent
	{
		/// <inheritdoc />
		public Project(NestableContent[] children, string targetFileName, string file, bool replaceParameters = true)
		{
			Children = children;
			TargetFileName = targetFileName;
			File = file;
			ReplaceParameters = replaceParameters;
		}

		/// <inheritdoc />
		public Project()
		{
		}

		/// <summary>
		/// <para>supports:</para>
		/// <para>Folder</para>
		/// <para>ProjectItem</para>
		/// </summary>
		[XmlElement(typeof(Folder))]
		[XmlElement(typeof(ProjectItem))]
		public NestableContent[] Children { get; set; }

		[XmlAttribute]
		public string TargetFileName { get; set; }

		[XmlAttribute]
		public string File { get; set; }

		[XmlAttribute]
		public bool ReplaceParameters { get; set; }

		/// <inheritdoc />
		public override int HasPrimaryProject(string primaryNamespace)
		{
			return Children.Max(d => d.HasPrimaryProject(primaryNamespace));
		}
	}
}