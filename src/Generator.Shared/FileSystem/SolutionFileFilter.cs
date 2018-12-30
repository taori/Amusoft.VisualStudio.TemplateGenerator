using System.IO;

namespace Generator.Shared.FileSystem
{
	public class SolutionFileFilter : FileWalkerFilter
	{
		/// <inheritdoc />
		public override void Initialize(string root)
		{
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return Path.GetExtension(file) != ".sln";
		}
	}
}