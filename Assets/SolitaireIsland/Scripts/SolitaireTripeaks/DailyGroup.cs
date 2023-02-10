using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyGroup
	{
		public DailyConfig[] dailyConfigs;

		public DailyRewardLevel[] dailyRewardLevels;

		public DailyTimeLevel[] dailyTimeLevels;
	}
}
