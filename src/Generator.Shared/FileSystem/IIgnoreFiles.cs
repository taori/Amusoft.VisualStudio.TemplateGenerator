using System;
using System.Collections.Generic;

namespace Generator.Shared.FileSystem
{
	public interface IIgnoreFiles
	{
		HashSet<Uri> Ignored { get; }
	}
}