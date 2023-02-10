using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubAvatarScene : BaseScene
	{
		public Transform ContentTransform;

		public void OnStart(UnityAction<Sprite> unityAction)
		{
			Sprite[] array = Resources.LoadAll<Sprite>("ClubAvatar");
			Sprite[] array2 = array;
			foreach (Sprite sprite in array2)
			{
				GameObject gameObject = new GameObject(sprite.name);
				gameObject.AddComponent<Image>().sprite = sprite;
				gameObject.AddComponent<Button>().onClick.AddListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Close();
					if (unityAction != null)
					{
						unityAction(sprite);
					}
				});
				gameObject.transform.SetParent(ContentTransform, worldPositionStays: false);
			}
		}
	}
}
