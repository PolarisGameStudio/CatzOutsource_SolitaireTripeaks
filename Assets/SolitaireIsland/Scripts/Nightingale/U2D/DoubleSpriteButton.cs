using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.U2D
{
	public class DoubleSpriteButton : DoubleSpriteUI
	{
		public Button button;

		public void AddListener(UnityAction unityAction)
		{
			if (button == null)
			{
				button = GetComponent<Button>();
				if (button == null)
				{
					button = base.gameObject.AddComponent<Button>();
				}
			}
			button.onClick.AddListener(unityAction);
		}

		public void RemoveAllListeners()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
				if (button == null)
				{
					button = base.gameObject.AddComponent<Button>();
				}
			}
			button.onClick.RemoveAllListeners();
		}
	}
}
