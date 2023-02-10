using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyReward
	{
		public string boosterType;

		public int count;

		public int weight;

		public BoosterType GetBoosterType()
		{
			BoosterType boosterType = EnumUtility.GetEnumType(this.boosterType, BoosterType.RandomBooster);
			if (boosterType == BoosterType.RandomBooster)
			{
				boosterType = AppearNodeConfig.Get().GetRandomBooster();
			}
			return boosterType;
		}
	}
}
