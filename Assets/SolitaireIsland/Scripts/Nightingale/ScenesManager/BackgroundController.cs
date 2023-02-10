using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.ScenesManager
{
	public class BackgroundController : MonoBehaviour
	{
		private static BackgroundController controller;

		public Image Background;

		private Tweener tweener;

		public static BackgroundController Get()
		{
			if (controller == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("SceneBackground"));
				controller = gameObject.GetComponent<BackgroundController>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return controller;
		}

		public virtual void SetLayer(string sortingLayerName, int sortingOrder)
		{
			Canvas[] componentsInChildren = base.transform.GetComponentsInChildren<Canvas>();
			Canvas[] array = componentsInChildren;
			foreach (Canvas canvas in array)
			{
				canvas.sortingLayerName = sortingLayerName;
				canvas.sortingOrder = sortingOrder;
			}
		}

		public void SetAnimation(bool active)
		{
			if (tweener != null)
			{
				tweener.Kill();
			}
			float num = 1f;
			float num2 = 0.5625f * (float)Screen.width;
			if (num2 < (float)Screen.height)
			{
				num *= (float)Screen.height / num2;
			}
			float duration = 0.61f * num;
			tweener = Background.DOFade((!active) ? 0f : 0.78f, duration);
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}
	}
}
