using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class JoinAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
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
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Join_Bella"));
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title));
		}
	}
}
