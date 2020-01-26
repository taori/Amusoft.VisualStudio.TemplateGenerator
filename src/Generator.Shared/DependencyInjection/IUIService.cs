using System.Threading.Tasks;

namespace Generator.Shared.DependencyInjection
{
	public interface IUIService
	{
		Task<IProgressController> ShowProgressAsync(string title, string message, bool cancelable);
		bool GetYesNo(string question, string title);
		void DisplayMessage(string message, string title);
		void DisplayError(string message, string title);
	}
}