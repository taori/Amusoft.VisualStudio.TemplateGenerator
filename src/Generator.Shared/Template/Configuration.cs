using System;
using System.Collections.Generic;
using Generator.Shared.Serialization;

namespace Generator.Shared.Template
{
	public class Configuration : ICloneable
	{
		public Guid Id { get; set; }

		public string SolutionPath { get; set; }

		public string ConfigurationName { get; set; }

		public IconPackageReference IconPackageReference { get; set; } = new IconPackageReference();

		public List<string> OpenInEditorReferences { get; set; } = new List<string>();

		public List<string> OutputFolders { get; set; } = new List<string>();

		public Folder TemplateHierarchy { get; set; } = new Folder() { IsRoot = true, Name = "Solution root"};

		public bool CreateInPlace { get; set; }

		public bool CreateNewFolder { get; set; }

		public bool ZipContents { get; set; }

		public string ArtifactName { get; set; }

		public string DefaultName { get; set; }

		public string Description { get; set; }

		public string CodeLanguage { get; set; }

		public bool ProvideDefaultName { get; set; }

		public string PrimaryProject { get; set; }

		public string Name { get; set; }

		/// <inheritdoc />
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}