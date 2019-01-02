using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Generator.Shared.DependencyInjection;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class UIService : IUIService
    {
	    /// <inheritdoc />
	    public async Task<IProgressController> ShowProgressAsync(string title, string message, bool cancelable)
	    {
		    if (App.Current.MainWindow is MetroWindow window)
		    {
			    return new ProgressControllerAdapter(await window.ShowProgressAsync(title, message, cancelable));
		    }

			throw new Exception($"{App.Current.MainWindow} is expected to be of type {nameof(MetroWindow)} at this point.");
	    }

	    /// <inheritdoc />
	    public bool GetYesNo(string question, string title)
	    {
		    return MessageBox.Show(question, title, MessageBoxButtons.YesNo) == DialogResult.Yes;
	    }
    }
}
