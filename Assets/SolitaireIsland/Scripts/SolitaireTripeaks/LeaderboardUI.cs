using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderboardUI : MonoBehaviour
	{
		public Text RankLabel;

		public Text NickNameLabel;

		public Text ScoreLabel;

		public CashText RewardLabel;

		public Image AvaterImage;

		public Sprite[] Ranks;

		public Image RankImage;

		public Image StageImage;

		public LayoutElement layoutElement;

		public int RankIndex;

		public GameObject UpObject;

		public GameObject DownObject;

		public GameObject NormalBackground;

		public GameObject PlayerBackground;

		public FriendAvaterUI FriendAvaterUI;

		public void ScrollCellContent(RankUser rankUser)
		{
			RankIndex = rankUser.Position;
			bool flag = SolitaireTripeaksData.Get().GetPlayerId().Equals(rankUser.PlayerId);
			if (NormalBackground != null && PlayerBackground != null)
			{
				NormalBackground.SetActive(!flag);
				PlayerBackground.SetActive(flag);
			}
			NickNameLabel.text = rankUser.PlayerName;
			if (flag)
			{
				NickNameLabel.color = Color.white;
			}
			else
			{
				NickNameLabel.color = new Color32(112, 42, 14, byte.MaxValue);
			}
			switch (rankUser.RankGrow)
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
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(rankUser.Stage);
			RankLabel.text = rankUser.Position.ToString();
			ScoreLabel.text = rankUser.Score.ToString();
			FriendAvaterUI.SetUser(rankUser.SocialId, rankUser.SocialPlatform, rankUser.AvatarId);
			RewardLabel.SetLeaderBoardCash(rankUser.Coins);
			UpdateRankImage(rankUser.Position);
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
