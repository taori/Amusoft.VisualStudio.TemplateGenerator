using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Generator.Client.Desktop.Utility
{
	public class EnhancedBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = false;
			if (value is bool b)
			{
				flag = b;
			}
			if (!(parameter is bool p))
			{
				p = false;
			}

			if (p)
			{
				return flag ? Visibility.Collapsed : Visibility.Visible;
			}
			else
			{
				return flag ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			if (value is Visibility)
				return (object)((Visibility)value == Visibility.Visible);
			return (object)false;
		}
	}
}