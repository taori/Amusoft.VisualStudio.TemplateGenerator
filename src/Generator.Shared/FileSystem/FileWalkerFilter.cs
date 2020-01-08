using NLog;

namespace Generator.Shared.FileSystem
{
	public abstract class FileWalkerFilter
	{
		protected static readonly ILogger Log = LogManager.GetLogger(nameof(FileWalkerFilter));

		public abstract void Initialize(string root);

		public abstract bool IsValid(string file);

		public static FileWalkerFilter[] NoArtifacts()
		{
			return new FileWalkerFilter[]
			{
				new BuildArtifactsFilter(), 
				new HiddenFolderFilter(), 
				new NugetFolderFilter(), 
				new TemplateFileFilter(),
			};
		}
	}
}