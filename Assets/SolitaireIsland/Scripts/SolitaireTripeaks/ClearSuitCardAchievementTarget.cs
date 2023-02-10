using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class ClearSuitCardAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
			if (data.world == info.GetConfig().scheduleData.world)
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
			string[] array = new string[4]
			{
				"Spade",
				"Heart",
				"Club",
				"Diamond"
			};
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Clear_Suit_Card"), info.GetConfig().NeedCount, LocalizationUtility.Get("Localization_achievement.json").GetString(array[info.GetConfig().scheduleData.world]));
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title));
		}
	}
}
