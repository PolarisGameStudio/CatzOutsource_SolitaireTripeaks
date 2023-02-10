using DG.Tweening;
using UnityEngine;

namespace Nightingale.ScenesManager
{
	public class ScaleEffect : NavigationEffect
	{
		public override void Open(BaseScene scene, TweenCallback tweenCallback = null)
		{
			scene.SetCanvasGraphicRaycaster(enabled: false);
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			sceneEffectTransform.localScale = Vector3.forward;
			string text = $"NavigationEffect_{scene.GetInstanceID()}";
			DOTween.Kill(text);
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScaleX(1.2f, 0.25f));
			sequence.Join(sceneEffectTransform.DOScaleY(1.2f, 0.3f));
			sequence.Append(sceneEffectTransform.DOScaleX(1f, 0.1f));
			sequence.Join(sceneEffectTransform.DOScaleY(1f, 0.15f));
			sequence.SetId(text);
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
			sceneEffectTransform.localScale = Vector3.forward;
			string text = $"NavigationEffect_{scene.GetInstanceID()}";
			DOTween.Kill(text);
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScaleX(1.2f, 0.25f));
			sequence.Join(sceneEffectTransform.DOScaleY(1.2f, 0.3f));
			sequence.Append(sceneEffectTransform.DOScaleX(1f, 0.1f));
			sequence.Join(sceneEffectTransform.DOScaleY(1f, 0.15f));
			sequence.SetId(text);
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
			string text = $"NavigationEffect_{scene.GetInstanceID()}";
			DOTween.Kill(text);
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(effectTransform.DOScaleX(1.2f, 0.1f));
			sequence.Join(effectTransform.DOScaleY(1.2f, 0.15f));
			sequence.Append(effectTransform.DOScaleX(0f, 0.3f));
			sequence.Join(effectTransform.DOScaleY(0f, 0.35f));
			sequence.SetId(text);
			sequence.OnComplete(delegate
			{
				effectTransform.localScale = Vector3.forward;
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
			string text = $"NavigationEffect_{scene.GetInstanceID()}";
			DOTween.Kill(text);
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScaleX(1.2f, 0.1f));
			sequence.Join(sceneEffectTransform.DOScaleY(1.2f, 0.15f));
			sequence.Append(sceneEffectTransform.DOScaleX(0f, 0.2f));
			sequence.Join(sceneEffectTransform.DOScaleY(0f, 0.25f));
			sequence.SetId(text);
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
