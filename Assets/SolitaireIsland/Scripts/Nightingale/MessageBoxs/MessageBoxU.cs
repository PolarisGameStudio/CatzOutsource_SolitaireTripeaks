using UnityEngine.Events;

namespace Nightingale.MessageBoxs
{
	public class MessageBoxU
	{
		public string title;

		public string description;

		public string leftButtonText;

		public string rightButtonText;

		public UnityAction<int> unityAction;
	}
}
