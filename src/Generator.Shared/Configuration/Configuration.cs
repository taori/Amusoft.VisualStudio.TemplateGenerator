using System;
using System.Collections.Generic;

namespace Generator.Shared.Configuration
{
	public class Configuration
	{
		public Guid Id { get; set; }

		public string SolutionPath { get; set; }

		public string ConfigurationName { get; set; }

		public IconPackageReference IconPackageReference { get; set; } = new IconPackageReference();

		public List<string> OpenInEditorReferences { get; set; } = new List<string>();

		public List<string> OutputFolders { get; set; } = new List<string>();

		public bool CreateInPlace { get; set; }

		public bool CreateNewFolder { get; set; }

		public string DefaultName { get; set; }

		public string Description { get; set; }

		public string ProjectType { get; set; }

		public bool ProvideDefaultName { get; set; }

		public string Name { get; set; }
	}
}