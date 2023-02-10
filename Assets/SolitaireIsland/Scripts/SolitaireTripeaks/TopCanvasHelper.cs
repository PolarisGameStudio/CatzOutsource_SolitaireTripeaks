using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TopCanvasHelper : SingletonBehaviour<TopCanvasHelper>
	{
		private Canvas canvas;

		private void Awake()
		{
			canvas = base.gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = Camera.main;
			canvas.planeDistance = 100f;
			canvas.sortingLayerName = "TopLayer";
			CanvasScaler canvasScaler = base.gameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
		}

		public void CreateBooster(BoosterType boosterType, Vector3 start, Vector3 end, float delay, TweenCallback tweenCallback)
		{
			DelayDo(new WaitForSeconds(delay), delegate
			{
				GameObject gameObject = new GameObject("Booster");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				Image image = gameObject.AddComponent<Image>();
				image.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(boosterType);
				image.SetNativeSize();
				gameObject.transform.position = start;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, 720f), 1f, RotateMode.FastBeyond360));
				sequence.Join(gameObject.transform.DOMove(end, 1f));
				sequence.OnComplete(delegate
				{
					if (tweenCallback != null)
					{
						tweenCallback();
					}
					UnityEngine.Object.Destroy(gameObject);
				});
				sequence.SetUpdate(isIndependentUpdate: true);
			});
		}
	}
}
