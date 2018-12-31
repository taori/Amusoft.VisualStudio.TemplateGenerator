using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Generator.Client.Desktop.Utility
{
	public static class ItemsControlHelper
	{
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty = DependencyProperty.RegisterAttached(
			"IsSynchronizedWithCurrentItem", typeof(bool), typeof(ItemsControlHelper), new PropertyMetadata(default(bool), IsSynchronizedWithCurrentItemChanged));

		private static void IsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is ItemsControl itemsControl))
				return;
			if (!(e.NewValue is bool value))
				return;

			if (value)
			{
				var view = CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
//				view.CurrentChanged += (sender, args) => 
//				view.CurrentChanged += (sender, args) => itemsControl.prop
			}
			else
			{
				var view = CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
			}
		}

		public static void SetIsSynchronizedWithCurrentItem(ItemsControl element, bool value)
		{
			element.SetValue(IsSynchronizedWithCurrentItemProperty, value);
		}

		public static bool GetIsSynchronizedWithCurrentItem(ItemsControl element)
		{
			return (bool) element.GetValue(IsSynchronizedWithCurrentItemProperty);
		}
	}
}