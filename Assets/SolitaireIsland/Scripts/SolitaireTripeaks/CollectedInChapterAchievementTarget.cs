using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Linq;

namespace SolitaireTripeaks
{
	public class CollectedInChapterAchievementTarget : IAchievementTarget
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
			return PlayData.Get().GetChapter(config.scheduleData.world, config.scheduleData.chapter)?.lvs.Sum((LevelData e) => e.Star) ?? 0;
		}

		public int GetTotal(AchievementInfo achievementInfo)
		{
			AchievementConfig config = achievementInfo.GetConfig();
			ChapterConfig chapterConfig = SingletonClass<AAOConfig>.Get().GetChapterConfig(config.scheduleData.world, config.scheduleData.chapter);
			if (chapterConfig == null)
			{
				return 60;
			}
			return chapterConfig.LevelCount * 3;
		}

		public string GetDescription(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString("Collected_In_Chapter"), SingletonClass<AAOConfig>.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name);
		}

		public string GetTitle(AchievementInfo info)
		{
			return string.Format(LocalizationUtility.Get("Localization_achievement.json").GetString(info.GetConfig().Title), SingletonClass<AAOConfig>.Get().GetChapterConfig(info.GetConfig().scheduleData.world, info.GetConfig().scheduleData.chapter).name);
		}
	}
}
