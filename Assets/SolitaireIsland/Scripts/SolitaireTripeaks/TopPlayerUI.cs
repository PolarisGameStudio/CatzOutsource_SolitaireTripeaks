using Nightingale.Utilitys;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TopPlayerUI : MonoBehaviour
	{
		public Text RankLabel;

		public Text NickNameLabel;

		public Image AvaterImage;

		public Image StageImage;

		public Image BackgroundImage;

		public FriendAvaterUI FriendAvaterUI;

		private void ScrollCellContent(TopPlayer topPlayerUser)
		{
			BackgroundImage.color = ((!SolitaireTripeaksData.Get().GetPlayerId().Equals(topPlayerUser.PlayerId)) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(byte.MaxValue, 252, 137, byte.MaxValue));
			topPlayerUser.Stage = Mathf.Max(1, topPlayerUser.Stage);
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite(topPlayerUser.Stage);
			StageImage.SetNativeSize();
			NickNameLabel.text = topPlayerUser.PlayerName;
			RankLabel.text = topPlayerUser.Position.ToString();
			FriendAvaterUI.SetUser(topPlayerUser.SocialId, topPlayerUser.SocialPlatform, topPlayerUser.AvatarId);
		}
	}
}
