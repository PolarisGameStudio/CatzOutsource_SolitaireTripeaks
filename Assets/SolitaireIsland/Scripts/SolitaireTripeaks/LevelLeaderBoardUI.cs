using Nightingale.Utilitys;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LevelLeaderBoardUI : MonoBehaviour
	{
		public Text NickNameLabel;

		public Text RankLabel;

		public Text ScoreLabel;

		public Image AvaterImage;

		public void SetRecord(Rank record)
		{
			NickNameLabel.text = record.PlayerName;
			RankLabel.text = record.Position.ToString();
			ScoreLabel.text = record.Score.ToString();
			AvaterImage.sprite = SingletonClass<AvaterUtility>.Get().GetAvater(record.AvatarId);
		}
	}
}
