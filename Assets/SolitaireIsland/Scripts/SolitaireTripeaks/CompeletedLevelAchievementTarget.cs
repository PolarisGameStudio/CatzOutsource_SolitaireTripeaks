using Nightingale.Localization;
using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class CompeletedLevelAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
			ScheduleData scheduleData = info.GetConfig().scheduleData;
			if (scheduleData.world == data.world && scheduleData.chapter == data.chapter && scheduleData.level == data.level)
			{
				info.CurrentCount++;
			}
		}

		public int GetCurrent(AchievementInfo achievementInfo)
		{
			return achievementInfo.CurrentCount;
		}

		public int GetTotal(AchievementInfo achievementInfo)
		{
			return achievementInfo.GetConfig().NeedCount;
		}

		public string GetDescription(AchievementInfo info)
		{
			if (info.GetConfig().scheduleData.world == -1)
			{
				return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Compeleted_Level"), LocalizationUtility.Get("Localization_achievement.json").GetString("Master"), SingletonClass<AAOConfig>.Get().GetLevelInWorld(info.GetConfig().scheduleData) + 1);
			}
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Compeleted_Level"), SingletonClass<AAOConfig>.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name, SingletonClass<AAOConfig>.Get().GetLevel(info.GetConfig().scheduleData) + 1);
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title));
		}
	}
}
