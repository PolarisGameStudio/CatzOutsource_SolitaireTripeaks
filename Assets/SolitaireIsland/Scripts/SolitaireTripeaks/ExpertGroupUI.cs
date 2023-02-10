using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ExpertGroupUI : MonoBehaviour
	{
		public Button Button;

		public Text Label;

		public void OnStart(int group, string content, UnityAction<int> unityAction)
		{
			Label.text = content;
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(delegate
			{
				if (unityAction != null)
				{
					unityAction(group);
				}
			});
		}
	}
}
