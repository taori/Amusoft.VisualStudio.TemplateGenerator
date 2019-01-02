using System;
using System.Threading;
using System.Threading.Tasks;

namespace Generator.Shared.DependencyInjection
{
	public interface IProgressController
	{
		void SetIndeterminate();
		Task CloseAsync();
		event EventHandler Canceled;
		void SetMessage(string message);
	}
}