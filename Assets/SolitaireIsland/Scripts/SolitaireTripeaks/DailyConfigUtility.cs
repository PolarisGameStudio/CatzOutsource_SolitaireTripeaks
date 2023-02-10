using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class DailyConfigUtility
	{
		private DailyGroup dailyGroup;

		private static Dictionary<QuestStyle, DailyConfigUtility> configs = new Dictionary<QuestStyle, DailyConfigUtility>();

		public QuestConfig RandomQuestConfig()
		{
			QuestConfig questConfig = new QuestConfig();
			int num = UnityEngine.Random.Range(0, dailyGroup.dailyRewardLevels.Length);
			DailyConfig[] array = (from e in dailyGroup.dailyConfigs
				where e.IsEnable()
				select e).ToArray();
			DailyConfig dailyConfig = array[UnityEngine.Random.Range(0, array.Length)];
			questConfig.QuestType = dailyConfig.GetQuestType();
			DailyInfo dailyInfo = dailyConfig.dailyLevels[num].RandomDailyInfo(questConfig.QuestType);
			questConfig.ScheduleData = dailyInfo.scheduleData;
			questConfig.NeedCount = dailyInfo.NeedCount;
			DailyReward dailyReward = dailyGroup.dailyRewardLevels[num].RandomDailyReward();
			questConfig.RewardCount = dailyReward.count;
			questConfig.BoosterType = dailyReward.GetBoosterType();
			DailyTime[] dailyTimes = dailyGroup.dailyTimeLevels[num].dailyTimes;
			DailyTime dailyTime = dailyTimes[UnityEngine.Random.Range(0, dailyTimes.Length)];
			questConfig.Hours = dailyTime.hours;
			questConfig.identifier = Guid.NewGuid().ToString();
			return questConfig;
		}

		public QuestConfig QuestEasyConfig()
		{
			QuestConfig questConfig = new QuestConfig();
			questConfig.QuestType = QuestType.CollectSuitCard;
			questConfig.ScheduleData = default(ScheduleData);
			questConfig.BoosterType = BoosterType.BurnRope;
			questConfig.RewardCount = 1;
			questConfig.NeedCount = 20;
			questConfig.Hours = 24;
			return questConfig;
		}

		public static DailyConfigUtility Get(QuestStyle style)
		{
			if (!configs.ContainsKey(style))
			{
				DailyConfigUtility dailyConfigUtility = new DailyConfigUtility();
				if (style == QuestStyle.Daily)
				{
					dailyConfigUtility.dailyGroup = JsonUtility.FromJson<DailyGroup>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/DailyConfig.json"));
				}
				else
				{
					dailyConfigUtility.dailyGroup = JsonUtility.FromJson<DailyGroup>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/BounsConfig.json"));
				}
				configs.Add(style, dailyConfigUtility);
			}
			return configs[style];
		}
	}
}
