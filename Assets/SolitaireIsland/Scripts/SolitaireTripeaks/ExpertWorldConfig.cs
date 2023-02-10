using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ExpertWorldConfig : WorldConfig
	{
		public override LevelConfig GetLevelConfig(ScheduleData scheduleData)
		{
			return SingletonClass<ExpertLevelConfigGroup>.Get().ReadLevelConfig(GetLevel(scheduleData));
		}

		public override LevelRetrunCoinConfig GetLevelRetrunCoinConfig(ScheduleData scheduleData)
		{
			return SingletonClass<ExpertLevelConfigGroup>.Get().ReadLevelRetrunCoinConfig(GetLevel(scheduleData));
		}
	}
}
