using DG.Tweening;
using Nightingale.ScenesManager;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SceneInternalAnimationUI : MonoBehaviour
	{
		public float HidePositionY;

		public float ShowPositionY;

		public float MiddlePositionY;

		private void Awake()
		{
			BaseScene componentInParent = base.gameObject.GetComponentInParent<BaseScene>();
			if (!(componentInParent == null))
			{
				RectTransform rectTransform = base.transform as RectTransform;
				Sequence s = DOTween.Sequence();
				s.AppendInterval(0.5f);
				s.Append(rectTransform.DOAnchorPosY(MiddlePositionY, 0.3f));
				s.Append(rectTransform.DOAnchorPosY(ShowPositionY, 0.1f));
				componentInParent.SceneStateChanged.AddListener(delegate(SceneState state)
				{
					if (state == SceneState.Closing)
					{
						rectTransform.DOAnchorPosY(HidePositionY, 0.3f);
					}
				});
			}
		}
	}
}
