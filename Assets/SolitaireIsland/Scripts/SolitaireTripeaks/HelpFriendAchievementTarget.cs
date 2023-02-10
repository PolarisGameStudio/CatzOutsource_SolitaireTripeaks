using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class HelpFriendAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
			info.CurrentCount += data.world;
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
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Help_Friend"), info.GetConfig().NeedCount);
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title));
		}
	}
}
