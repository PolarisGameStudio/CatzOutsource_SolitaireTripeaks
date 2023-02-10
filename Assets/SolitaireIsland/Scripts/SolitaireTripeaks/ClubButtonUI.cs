using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubButtonUI : MonoBehaviour
	{
		public Text RemainLabel;

		public RectTransform Content;

		private void Start()
		{
			UpdateExclamationMark();
			InvokeRepeating("UpdateExclamationMark", 1f, 1f);
		}

		public void UpdateExclamationMark()
		{
			SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(base.gameObject, SingletonBehaviour<ClubSystemHelper>.Get().HasSuperTreasure() == SuperTreasure.Normal || ClubSystemData.Get().GetLeaderboardDatas().Count > 0);
			Content.gameObject.SetActive(!string.IsNullOrEmpty(SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier()));
			if (!Content.gameObject.activeSelf)
			{
				return;
			}
			if (SingletonBehaviour<ClubSystemHelper>.Get().GetRankType() == RankType.Upload)
			{
				TimeSpan uploadRemainTime = SingletonBehaviour<ClubSystemHelper>.Get().GetUploadRemainTime();
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
	}
}
