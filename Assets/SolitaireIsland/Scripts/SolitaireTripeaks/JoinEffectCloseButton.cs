using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class JoinEffectCloseButton : MonoBehaviour
	{
		private Button button;

		public JoinEffectDir joinEffectDir;

		private void Awake()
		{
			button = GetComponent<Button>();
			if (!(button == null))
			{
				button.onClick.AddListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Close(new JoinEffect(joinEffectDir));
				});
			}
		}
	}
}
