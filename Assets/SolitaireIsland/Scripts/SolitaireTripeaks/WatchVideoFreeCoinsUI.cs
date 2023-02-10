using DG.Tweening;
using Nightingale.Ads;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class WatchVideoFreeCoinsUI : MonoBehaviour
	{
		public Button WatchButton;

		public float HidePosition;

		public float ActivePosition;

		public float MiddlePosition;

		public void TryShow(int level)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			RectTransform rectTransform2 = rectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			rectTransform2.anchoredPosition = new Vector2(anchoredPosition.x, HidePosition);
			if (GameConfig.Get().HasVideo(level) && SingletonBehaviour<ThirdPartyAdManager>.Get().IsRewardedVideoAvailable(AuxiliaryData.Get().WatchVideoCount))
			{
				AuxiliaryData.Get().WatchAdInLevelShow++;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(WatchButton.transform.DOScale(1.1f, 0.2f));
				sequence.Append(WatchButton.transform.DOScale(1f, 0.1f));
				sequence.SetLoops(-1);
				WatchButton.onClick.AddListener(delegate
				{
					SingletonBehaviour<ThirdPartyAdManager>.Get().compeleted.RemoveListener(WatchVideoCompleted);
					SingletonBehaviour<ThirdPartyAdManager>.Get().compeleted.AddListener(WatchVideoCompleted);
					SingletonBehaviour<ThirdPartyAdManager>.Get().ShowRewardedVideoAd();
				});
				base.gameObject.SetActive(value: true);
				sequence = DOTween.Sequence();
				sequence.Append(rectTransform.DOAnchorPosY(MiddlePosition, 0.2f));
				sequence.Append(rectTransform.DOAnchorPosY(ActivePosition, 0.2f));
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public void TryHide()
		{
			(base.transform as RectTransform).DOAnchorPosY(HidePosition, 0.2f).OnComplete(delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}

		private void OnDestroy()
		{
			SingletonBehaviour<ThirdPartyAdManager>.Get().compeleted.RemoveListener(WatchVideoCompleted);
		}

		private void WatchVideoCompleted(bool compeleted)
		{
			if (compeleted)
			{
				AuxiliaryData.Get().WatchAdInLevelCount++;
				AuxiliaryData.Get().WatchVideoCount++;
				AuxiliaryData.Get().WatchVideoTotal++;
				SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Video, 1000L, changed: false);
				TipPopupIconNumberScene.ShowVideoRewardCoins(1000);
				TryHide();
			}
		}
	}
}
