using Nightingale.Localization;
using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class CompeletedChapterAchievementTarget : IAchievementTarget
	{
		public void DOAchievement(AchievementInfo info, ScheduleData data)
		{
			ScheduleData scheduleData = info.GetConfig().scheduleData;
			if (scheduleData.world == data.world && scheduleData.chapter == data.chapter)
			{
				info.CurrentCount++;
			}
		}

		public int GetCurrent(AchievementInfo achievementInfo)
		{
			AchievementConfig config = achievementInfo.GetConfig();
			ChapterData chapter = PlayData.Get().GetChapter(config.scheduleData.world, config.scheduleData.chapter);
			if (chapter != null)
			{
				return chapter.lvs.Count;
			}
			return 0;
		}

		public int GetTotal(AchievementInfo achievementInfo)
		{
			AchievementConfig config = achievementInfo.GetConfig();
			return SingletonClass<AAOConfig>.Get().GetChapterConfig(config.scheduleData.world, config.scheduleData.chapter)?.LevelCount ?? 20;
		}

		public string GetDescription(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Compeleted_Chapter"), SingletonClass<AAOConfig>.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name);
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title), SingletonClass<AAOConfig>.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name);
		}
	}
}
