using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AchievementConfig
	{
		public string Date;

		public string identifier;

		public AchievementType achievementType;

		public ScheduleData scheduleData;

		public int NeedCount;

		public string AvaterFileName;

		public string Title;

		public int OrderIndex;

		public bool IsSpecial()
		{
			return !string.IsNullOrEmpty(Date);
		}

		public bool IsJourney()
		{
			if (IsSpecial())
			{
				return false;
			}
			if (IsSocial())
			{
				return false;
			}
			if (achievementType == AchievementType.CompeletedChapter || achievementType == AchievementType.CollectedAllStarsInChapter)
			{
				return false;
			}
			return true;
		}

		public bool IsSocial()
		{
			if (achievementType == AchievementType.LoginFacebook || achievementType == AchievementType.AskHelp || achievementType == AchievementType.HelpFriend || achievementType == AchievementType.InviteFriend)
			{
				return true;
			}
			return false;
		}

		public bool IsWorld(int world)
		{
			if (achievementType == AchievementType.CompeletedChapter || achievementType == AchievementType.CollectedAllStarsInChapter)
			{
				return scheduleData.world == world;
			}
			return false;
		}
	}
}
