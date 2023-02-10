namespace SolitaireTripeaks
{
	public interface IAchievementTarget
	{
		void DOAchievement(AchievementInfo info, ScheduleData data);

		string GetDescription(AchievementInfo info);

		string GetTitle(AchievementInfo info);

		int GetCurrent(AchievementInfo achievementInfo);

		int GetTotal(AchievementInfo achievementInfo);
	}
}
