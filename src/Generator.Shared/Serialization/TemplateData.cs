using System;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(TypeName = "TemplateData", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class TemplateData
	{
		/// <remarks/>
		public string Name { get; set; }

		/// <remarks/>
		public string Description { get; set; }

		/// <remarks/>
		public IconPackageReference Icon { get; set; } = new IconPackageReference(string.Empty);

		/// <remarks/>
		[XmlElement("ProjectType")]
		public string CodeLanguage { get; set; }

		/// <remarks/>
		public bool CreateNewFolder { get; set; }

		/// <remarks/>
		public bool CreateInPlace { get; set; }

		/// <remarks/>
		public string DefaultName { get; set; }

		/// <remarks/>
		public bool ProvideDefaultName { get; set; }

		public bool Hidden { get; set; }
	}
}