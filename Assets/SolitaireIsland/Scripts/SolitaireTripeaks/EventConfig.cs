using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class EventConfig
	{
		public DayOfWeek DayOfWeek;

		public QuestType QuestType;

		public ScheduleData ScheduleData;

		public List<EventItemConfig> Items;

		public List<QuestConfig> GetConfigs(DateTime Today)
		{
			string identifier = Today.Date.AddDays(DayOfWeek - Today.Date.DayOfWeek).ToString("yyyy/M/d");
			return (from p in Items
				select new QuestConfig
				{
					identifier = identifier,
					StartTime = identifier,
					Hours = 24,
					ScheduleData = ScheduleData,
					QuestType = QuestType,
					NeedCount = p.NeedCount,
					BoosterType = p.BoosterType,
					RewardCount = p.RewardCount
				}).ToList();
		}

		public QuestConfig GetConfig(DateTime Today)
		{
			string text = Today.Date.AddDays(DayOfWeek - Today.Date.DayOfWeek).ToString("yyyy/M/d");
			QuestConfig questConfig = new QuestConfig();
			questConfig.identifier = text;
			questConfig.StartTime = text;
			questConfig.Hours = 24;
			questConfig.ScheduleData = ScheduleData;
			questConfig.QuestType = QuestType;
			questConfig.NeedCount = Items[0].NeedCount;
			questConfig.BoosterType = Items[0].BoosterType;
			questConfig.RewardCount = Items[0].RewardCount;
			return questConfig;
		}
	}
}
