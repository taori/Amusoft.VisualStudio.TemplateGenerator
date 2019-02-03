using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Generator.Shared.Transformation
{
	public class ProjectRewriteCache
	{
		public Dictionary<string, ProjectRewriteCacheEntry> Items { get; set; } = new Dictionary<string, ProjectRewriteCacheEntry>();

		public SolutionExplorer Explorer { get; }

		public string RootTemplatePath { get; }

		public Dictionary<string, List<string>> FilesByProjectPath { get; private set; }

		public ProjectRewriteCache(SolutionExplorer explorer, string rootTemplatePath)
		{
			Explorer = explorer;
			RootTemplatePath = rootTemplatePath;
		}

		public void Build()
		{
			FilesByProjectPath = Explorer
				.ProjectsLookup
				.Select(s => s.Key)
				.ToDictionary(
					projectFilePath => projectFilePath,
					projectFilePath =>
						Explorer.GetAdditiontalDocuments(projectFilePath)
							.Concat(Explorer.GetReferencedDocuments(projectFilePath)).ToList()
				);

			var uniqueProjects = Explorer
				.Solution
				.Projects
				.GroupBy(d => d.FilePath)
				.Select(s => s.FirstOrDefault());

			foreach (var project in uniqueProjects)
			{
				Items.Add(project.FilePath, BuildCacheEntry(project));
			}

			AdjustReferenceNames(Items.Values);
		}

		private void AdjustReferenceNames(IEnumerable<ProjectRewriteCacheEntry> items)
		{
			var options = Items.Values.Select(s => s.OriginalAssemblyName).ToList();
			foreach (var item in Items.Values)
			{
				var aligned = AssemblyNameAligner.Execute(item.OriginalAssemblyName, options);
				if (string.IsNullOrEmpty(aligned))
				{
					item.RootTemplateNamespace = $"$safeprojectname$";
					item.ProjectTemplateReference = $"$ext_safeprojectname${Path.GetExtension(item.ProjectFilePath)}";
					item.ProjectTemplateNamespace = $"$ext_safeprojectname$";
				}
				else
				{
					item.RootTemplateNamespace = $"$safeprojectname$.{aligned}";
					item.ProjectTemplateReference = $"$ext_safeprojectname$.{aligned}{Path.GetExtension(item.ProjectFilePath)}";
					item.ProjectTemplateNamespace = $"$ext_safeprojectname$.{aligned}";
				}
			}
		}

		private ProjectRewriteCacheEntry BuildCacheEntry(Project project)
		{
			var item = new ProjectRewriteCacheEntry();
			item.ProjectFilePath = project.FilePath;
			item.OriginalAssemblyName = project.AssemblyName;
			item.RelativeVsTemplatePath = BuildRelativeTemplatePath(project.FilePath, RootTemplatePath);
			return item;
		}

		private string BuildRelativeTemplatePath(string projectFilePath, string rootTemplatePath)
		{
			var destinationDirectory = Path.GetDirectoryName(projectFilePath);
			var projectVsTemplatePath = Path.Combine(destinationDirectory, "Generated.vstemplate");
			var destination = new Uri(projectVsTemplatePath, UriKind.Absolute);
			var rootUri = new Uri(rootTemplatePath, UriKind.Absolute);
			var relative = rootUri.MakeRelativeUri(destination);
			return relative.OriginalString.Replace('/', Path.DirectorySeparatorChar);
		}

		public IEnumerable<ProjectRewriteCacheEntry> GetSolutionProjectReferences()
		{
			/*
			 *	root.vstemplate links like
			 *	<ProjectTemplateLink ProjectName="$safeprojectname$.Interface" CopyParameters="true">
				  Interface\InterfaceTemplate.vstemplate
				</ProjectTemplateLink>
			 */

			return this.Items.Values.AsEnumerable();
		}
	}
}