using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Generator.Shared.Resources;
using Generator.Shared.Serialization;
using Generator.Shared.Utilities;
using NLog;

namespace Generator.Shared.Transformation
{
	public class ProjectRewriter : RewriterBase
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(ProjectRewriter));

		public ProjectRewriteContext Context { get; }

		public ProjectRewriter(ProjectRewriteContext context)
		{
			// rewrite csproj references like <ProjectReference Include="..\$ext_safeprojectname$.Business\$ext_safeprojectname$.Business.csproj">
			Context = context;
		}

		public async Task ExecuteAsync()
		{
			var projectInfos = Context.Cache.Items.Values.FirstOrDefault(d => d.ProjectFilePath == Context.ProjectPath);
			if (projectInfos == null)
			{
				Log.Error($"Failed to obtain {nameof(projectInfos)} for \"{Context.ProjectPath}\".");
				throw new Exception($"Failed to obtain {nameof(projectInfos)} for \"{Context.ProjectPath}\".");
			}

			var template = CreateTemplate(projectInfos);
			SaveTemplate(template, projectInfos.AbsoluteVsTemplatePath);

			var files = new HashSet<string>(
				Context.Explorer.GetAdditiontalDocuments(Context.ProjectPath)
					.Concat(Context.Explorer.GetReferencedDocuments(Context.ProjectPath))
					.Concat(Context.Explorer.GetAllProjectFiles())
			);

			await RewriteFilesAsync(files);
		}

		private async Task RewriteFilesAsync(HashSet<string> files)
		{
			var replacements = GetRewriteRules();
			
			foreach (var file in files)
			{
				if (Context.CancellationToken.IsCancellationRequested)
					return;

				await RewriteAsync(file, replacements);
			}
		}

		private Dictionary<string, string> GetRewriteRules()
		{
			var rules = new Dictionary<string, string>();
			foreach (var cacheItem in Context.Cache.Items.Values)
			{
				// Company.Desktop.Framework.Controls -> $ext_safeprojectname$.Framework.Controls
				rules.Add(cacheItem.OriginalAssemblyName, cacheItem.ProjectTemplateNamespace);
			}

			return rules;
		}

		private async Task RewriteAsync(string file, Dictionary<string, string> replacements)
		{
			string replaced;
			using (var fileStream = new FileStream(file, FileMode.Open))
			{
				using (var reader = new StreamReader(fileStream, Encoding.UTF8))
				{
					replaced = StringHelper.MultiReplace(await reader.ReadToEndAsync(), replacements);
				}
			}
			using (var fileStream = new FileStream(file, FileMode.Create))
			{
				using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
				{
					await writer.WriteAsync(replaced);
				}
			}
		}

		private VsTemplate CreateTemplate(ProjectRewriteCacheEntry projectInfos)
		{
			var template = new VsTemplate();
			template.Type = Constants.VsTemplate.ProjectTypes.Project;
			template.TemplateData = new TemplateData();
			template.TemplateData.Icon = null;
			template.TemplateData.Name = projectInfos.ProjectTemplateNamespace;
			template.TemplateData.DefaultName = projectInfos.ProjectTemplateNamespace;
			template.TemplateData.CodeLanguage = Context.Configuration.CodeLanguage;
			template.TemplateContent = CreateContent(projectInfos);
			return template;
		}

		private TemplateContent CreateContent(ProjectRewriteCacheEntry projectInfos)
		{
			var content = new TemplateContent();
			var project = new Project();
			project.ReplaceParameters = true;
			project.File = Path.GetFileName(projectInfos.ProjectFilePath);
			project.TargetFileName = projectInfos.ProjectTemplateReference;
			AddFiles(project);
			content.Children = new NestableContent[]
			{
				project
			};
			return content;
		}

		private void AddFiles(Project project)
		{
			/**
			 * project.vstemplate TemplateContent like
			 *
				<Project TargetFileName="$ext_safeprojectname$.ViewModels.csproj" File="Company.Desktop.ViewModels.csproj" ReplaceParameters="true">
				  <Folder Name="Common" TargetFolderName="Common">
				    <ProjectItem ReplaceParameters="true" TargetFileName="RegionNames.cs">RegionNames.cs</ProjectItem>
				  </Folder>
				  <Folder Name="Controls" TargetFolderName="Controls">
				    <ProjectItem ReplaceParameters="true" TargetFileName="SampleDataOverviewViewModel.cs">SampleDataOverviewViewModel.cs</ProjectItem>
				    <ProjectItem ReplaceParameters="true" TargetFileName="SampleDataViewModel.cs">SampleDataViewModel.cs</ProjectItem>
				  </Folder>
				  <Folder Name="Windows" TargetFolderName="Windows">
				    <ProjectItem ReplaceParameters="true" TargetFileName="MainViewModel.cs">MainViewModel.cs</ProjectItem>
				    <ProjectItem ReplaceParameters="true" TargetFileName="SecondaryWindowViewModel.cs">SecondaryWindowViewModel.cs</ProjectItem>
				  </Folder>
				</Project>
			 */

			var allowedDocuments = Enumerable.Concat(
				Context.Explorer.GetAdditiontalDocuments(Context.ProjectPath),
				Context.Explorer.GetReferencedDocuments(Context.ProjectPath)
			);
			var whiteList = new HashSet<string>(allowedDocuments);

			project.Children = GetProjectMembers(Path.GetDirectoryName(Context.ProjectPath), whiteList).ToArray();
		}

		private static IEnumerable<NestableContent> GetProjectMembers(string directoryName, HashSet<string> whiteList)
		{
			var dirInfo = new DirectoryInfo(directoryName);
			foreach (var subInfo in dirInfo.GetDirectories())
			{
				yield return new Folder(GetProjectMembers(subInfo.FullName, whiteList).ToArray(), subInfo.Name, subInfo.Name);
			}

			foreach (var fileInfo in dirInfo.GetFiles())
			{
				if (whiteList.Contains(fileInfo.FullName))
					yield return new ProjectItem(fileInfo.Name, fileInfo.Name);
			}
		}
	}
}