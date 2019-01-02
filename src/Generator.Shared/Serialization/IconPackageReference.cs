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
		/// <remarks/>
		[XmlAttribute]
		public string Package { get; set; }

		/// <remarks/>
		[XmlAttribute("ID")]
		public ushort Id { get; set; }
	}
}