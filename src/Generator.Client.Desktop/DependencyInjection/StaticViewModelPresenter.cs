using System;
using System.Collections.Generic;
using Generator.Client.Desktop.Utility;
using Generator.Client.Desktop.Views;
using Generator.Shared.DependencyInjection;
using Generator.Shared.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class StaticViewModelPresenter : IViewModelPresenter
	{
		private readonly Dictionary<Type, Action<object>> mappings = new Dictionary<Type, Action<object>>();
		
		/// <inheritdoc />
		public void Present(object viewModel)
		{
			if (!mappings.TryGetValue(viewModel.GetType(), out var action))
				throw new Exception($"{viewModel.GetType()} is not mapped.");

			action(viewModel);
		}

		public void Build()
		{
			mappings.Add(typeof(ConfigurationViewModel), ConfigurationViewModel);
			mappings.Add(typeof(ManageOpenInEditorReferencesViewModel), ManageOpenInEditorReferencesViewModel);
		}

		private void ManageOpenInEditorReferencesViewModel(object obj)
		{
			if (obj is ManageOpenInEditorReferencesViewModel viewModel)
			{
				var window = new ManageOpenInEditorReferencesWindow();
				viewModel.WhenConfirmed.Subscribe(model => window.Close());
				viewModel.WhenDiscarded.Subscribe(model => window.Close());
				window.DataContext = viewModel;
				window.Show();
			}
		}

		private void ConfigurationViewModel(object obj)
		{
			if (obj is ConfigurationViewModel viewModel)
			{
				var window = new ConfigurationEditWindow();
				Interaction.GetBehaviors(window).Add(new CloseOnEscapeBehavior());
				var editModel = new ConfigurationViewModel(viewModel.Model);
				window.DataContext = editModel;
				editModel.WhenSaved.Subscribe(async(_) =>
				{
					viewModel.NotifySaved();
				});
				editModel.WhenConfirm.Subscribe(_ => window.Close());
				editModel.WhenDiscard.Subscribe(_ => window.Close());
				window.Show();
			}
		}
	}
}