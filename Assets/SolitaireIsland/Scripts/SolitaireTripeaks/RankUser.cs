using Nightingale.Utilitys;
using TriPeaks.ProtoData.Leaderboard;

namespace SolitaireTripeaks
{
	public class RankUser : Rank
	{
		public int Coins
		{
			get;
			private set;
		}

		public int Stage
		{
			get;
			private set;
		}

		public RankGrow RankGrow
		{
			get;
			set;
		}

		public RankUser(Rank rank, int stage, int min, int max)
		{
			Stage = stage;
			RankGrow = RankGrow.Default;
			if (min > 0 && rank.Position <= min)
			{
				RankGrow = RankGrow.Up;
				Stage = stage + 1;
			}
			if (max > 0 && rank.Position >= max)
			{
				RankGrow = RankGrow.Down;
				Stage = stage - 1;
			}
			base.Position = rank.Position;
			base.PlayerId = rank.PlayerId;
			base.PlayerName = rank.PlayerName;
			base.SocialId = rank.SocialId;
			base.SocialPlatform = rank.SocialPlatform;
			base.AvatarId = rank.AvatarId;
			base.Score = rank.Score;
			Coins = SingletonBehaviour<LeaderBoardUtility>.Get().GetCoins(stage, rank.Position);
		}
	}
}
