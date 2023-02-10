using DG.Tweening;
using Nightingale.U2D;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderboardStateUI : MonoBehaviour
	{
		public DoubleSpriteUI doubleSpriteUI;

		public Text RemainLabel;

		public RectTransform Content;

		private void Awake()
		{
			ChangeRank(SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType());
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.AddListener(ChangeRank);
			RectTransform rectTransform = doubleSpriteUI.transform as RectTransform;
			RectTransform rectTransform2 = rectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			rectTransform2.anchoredPosition = new Vector2(anchoredPosition.x, 10f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(rectTransform.DOAnchorPosY(-10f, 0.7f));
			sequence.Append(rectTransform.DOAnchorPosY(10f, 0.7f));
			sequence.SetEase(Ease.Linear);
			sequence.SetLoops(-1);
			GetComponent<Button>().onClick.AddListener(delegate
			{
				doubleSpriteUI.gameObject.SetActive(value: false);
			});
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(ChangeRank);
			SingletonBehaviour<LeaderBoardUtility>.Get().MiniLeaderBoardEvent.RemoveListener(MiniRank);
		}

		private void RepeatingUpdate()
		{
			if (SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType() == RankType.Upload)
			{
				TimeSpan uploadRemainTime = SingletonBehaviour<LeaderBoardUtility>.Get().GetUploadRemainTime();
				if (uploadRemainTime.TotalDays >= 1.0)
				{
					RectTransform content = Content;
					Vector2 sizeDelta = Content.sizeDelta;
					content.sizeDelta = new Vector2(130f, sizeDelta.y);
					RemainLabel.text = $"{(int)uploadRemainTime.TotalDays}d";
				}
				else
				{
					RectTransform content2 = Content;
					Vector2 sizeDelta2 = Content.sizeDelta;
					content2.sizeDelta = new Vector2(200f, sizeDelta2.y);
					RemainLabel.text = $"{uploadRemainTime.Hours:D1}:{uploadRemainTime.Minutes:D1}:{uploadRemainTime.Seconds:D1}";
				}
			}
			else
			{
				RectTransform content3 = Content;
				Vector2 sizeDelta3 = Content.sizeDelta;
				content3.sizeDelta = new Vector2(200f, sizeDelta3.y);
				RemainLabel.text = "Pending";
			}
		}

		private void ChangeRank(RankType type)
		{
			if (type != RankType.Upload)
			{
				doubleSpriteUI.gameObject.SetActive(value: false);
				SingletonBehaviour<LeaderBoardUtility>.Get().MiniLeaderBoardEvent.RemoveListener(MiniRank);
			}
			else
			{
				SingletonBehaviour<LeaderBoardUtility>.Get().MiniLeaderBoardEvent.AddListener(MiniRank);
				SingletonBehaviour<LeaderBoardUtility>.Get().GetMiniRank();
			}
		}

		private void MiniRank(int rank, RankGrow grow)
		{
			switch (grow)
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
		}
	}
}
