using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Generator.Client.Desktop.Utility
{
	public class SelectAllOnFocusBehavior : Behavior<TextBox>
	{
		/// <inheritdoc />
		protected override void OnAttached()
		{
			this.AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;
			base.OnAttached();
		}

		private void AssociatedObjectOnGotFocus(object sender, RoutedEventArgs e)
		{
			this.AssociatedObject.SelectAll();
		}

		/// <inheritdoc />
		protected override void OnDetaching()
		{
			this.AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus;
			base.OnDetaching();
		}
	}
}