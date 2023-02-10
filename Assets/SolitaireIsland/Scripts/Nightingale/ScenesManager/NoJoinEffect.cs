using DG.Tweening;
using UnityEngine;

namespace Nightingale.ScenesManager
{
	public class NoJoinEffect : NavigationEffect
	{
		private const float width = 1920f;

		private const float height = 1080f;

		private float timeScale = 1f;

		private Vector3 currentDir = Vector3.one;

		public NoJoinEffect(JoinEffectDir dir = JoinEffectDir.Top)
		{
			float num = 0.5625f * (float)Screen.width;
			if (num < (float)Screen.height)
			{
				timeScale *= (float)Screen.height / num;
			}
			switch (dir)
			{
			case JoinEffectDir.Left:
				currentDir = new Vector2(-1920f * timeScale, 0f);
				break;
			case JoinEffectDir.Right:
				currentDir = new Vector2(1920f * timeScale, 0f);
				break;
			case JoinEffectDir.Bottom:
				currentDir = new Vector2(0f, -1080f * timeScale);
				break;
			default:
				currentDir = new Vector2(0f, 1080f * timeScale);
				break;
			}
		}

		public override void Open(BaseScene scene, TweenCallback tweenCallback = null)
		{
			Transform sceneEffectTransform = scene.GetSceneEffectTransform();
			scene.SetCanvasGraphicRaycaster(enabled: false);
			sceneEffectTransform.localPosition = currentDir;
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOLocalMove(Vector3.zero, 0.5f * timeScale));
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
			sceneEffectTransform.localPosition = -currentDir * 1.2f;
			sceneEffectTransform.gameObject.SetActive(value: true);
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOLocalMove(Vector3.zero, 0.5f * timeScale));
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
			sequence.Append(effectTransform.DOLocalMove(-currentDir, 0.5f * timeScale));
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
			sceneEffectTransform.localPosition = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.SetUpdate(isIndependentUpdate: true);
			sequence.Append(sceneEffectTransform.DOLocalMove(currentDir, 1f * timeScale));
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
