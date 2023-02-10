using DG.Tweening;
using UnityEngine;

namespace Nightingale.ScenesManager
{
	public class PivotScaleEffect : NavigationEffect
	{
		private Vector3 pivotPosition;

		public PivotScaleEffect(Vector3 pivot)
		{
			pivotPosition = pivot;
		}

		public override void Open(BaseScene scene, TweenCallback tweenCallback = null)
		{
			scene.SetCanvasGraphicRaycaster(enabled: false);
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			sceneEffectTransform.localScale = Vector3.forward;
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScaleX(1.2f, 0.25f));
			sequence.Join(sceneEffectTransform.DOScaleY(1.2f, 0.3f));
			sequence.Append(sceneEffectTransform.DOScaleX(1f, 0.1f));
			sequence.Join(sceneEffectTransform.DOScaleY(1f, 0.15f));
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
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScaleX(1.2f, 0.25f));
			sequence.Join(sceneEffectTransform.DOScaleY(1.2f, 0.3f));
			sequence.Append(sceneEffectTransform.DOScaleX(1f, 0.1f));
			sequence.Join(sceneEffectTransform.DOScaleY(1f, 0.15f));
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
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(effectTransform.DOScaleX(1.2f, 0.1f));
			sequence.Join(effectTransform.DOScaleY(1.2f, 0.15f));
			sequence.Append(effectTransform.DOScaleX(0f, 0.3f));
			sequence.Join(effectTransform.DOScaleY(0f, 0.35f));
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
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOScale(1.1f, 0.1f));
			sequence.Append(sceneEffectTransform.DOScale(0f, 0.3f));
			sequence.Join(sceneEffectTransform.DOMove(pivotPosition, 0.3f));
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
