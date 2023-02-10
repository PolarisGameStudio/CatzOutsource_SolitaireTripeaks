using Nightingale.ScenesManager;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubLeaderboardUI : MonoBehaviour
	{
		public Image Icon;

		public Text NameLabel;

		public Text RankLabel;

		public Text ScoreLabel;

		public CashText RewardLabel;

		public Button Button;

		public Sprite[] Ranks;

		public Image RankImage;

		public Image StageImage;

		public int RankIndex;

		public GameObject UpObject;

		public GameObject DownObject;

		public GameObject NormalBackground;

		public GameObject PlayerBackground;

		public void ScrollCellContent(RankClub rankClub)
		{
			RankIndex = rankClub.Rank;
			bool flag = SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier().Equals(rankClub.ClubId);
			if (NormalBackground != null && PlayerBackground != null)
			{
				NormalBackground.SetActive(!flag);
				PlayerBackground.SetActive(flag);
			}
			NameLabel.text = rankClub.ClubName;
			if (flag)
			{
				NameLabel.color = Color.white;
			}
			else
			{
				NameLabel.color = new Color32(112, 42, 14, byte.MaxValue);
			}
			switch (rankClub.RankGrow)
			{
			case RankGrow.Up:
				UpObject.SetActive(value: true);
				DownObject.SetActive(value: false);
				break;
			case RankGrow.Down:
				UpObject.SetActive(value: false);
				DownObject.SetActive(value: true);
				break;
			default:
				UpObject.SetActive(value: false);
				DownObject.SetActive(value: false);
				break;
			}
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(rankClub.Stage, isClan: true);
			RankLabel.text = rankClub.Rank.ToString();
			ScoreLabel.text = rankClub.Score.ToString();
			Icon.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(rankClub.ClubIcon);
			RewardLabel.SetLeaderBoardCash(rankClub.Coins);
			UpdateRankImage(rankClub.Rank);
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ClubScene>("Scenes/ClubScene").OnStart(new Club
				{
					ClubId = rankClub.ClubId,
					ClubName = rankClub.ClubName,
					ClubIcon = rankClub.ClubIcon,
					Private = rankClub.Private,
					MemberCount = rankClub.MemberCount,
					MemberLimit = rankClub.MemberLimit,
					Level = rankClub.Level
				});
			});
		}

		private void UpdateRankImage(int position)
		{
			if (position <= 3)
			{
				RankImage.sprite = Ranks[position - 1];
				RankImage.gameObject.SetActive(value: true);
			}
			else
			{
				RankImage.gameObject.SetActive(value: false);
			}
		}
	}
}
