using DG.Tweening;
using UnityEngine;

namespace Nightingale.ScenesManager
{
	public class NavigationEffect
	{
		public virtual void Open(BaseScene scene, TweenCallback tweenCallback = null)
		{
			DOTween.Kill($"NavigationEffect_{scene.GetInstanceID()}");
			tweenCallback?.Invoke();
		}

		public virtual void Show(BaseScene scene, TweenCallback tweenCallback = null)
		{
			DOTween.Kill($"NavigationEffect_{scene.GetInstanceID()}");
			scene.gameObject.SetActive(value: true);
			tweenCallback?.Invoke();
		}

		public virtual void Hide(BaseScene scene, TweenCallback tweenCallback = null)
		{
			DOTween.Kill($"NavigationEffect_{scene.GetInstanceID()}");
			scene.gameObject.SetActive(value: false);
			tweenCallback?.Invoke();
		}

		public virtual void Closed(BaseScene scene, TweenCallback tweenCallback = null)
		{
			DOTween.Kill($"NavigationEffect_{scene.GetInstanceID()}");
			tweenCallback?.Invoke();
			UnityEngine.Object.Destroy(scene.gameObject);
		}
	}
}
