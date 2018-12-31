using System;
using System.Linq;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "Folder", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class Folder : NestableContent
	{
		/// <inheritdoc />
		public Folder(NestableContent[] children, string name, string targetFolderName)
		{
			Children = children;
			Name = name;
			TargetFolderName = targetFolderName;
		}

		/// <inheritdoc />
		public Folder()
		{
		}

		/// <summary>
		/// <para>supports:</para>
		/// <para>Folder</para>
		/// <para>ProjectItem</para>
		/// </summary>
		[XmlElement(typeof(ProjectItem))]
		[XmlElement(typeof(Folder))]
		public NestableContent[] Children { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string TargetFolderName { get; set; }

		/// <inheritdoc />
		public override int HasPrimaryProject(string primaryNamespace)
		{
			return Children.Max(d => d.HasPrimaryProject(primaryNamespace));
		}
	}
}