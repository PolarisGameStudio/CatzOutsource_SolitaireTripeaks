using Nightingale.Utilitys;
using TriPeaks.ProtoData.Club;

namespace SolitaireTripeaks
{
	public class RankClub : ClubRank
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

		public RankClub(ClubRank rank, int stage, int min, int max)
		{
			Stage = stage;
			RankGrow = RankGrow.Default;
			if (min > 0 && rank.Rank <= min)
			{
				RankGrow = RankGrow.Up;
				Stage = stage + 1;
			}
			if (max > 0 && rank.Rank >= max)
			{
				RankGrow = RankGrow.Down;
				Stage = stage - 1;
			}
			base.Rank = rank.Rank;
			base.ClubId = rank.ClubId;
			base.ClubName = rank.ClubName;
			base.ClubIcon = rank.ClubIcon;
			base.Private = rank.Private;
			base.Level = rank.Level;
			base.MemberCount = rank.MemberCount;
			base.MemberLimit = rank.MemberLimit;
			base.Score = rank.Score;
			Coins = SingletonBehaviour<ClubSystemHelper>.Get().GetCoins(stage, rank.Rank);
		}
	}
}
