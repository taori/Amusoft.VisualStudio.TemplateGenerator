using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Generator.Shared.Resources;
using Generator.Shared.Template;
using Generator.Shared.Utilities;
using NLog;

namespace Generator.Shared.Transformation
{
	public class SolutionRewriter
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(SolutionRewriter));

		public RewriteContext Context { get; }

		public string Folder { get; }

		public SolutionRewriter(RewriteContext context, string folder)
		{
			Context = context;
			Folder = folder;
		}

		public async Task<bool> RewriteAsync()
		{
			Log.Debug($"Rewriting folder \"{Folder}\".");
			if (!MoveContentFiles(out var contentFolder))
				return false;

			var solutionFile = FileHelper.FindSolutionAsync(contentFolder);
			if (!File.Exists(solutionFile))
			{
				Log.Error($"Solution file at \"{contentFolder}\" not found.");
				return false;
			}

			var explorer = await SolutionExplorer.CreateAsync(solutionFile, Context.Progress, Context.CancellationToken);
			var projectFileList = explorer
				.ProjectsLookup
				.Select(s => s.Key)
				.ToDictionary(
					projectFilePath => projectFilePath, 
					projectFilePath =>
						explorer.GetAdditiontalDocuments(projectFilePath)
							.Concat(explorer.GetReferencedDocuments(projectFilePath)).ToList()
					);


			var rootTemplatePath = Path.Combine(Folder, "root.vstemplate");
			try
			{
				CreateRootVsTemplate(Context, explorer, rootTemplatePath);
				if(!FileHelper.RemoveXmlMarker(rootTemplatePath))
					throw new Exception($"Failed to remove xml tag from template file.");
			}
			catch (Exception e)
			{
				Context.Progress.Report($"Failed to create {rootTemplatePath}.");
				await Task.Delay(3000);
				Log.Error(e);
				return false;
			}


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

			Log.Info($"Rewriting complete.");
			return true;
		}

		private void CreateRootVsTemplate(RewriteContext context, SolutionExplorer explorer, string templatePath)
		{
			var serializer = new XmlSerializer(typeof(VsTemplate));
			var template = new VsTemplate();
			template.Type = Constants.VsTemplate.ProjectTypes.ProjectGroup;
			template.TemplateData.CreateInPlace = context.Configuration.CreateInPlace;
			template.TemplateData.CreateNewFolder = context.Configuration.CreateNewFolder;
			template.TemplateData.DefaultName = context.Configuration.DefaultName;
			template.TemplateData.Description = context.Configuration.Description;
			template.TemplateData.Name = context.Configuration.Name;
			template.TemplateData.ProvideDefaultName = context.Configuration.ProvideDefaultName;
			template.TemplateData.CodeLanguage = context.Configuration.CodeLanguage;
			template.TemplateData.Icon.Id = context.Configuration.IconPackageReference.Id;
			template.TemplateData.Icon.Package = context.Configuration.IconPackageReference.Package;

			template.TemplateContent = BuildRootTemplateContent(context, explorer, templatePath);

			using (var fileStream = new FileStream(templatePath, FileMode.Create))
			{
				using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
				{
					serializer.Serialize(streamWriter, template);
				}
			}

			/**
			 *	root.vstemplate links like
			 *	<ProjectTemplateLink ProjectName="$safeprojectname$.Interface" CopyParameters="true">
				  Interface\InterfaceTemplate.vstemplate
				</ProjectTemplateLink>
			 */
			// rewrite csproj references like <ProjectReference Include="..\$ext_safeprojectname$.Business\$ext_safeprojectname$.Business.csproj">
		}

		private TemplateContent BuildRootTemplateContent(RewriteContext context, SolutionExplorer explorer, string templatePath)
		{
			throw new NotImplementedException();
		}

		private bool MoveContentFiles(out string contentFolder)
		{
			// bypass folders which are named "Content too
			var tmpContent = Path.Combine(Folder, "___contents");
			if (!FileHelper.MoveContents(Folder, tmpContent))
			{
				Log.Error($"Failed to copy contents from \"{Folder}\" to \"{tmpContent}\".");
				contentFolder = null;
				return false;
			}

			contentFolder = Path.Combine(Folder, "Content");
			Directory.Move(tmpContent, contentFolder);
			return true;
		}
	}
}