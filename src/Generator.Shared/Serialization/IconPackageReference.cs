using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Generator.Shared.Serialization
{
	/// <remarks/>
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlRoot("Icon", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
	public class IconPackageReference : IXmlSerializable
	{
		public IconPackageReference(string package, ushort id)
		{
			Package = package;
			Id = id;
		}

		/// <inheritdoc />
		public IconPackageReference(string path)
		{
			Path = path;
		}

		public IconPackageReference()
		{
		}

		public string Path { get; set; }

		public string Package { get; set; }

		public ushort Id { get; set; }

		/// <inheritdoc />
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <inheritdoc />
		public void ReadXml(XmlReader reader)
		{
			var content = reader.ReadString();
			if (string.IsNullOrEmpty(content))
			{
				Package = reader.GetAttribute("Package");
				var id = reader.GetAttribute("ID");
				if (ushort.TryParse(id, out var typedId))
					Id = typedId;
			}
			else
			{
				Path = content;
			}
		}

		/// <inheritdoc />
		public void WriteXml(XmlWriter writer)
		{
			if (IsVisualStudioIcon())
			{
				WriteVsIcon(writer);
			}
			else
			{
				WritePathIcon(writer);
			}
		}

		private void WritePathIcon(XmlWriter writer)
		{
			writer.WriteStartElement("Icon");
			writer.WriteString(Path);
			writer.WriteEndElement();
		}

		private void WriteVsIcon(XmlWriter writer)
		{
			writer.WriteStartElement("Icon");
			writer.WriteAttributeString("Package", Package);
			writer.WriteAttributeString("ID", Package);
			writer.WriteEndElement();
		}

		public bool IsVisualStudioIcon()
		{
			return Id > 0 || !string.IsNullOrEmpty(Package);
		}

		public bool IsPathIcon()
		{
			return !string.IsNullOrEmpty(Path);
		}
	}

//	/// <remarks/>
//	[Serializable]
//	[System.ComponentModel.DesignerCategory("code")]
////	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
//	[XmlRoot("Icon", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
//	public class VisualStudioIcon : IconPackageReference//, IXmlSerializable
//	{
//		/// <inheritdoc />
//		public VisualStudioIcon(string package, ushort id)
//		{
//			Package = package;
//			Id = id;
//		}
//
//		/// <inheritdoc />
//		public VisualStudioIcon()
//		{
//		}
//
//		/// <remarks/>
//		[XmlAttribute]
//		public string Package { get; set; }
//
//		/// <remarks/>
//		[XmlAttribute("ID")]
//		public ushort Id { get; set; }
//
////		/// <inheritdoc />
////		public XmlSchema GetSchema()
////		{
////			throw new NotImplementedException();
////		}
////
////		/// <inheritdoc />
////		public void ReadXml(XmlReader reader)
////		{
////			throw new NotImplementedException();
////		}
////
////		/// <inheritdoc />
////		public void WriteXml(XmlWriter writer)
////		{
////			throw new NotImplementedException();
////		}
//	}
//	/// <remarks/>
//	[Serializable]
//	[System.ComponentModel.DesignerCategory("code")]
//	[XmlRoot("Icon", Namespace = "http://schemas.microsoft.com/developer/vstemplate/2005")]
//	public class AbsolutePathIcon : IconPackageReference
//	{
//		/// <inheritdoc />
//		public AbsolutePathIcon(string path)
//		{
//			Path = path;
//		}
//
//		/// <inheritdoc />
//		public AbsolutePathIcon()
//		{
//		}
//
//		/// <remarks/>
//		[XmlText]
//		public string Path { get; set; }
//	}
}