using DG.Tweening;
using UnityEngine;

namespace Nightingale.ScenesManager
{
	public class DelayEffect : NavigationEffect
	{
		private float delayInterval = 1f;

		public DelayEffect(float delayInterval)
		{
			this.delayInterval = delayInterval;
		}

		public override void Open(BaseScene scene, TweenCallback tweenCallback = null)
		{
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			scene.SetCanvasGraphicRaycaster(enabled: false);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(delayInterval);
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.OnComplete(delegate
			{
				if (tweenCallback != null)
				{
					tweenCallback();
				}
				scene.SetCanvasGraphicRaycaster(enabled: true);
			});
		}

		public override void Show(BaseScene scene, TweenCallback tweenCallback = null)
		{
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			scene.SetCanvasGraphicRaycaster(enabled: false);
			sceneEffectTransform.gameObject.SetActive(value: true);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(delayInterval);
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.OnComplete(delegate
			{
				if (tweenCallback != null)
				{
					tweenCallback();
				}
				scene.SetCanvasGraphicRaycaster(enabled: true);
			});
		}

		public override void Hide(BaseScene scene, TweenCallback tweenCallback = null)
		{
			Transform effectTransform = scene.GetSceneEffectTransform();
			scene.SetCanvasGraphicRaycaster(enabled: false);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(delayInterval);
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.OnComplete(delegate
			{
				effectTransform.localPosition = Vector3.zero;
				effectTransform.gameObject.SetActive(value: false);
				scene.SetCanvasGraphicRaycaster(enabled: true);
				if (tweenCallback != null)
				{
					tweenCallback();
				}
			});
		}

		public override void Closed(BaseScene scene, TweenCallback tweenCallback = null)
		{
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			scene.SetCanvasGraphicRaycaster(enabled: false);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(delayInterval);
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.OnComplete(delegate
			{
				if (tweenCallback != null)
				{
					tweenCallback();
				}
				UnityEngine.Object.Destroy(scene.gameObject);
			});
		}
	}
}
