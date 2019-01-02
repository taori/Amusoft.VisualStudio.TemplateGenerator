using System;
using System.Threading.Tasks;
using Generator.Shared.DependencyInjection;
using MahApps.Metro.Controls.Dialogs;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class ProgressControllerAdapter : IProgressController
	{
		public ProgressDialogController DialogController { get; }

		public ProgressControllerAdapter(ProgressDialogController dialogController)
		{
			DialogController = dialogController;
		}

		/// <inheritdoc />
		public void SetIndeterminate()
		{
			DialogController.SetIndeterminate();
		}

		/// <inheritdoc />
		public async Task CloseAsync()
		{
			await DialogController.CloseAsync();
		}

		/// <inheritdoc />
		public event EventHandler Canceled
		{
			add { DialogController.Canceled += value; }
			remove { DialogController.Canceled -= value; }
		}

		/// <inheritdoc />
		public void SetMessage(string message)
		{
			DialogController.SetMessage(message);
		}
	}
}