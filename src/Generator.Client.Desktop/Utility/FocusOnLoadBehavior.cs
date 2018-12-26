using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Generator.Client.Desktop.Utility
{
	public class FocusOnLoadBehavior : Behavior<Control>
	{
		public static readonly DependencyProperty FocusTargetProperty = DependencyProperty.Register(
			nameof(FocusTarget), typeof(Control), typeof(FocusOnLoadBehavior), new PropertyMetadata(default(Control)));

		public Control FocusTarget
		{
			get { return (Control) GetValue(FocusTargetProperty); }
			set { SetValue(FocusTargetProperty, value); }
		}

		/// <inheritdoc />
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Loaded += AssociatedObjectOnLoaded;
		}

		private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
		{
			FocusTarget.Focus();
			AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
		}
	}
}