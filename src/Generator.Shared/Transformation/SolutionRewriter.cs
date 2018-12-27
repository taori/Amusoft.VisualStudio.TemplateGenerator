using System.IO;
using System.Threading.Tasks;
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
			explorer.GetAdditiontalDocuments()

			/**
			 *	root.vstemplate links like
			 *	<ProjectTemplateLink ProjectName="$safeprojectname$.Interface" CopyParameters="true">
				  Interface\InterfaceTemplate.vstemplate
				</ProjectTemplateLink>
			 */
			// rewrite csproj references like <ProjectReference Include="..\$ext_safeprojectname$.Business\$ext_safeprojectname$.Business.csproj">

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