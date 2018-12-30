using System;

namespace Generator.Shared.FileSystem
{
	public class DelegateFilter : FileWalkerFilter
	{
		private readonly Predicate<string> _filter;
		
		/// <exception cref="ArgumentNullException"></exception>
		/// <param name="filter"></param>
		public DelegateFilter(Predicate<string> filter)
		{
			_filter = filter ?? throw new ArgumentNullException(nameof(filter));
		}

		/// <inheritdoc />
		public override void Initialize(string root)
		{
		}

		/// <inheritdoc />
		public override bool IsValid(string file)
		{
			return _filter.Invoke(file);
		}
	}
}