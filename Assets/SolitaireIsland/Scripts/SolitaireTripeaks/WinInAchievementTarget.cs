using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class WinInAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
			if (data.world == info.GetConfig().scheduleData.world && data.chapter == info.GetConfig().scheduleData.chapter)
			{
				info.CurrentCount++;
			}
		}

		public string GetDescription(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("WinIn"), UniverseConfig.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name);
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title));
		}

		public int GetCurrent(AchievementInfo achievementInfo)
		{
			return achievementInfo.CurrentCount;
		}

		public int GetTotal(AchievementInfo achievementInfo)
		{
			return achievementInfo.GetConfig().NeedCount;
		}
	}
}
