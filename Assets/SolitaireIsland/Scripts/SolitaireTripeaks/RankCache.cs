using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class RankCache : SingletonData<RankCache>
	{
		public string RankId;

		public int Rank;

		public RankGrow RankGrow;

		public RankCache()
		{
			RankId = string.Empty;
			Rank = -1;
		}

		private RankGrow CalcRankGrow(int rank, int last)
		{
			if (rank < 0)
			{
				rank = 5001;
			}
			if (last < 0)
			{
				last = 5001;
			}
			if (rank < last)
			{
				return RankGrow.Up;
			}
			if (rank > last)
			{
				return RankGrow.Down;
			}
			return RankGrow.Default;
		}

		public void Put(string id, int r)
		{
			if (id.Equals(RankId))
			{
				RankGrow = CalcRankGrow(r, Rank);
				Rank = r;
			}
			else
			{
				RankId = id;
				Rank = r;
				RankGrow = RankGrow.Default;
			}
			FlushData();
		}
	}
}
