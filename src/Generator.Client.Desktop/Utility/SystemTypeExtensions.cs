using System;
using System.Windows.Markup;

namespace Generator.Client.Desktop.Utility
{
	public class SystemTypeExtension : MarkupExtension
	{
		private object _value;

		public int Int
		{
			set => _value = value;
		}

		public double Double
		{
			set => _value = value;
		}

		public float Float
		{
			set => _value = value;
		}

		public bool Bool
		{
			set => _value = value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _value;
		}
	}
}