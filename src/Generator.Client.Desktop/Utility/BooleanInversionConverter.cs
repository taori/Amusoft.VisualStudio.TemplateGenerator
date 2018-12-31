using System;
using System.Globalization;
using System.Windows.Data;

namespace Generator.Client.Desktop.Utility
{
	public class BooleanInversionConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
				return !b;

			return value;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool b)
				return !b;

			return value;
		}
	}
}