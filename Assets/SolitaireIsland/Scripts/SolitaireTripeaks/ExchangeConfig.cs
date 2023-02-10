using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ExchangeConfig
	{
		public BoosterType boosterType;

		public int count;

		public int need;

		public int limit;

		public bool IsActive()
		{
			if (boosterType == BoosterType.Poker)
			{
				PokerThemeConfig poker = PokerThemeGroup.Get().GetPoker(count);
				if (poker.IsCanUse())
				{
					return false;
				}
			}
			else if (limit > 0 && limit <= SpecialActivityData.Get().GetNumber(boosterType))
			{
				return false;
			}
			return true;
		}
	}
}
