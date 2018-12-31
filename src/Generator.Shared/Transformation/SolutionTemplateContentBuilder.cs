using System;
using System.Collections.Generic;
using System.Linq;
using Generator.Shared.Serialization;
using Folder = Generator.Shared.Template.Folder;
using Project = Generator.Shared.Template.Project;

namespace Generator.Shared.Transformation
{
	public class SolutionTemplateContentBuilder
	{
		public ProjectRewriteCache Cache { get; }

		public SolutionRewriteContext Context { get; }

		public SolutionTemplateContentBuilder(ProjectRewriteCache cache, SolutionRewriteContext context)
		{
			Cache = cache;
			Context = context;
		}

		public ProjectCollection BuildProjectCollection()
		{
			var sortedReferences = GetSortedSolutionReferences(Cache).ToArray();
			var referencesByNamespace = sortedReferences.ToDictionary(d => d.OriginalAssemblyName, d => d);

			var root = Context.Configuration.TemplateHierarchy;
			var referencedProjects = new HashSet<string>(root.GetProjectsRecursive().Select(s => s.Namespace));
			var orphans = sortedReferences.Where(d => !referencedProjects.Contains(d.OriginalAssemblyName));
			var projectCollection = AddRecursive(root, referencesByNamespace)
				.Concat(orphans.Select(ToProjectTemplateLink))
				.OrderByDescending(d => d.HasPrimaryProject(Context.Configuration.PrimaryProject))
				.ToList();

			return new ProjectCollection(projectCollection);
		}

		private IEnumerable<NestableContent> AddRecursive(Folder folder, Dictionary<string, ProjectRewriteCacheEntry> referencesByNamespace)
		{
			if (folder == null)
				throw new ArgumentNullException(nameof(folder));

			foreach (var folderItem in folder.Items)
			{
				if (folderItem is Project project)
				{
					if (referencesByNamespace.TryGetValue(project.Namespace, out var cacheEntry))
						yield return ToProjectTemplateLink(cacheEntry);
				}

				if (folderItem is Folder subFolder)
				{
					if (subFolder.Items.Count > 0)
					{
						var children = AddRecursive(subFolder, referencesByNamespace).ToArray();
						yield return new SolutionFolder(subFolder.Name, children);
					}
				}
			}
		}

		private static ProjectTemplateLink ToProjectTemplateLink(ProjectRewriteCacheEntry reference)
		{
			return new ProjectTemplateLink(reference.RootTemplateNamespace, reference.RelativeVsTemplatePath, reference.OriginalAssemblyName);
		}

		private IEnumerable<ProjectRewriteCacheEntry> GetSortedSolutionReferences(ProjectRewriteCache cache)
		{
			var references = cache.GetSolutionProjectReferences().ToArray();
			var pp = Context.Configuration.PrimaryProject;

			if (!string.IsNullOrEmpty(pp) && references.Any(d => string.Equals(pp, d.OriginalAssemblyName, StringComparison.OrdinalIgnoreCase)))
			{
				return references
					.Where(d => string.Equals(pp, d.OriginalAssemblyName, StringComparison.OrdinalIgnoreCase))
					.Concat(references.Where(d => d.OriginalAssemblyName != pp)
						.OrderBy(d => d.OriginalAssemblyName));
			}
			else
			{
				return references.OrderBy(d => d.OriginalAssemblyName);
			}
		}
	}
}