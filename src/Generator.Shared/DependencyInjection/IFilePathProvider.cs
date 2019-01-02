namespace Generator.Shared.DependencyInjection
{
	public interface IFilePathProvider
	{
		bool RequestFolderName(out string folder, string description, string suggestedFolder);
		bool RequestFileName(out string filePath, string description, string suggestedFolder);
	}
}