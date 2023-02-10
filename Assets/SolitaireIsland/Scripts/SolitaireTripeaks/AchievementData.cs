using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AchievementData
	{
		public List<AchievementInfo> AchievementDatas;

		public UnityEvent Changed = new UnityEvent();

		public AchievementData()
		{
			AchievementDatas = new List<AchievementInfo>();
		}

		public static AchievementData Get()
		{
			if (SolitaireTripeaksData.Get().Achievement == null)
			{
				SolitaireTripeaksData.Get().Achievement = new AchievementData();
			}
			return SolitaireTripeaksData.Get().Achievement;
		}

		public void DOChanged()
		{
			Changed.Invoke();
		}

		public void CalcAchievement()
		{
			for (int i = 0; i < UniverseConfig.Get().worlds.Count; i++)
			{
				List<ChapterConfig> chapters = UniverseConfig.Get().worlds[i].chapters;
				for (int j = 0; j < chapters.Count; j++)
				{
					CalcAchievement(i, j);
				}
			}
			foreach (AchievementInfo achievementData in AchievementDatas)
			{
				if (achievementData.GetConfig() != null && achievementData.GetConfig().achievementType == AchievementType.CompeletedLevel && PlayData.Get().HasLevelData(achievementData.GetConfig().scheduleData))
				{
					PutAchievement(AchievementType.CompeletedLevel, achievementData.GetConfig().scheduleData);
				}
			}
			if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
			{
				DoAchievement(AchievementType.LoginFacebook);
			}
		}

		public void CalcAchievement(int world, int chapter)
		{
			if (world == -1)
			{
				return;
			}
			ChapterData chapter2 = PlayData.Get().GetChapter(world, chapter);
			if (chapter2 == null)
			{
				return;
			}
			ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(world, chapter);
			if (chapter2.lvs.Count == chapterConfig.LevelCount)
			{
				PutAchievement(AchievementType.CompeletedChapter, new ScheduleData(world, chapter, 0));
				if (chapter2.lvs.Count((LevelData e) => e.Star >= 3) == chapter2.lvs.Count)
				{
					PutAchievement(AchievementType.CollectedAllStarsInChapter, new ScheduleData(world, chapter, 0));
				}
			}
		}

		public void PutConfig(AchievementConfig[] configs)
		{
			if (AchievementDatas == null || AchievementDatas.Count == 0)
			{
				AchievementDatas = new List<AchievementInfo>();
			}
			foreach (AchievementConfig config in configs)
			{
				AchievementInfo achievementInfo = AchievementDatas.Find((AchievementInfo e) => e.identifier == config.identifier);
				if (achievementInfo == null)
				{
					achievementInfo = new AchievementInfo();
					AchievementDatas.Add(achievementInfo);
				}
				achievementInfo.PutConfig(config);
			}
			List<string> identifiers = (from e in configs
				select e.identifier).ToList();
			AchievementInfo[] array = (from e in AchievementDatas
				where !identifiers.Contains(e.identifier)
				select e).ToArray();
			if (array.Length > 0)
			{
				AchievementInfo[] array2 = array;
				foreach (AchievementInfo item in array2)
				{
					AchievementDatas.Remove(item);
				}
			}
		}

		public void DoAchievement(AchievementType type, int delta = 1)
		{
			foreach (AchievementInfo achievementData in AchievementDatas)
			{
				if (achievementData.GetConfig().achievementType == type)
				{
					achievementData.DoAchievement(delta);
				}
			}
		}

		public void PutAchievement(AchievementType type, ScheduleData schedule)
		{
			foreach (AchievementInfo achievementData in AchievementDatas)
			{
				if (achievementData.GetConfig().achievementType == type)
				{
					achievementData.DoAchievement(schedule);
					if (achievementData.IsComplete() && !achievementData.IsTips)
					{
						DOChanged();
					}
				}
			}
		}

		public void PutAchievement(AchievementType type, int count)
		{
			foreach (AchievementInfo achievementData in AchievementDatas)
			{
				if (achievementData.GetConfig().achievementType == type)
				{
					achievementData.DoAchievement(count);
					if (achievementData.IsComplete() && !achievementData.IsTips)
					{
						DOChanged();
					}
				}
			}
		}

		public int GetNeedTipsAchievementCount()
		{
			return (from achievement in AchievementDatas
				where !achievement.IsTips && achievement.IsComplete()
				select achievement).Count();
		}
	}
}
