using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public enum AchievementType
	{
		[Type(typeof(InviteFriendAchievementTarget))]
		InviteFriend,
		[Type(typeof(HelpFriendAchievementTarget))]
		HelpFriend,
		[Type(typeof(AskHelpAchievementTarget))]
		AskHelp,
		[Type(typeof(ClearSuitCardAchievementTarget))]
		ClearSuitCard,
		[Type(typeof(PlayAchievementTarget))]
		Play,
		[Type(typeof(WinAchievementTarget))]
		Win,
		[Type(typeof(WinInAchievementTarget))]
		WinIn,
		[Type(typeof(QuestAchievementTarget))]
		Quest,
		[Type(typeof(UseBoosterAchievementTarget))]
		UseBooster,
		[Type(typeof(UseWildAchievementTarget))]
		UseWild,
		[Type(typeof(RowDayAchievementTarget))]
		RowDay,
		[Type(typeof(CompeletedLevelAchievementTarget))]
		CompeletedLevel,
		[Type(typeof(CompeletedChapterAchievementTarget))]
		CompeletedChapter,
		[Type(typeof(CollectedInChapterAchievementTarget))]
		CollectedAllStarsInChapter,
		[Type(typeof(LoginFacebookAchievementTarget))]
		LoginFacebook,
		[Type(typeof(StreaksAchievementTarget))]
		Streaks,
		[Type(typeof(JoinAchievementTarget))]
		Join,
		[Type(typeof(UseRocketAchievementTarget))]
		UseRocket,
		[Type(typeof(UseSnakeAchievementTarget))]
		UseSnake
	}
}
