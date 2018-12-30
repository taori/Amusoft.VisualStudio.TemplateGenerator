namespace Generator.Shared.FileSystem
{
	public class NoFilter : FileListerOptions
	{
		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return true;
		}
	}
}