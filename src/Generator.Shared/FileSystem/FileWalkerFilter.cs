namespace Generator.Shared.FileSystem
{
	public abstract class FileWalkerFilter
	{
		public abstract void Initialize(string root);

		public abstract bool IsValid(string file);
	}
}