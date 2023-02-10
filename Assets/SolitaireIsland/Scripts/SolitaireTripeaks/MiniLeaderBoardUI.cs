using DG.Tweening;
using Nightingale.Localization;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class MiniLeaderBoardUI : MonoBehaviour
	{
		public Text RankLabel;

		public Text StarLabel;

		public Text RankRemainTimeLabel;

		public Button LeaderBoardButton;

		public Button BoosterButton;

		public DoubleSpriteUI doubleSpriteUI;

		public Text DoubleRemainTimeLabel;

		public float HidePosition;

		public float ActivePosition;

		public float MiddlePosition;

		private Sequence sequence;

		public void OnLeaderBoardStart(bool delay)
		{
			base.gameObject.SetActive(value: false);
			if (SingletonBehaviour<LeaderBoardUtility>.Get().IsUploadEnable)
			{
				Sequence sequence = null;
				TweenCallback callback = delegate
				{
					SingletonBehaviour<LeaderBoardUtility>.Get().ClearMiniCache();
					RectTransform component = GetComponent<RectTransform>();
					RectTransform rectTransform = component;
					Vector2 anchoredPosition = component.anchoredPosition;
					rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, HidePosition);
					SingletonBehaviour<LeaderBoardUtility>.Get().MiniLeaderBoardEvent.AddListener(UpdateMiniLeaderBoard);
					SingletonBehaviour<LeaderBoardUtility>.Get().GetMiniRank();
					component = (doubleSpriteUI.transform as RectTransform);
					RectTransform rectTransform2 = component;
					Vector2 anchoredPosition2 = component.anchoredPosition;
					rectTransform2.anchoredPosition = new Vector2(anchoredPosition2.x, -30f);
					sequence = DOTween.Sequence();
					sequence.Append(component.DOAnchorPosY(-10f, 0.7f));
					sequence.Append(component.DOAnchorPosY(-30f, 0.7f));
					sequence.SetEase(Ease.Linear);
					sequence.SetLoops(-1);
					LeaderBoardButton.onClick.AddListener(delegate
					{
						AuxiliaryData.Get().LeaderBoardInLevelEndClick++;
					});
					SingletonBehaviour<GlobalConfig>.Get().CreateNumber(BoosterButton.gameObject, 0.7f, (int)PackData.Get().GetCommodity(BoosterType.DoubleStar).GetTotal(), left: false, -20f, -20f);
					BoosterButton.onClick.AddListener(delegate
					{
						SingletonBehaviour<GlobalConfig>.Get().BuyDoubleBooster(delegate(bool success)
						{
							if (success)
							{
								AuxiliaryData.Get().LeaderBoardInLevelEndBoost++;
								RepeatingUpdate();
							}
							else
							{
								SingletonBehaviour<GlobalConfig>.Get().CreateNumber(BoosterButton.gameObject, 0.7f, (int)PackData.Get().GetCommodity(BoosterType.DoubleStar).GetTotal(), left: false, -20f, -20f);
							}
						});
					});
				};
				sequence = DOTween.Sequence();
				if (delay)
				{
					sequence.AppendInterval(0.5f);
				}
				sequence.AppendCallback(callback);
			}
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(UpdateRank);
			SingletonBehaviour<LeaderBoardUtility>.Get().MiniLeaderBoardEvent.RemoveListener(UpdateMiniLeaderBoard);
		}

		private void RepeatingUpdate()
		{
			if (SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType() == RankType.Upload)
			{
				RankRemainTimeLabel.text = SingletonBehaviour<LeaderBoardUtility>.Get().GetUploadRemainTime().TOString();
			}
			if (RankCoinData.Get().IsDouble())
			{
				DoubleRemainTimeLabel.text = RankCoinData.Get().GetRemainTimeString();
			}
			else
			{
				DoubleRemainTimeLabel.text = LocalizationUtility.Get("Localization_popup.json").GetString("btn_buyDouble");
			}
		}

		private void UpdateRank(RankType rankType)
		{
			if (sequence != null)
			{
				sequence.Kill();
				sequence = null;
			}
			RectTransform component = GetComponent<RectTransform>();
			if (rankType == RankType.Upload)
			{
				base.gameObject.SetActive(value: true);
				sequence = DOTween.Sequence();
				sequence.Append(component.DOAnchorPosY(MiddlePosition, 0.2f));
				sequence.Append(component.DOAnchorPosY(ActivePosition, 0.3f));
			}
			else
			{
				sequence = DOTween.Sequence();
				sequence.Append(component.DOAnchorPosY(HidePosition, 0.2f));
				sequence.OnComplete(delegate
				{
					base.gameObject.SetActive(value: false);
				});
			}
		}

		private void UpdateMiniLeaderBoard(int rank, RankGrow rankState)
		{
			UnityEngine.Debug.Log("本次排行名次是：" + rank);
			AuxiliaryData.Get().LeaderBoardInLevelEndShow++;
			if (rank > 0)
			{
				RankLabel.text = $"{rank}";
			}
			else
			{
				RankLabel.text = "5000+";
			}
			base.gameObject.SetActive(value: true);
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(UpdateRank);
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.AddListener(UpdateRank);
			StarLabel.text = $"{RankCoinData.Get().RankCoinNumbers}";
			switch (rankState)
			{
			case RankGrow.Up:
				doubleSpriteUI.gameObject.SetActive(value: true);
				doubleSpriteUI.SetState(normal: true);
				break;
			case RankGrow.Down:
				doubleSpriteUI.gameObject.SetActive(value: true);
				doubleSpriteUI.SetState(normal: false);
				break;
			case RankGrow.Default:
				doubleSpriteUI.gameObject.SetActive(value: false);
				break;
			}
			RectTransform component = GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			Vector2 anchoredPosition = component.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, HidePosition);
			Sequence s = DOTween.Sequence();
			s.Append(component.DOAnchorPosY(MiddlePosition, 0.2f));
			s.Append(component.DOAnchorPosY(ActivePosition, 0.3f));
		}
	}
}
