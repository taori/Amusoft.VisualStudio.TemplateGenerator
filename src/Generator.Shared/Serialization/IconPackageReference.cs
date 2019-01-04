using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public abstract class IconPackageReference
	{
	}

	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class VisualStudioIcon : IconPackageReference
	{
		/// <inheritdoc />
		public VisualStudioIcon(string package, ushort id)
		{
			Package = package;
			Id = id;
		}

		/// <inheritdoc />
		public VisualStudioIcon()
		{
		}

		/// <remarks/>
		[XmlAttribute]
		public string Package { get; set; }

		/// <remarks/>
		[XmlAttribute("ID")]
		public ushort Id { get; set; }
	}
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class AbsolutePathIcon : IconPackageReference
	{
		/// <inheritdoc />
		public AbsolutePathIcon(string path)
		{
			Path = path;
		}

		/// <inheritdoc />
		public AbsolutePathIcon()
		{
		}

		/// <remarks/>
		[XmlText]
		public string Path { get; set; }
	}
}