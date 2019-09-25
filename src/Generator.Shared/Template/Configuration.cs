using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Generator.Shared.Serialization;

namespace Generator.Shared.Template
{
	public class Configuration : ICloneable
	{
		public static readonly Regex FileCopyBlacklistRegex = new Regex(@"\*\.[\w]+(?=[;,]?)", RegexOptions.Compiled);

		public IEnumerable<string> GetFileCopyBlackListElements()
		{
			foreach (Match match in FileCopyBlacklistRegex.Matches(FileCopyBlacklist))
			{
				foreach (Group matchGroup in match.Groups)
				{
					yield return matchGroup.Value;
				}
			}
		}

		[XmlAttribute]
		public Guid Id { get; set; }

		[XmlAttribute]
		public string SolutionPath { get; set; }

		[XmlAttribute]
		public string ConfigurationName { get; set; }

		public IconPackageReference Icon { get; set; } = new IconPackageReference(string.Empty);

		[XmlArrayItem(typeof(string))]
		public List<string> OpenInEditorReferences { get; set; } = new List<string>();

		[XmlArrayItem(typeof(string))]
		public List<string> OutputFolders { get; set; } = new List<string>();

		[XmlElement]
		public Folder TemplateHierarchy { get; set; } = new Folder() { IsRoot = true, Name = "Solution root"};

		[XmlAttribute]
		public bool CreateInPlace { get; set; }

		[XmlAttribute]
		public bool CreateNewFolder { get; set; }

		[XmlAttribute]
		public bool ZipContents { get; set; }

		[XmlAttribute]
		public string ArtifactName { get; set; }

		[XmlAttribute]
		public string FileCopyBlacklist { get; set; }

		[XmlAttribute]
		public string DefaultName { get; set; }

		[XmlAttribute]
		public string Description { get; set; }

		[XmlAttribute]
		public string CodeLanguage { get; set; }

		[XmlAttribute]
		public bool ProvideDefaultName { get; set; }

		[XmlAttribute]
		public string PrimaryProject { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public bool HideSubProjects { get; set; }

		/// <inheritdoc />
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}