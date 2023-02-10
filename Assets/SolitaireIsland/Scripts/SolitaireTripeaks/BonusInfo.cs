using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class BonusInfo
	{
		public int maxLevel;

		public PurchasingCommodity[] commoditys;

		public string guid;

		public string playerId;
	}
}
