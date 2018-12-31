using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Generator.Client.Desktop.Utility
{
	public class AdditionalBehaviorsCollection : Collection<Behavior>
	{

	}
	public static class BehaviorInStyleAttacher
	{
		public static readonly DependencyProperty BehaviorsProperty =
			DependencyProperty.RegisterAttached(
				"Behaviors",
				typeof(AdditionalBehaviorsCollection),
				typeof(BehaviorInStyleAttacher),
				new UIPropertyMetadata(null, OnBehaviorsChanged));

		public static AdditionalBehaviorsCollection GetBehaviors(DependencyObject source)
		{
			return (AdditionalBehaviorsCollection)source.GetValue(BehaviorsProperty);
		}

		public static void SetBehaviors(DependencyObject source, AdditionalBehaviorsCollection value)
		{
			source.SetValue(BehaviorsProperty, value);
		}

		private static void OnBehaviorsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(e.NewValue is AdditionalBehaviorsCollection newBehaviors))
				return;

			BehaviorCollection currentBehaviors = Interaction.GetBehaviors(dependencyObject);
			foreach (var newBehavior in newBehaviors)
			{
				var behavior = Activator.CreateInstance(newBehavior.GetType()) as Behavior;
				currentBehaviors.Add(behavior);
			}
		}
	}
}