using Nightingale;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PlayData
	{
		public int playWorld;

		public List<WorldData> worlds;

		public PlayData()
		{
			worlds = new List<WorldData>();
		}

		public static PlayData Get()
		{
			if (SolitaireTripeaksData.Get().Play == null)
			{
				SolitaireTripeaksData.Get().Play = new PlayData();
			}
			return SolitaireTripeaksData.Get().Play;
		}

		public int GetStars()
		{
			int num = 0;
			foreach (WorldData world in worlds)
			{
				foreach (ChapterData chapter in world.chapters)
				{
					num += chapter.lvs.Sum((LevelData e) => e.Star);
				}
			}
			return num;
		}

		public int GetStars(int world)
		{
			int num = 0;
			WorldData worldData = GetWorldData(world);
			if (worldData != null)
			{
				foreach (ChapterData chapter in worldData.chapters)
				{
					num += chapter.lvs.Sum((LevelData e) => e.Star);
				}
				return num;
			}
			return num;
		}

		public ScheduleData GetPlayScheduleData()
		{
			WorldData worldData = GetWorldData(playWorld);
			if (worldData == null)
			{
				return new ScheduleData(playWorld, 0, 0);
			}
			if (worldData.chapters.Count > worldData.playChapter)
			{
				return new ScheduleData(playWorld, worldData.playChapter, worldData.chapters[worldData.playChapter].playLevel);
			}
			return new ScheduleData(playWorld, worldData.playChapter, 0);
		}

		public ScheduleData GetBestScheduleData()
		{
			if (worlds.Count > 0)
			{
				WorldData worldData = (from e in worlds
					orderby e.world descending
					select e).ElementAt(0);
				if (worldData.chapters.Count > 0)
				{
					ChapterData chapterData = worldData.chapters[worldData.chapters.Count - 1];
					return new ScheduleData(worldData.world, worldData.chapters.Count - 1, chapterData.lvs.Count - 1);
				}
				return new ScheduleData(worldData.world, 0, 0);
			}
			return default(ScheduleData);
		}

		public int GetMax()
		{
			if (UniverseConfig.Get() == null)
			{
				return 0;
			}
			return UniverseConfig.Get().GetLevels(GetMaxPlayScheduleData()) + 1;
		}

		public bool MustPlayMasterLevel()
		{
			foreach (ScheduleData allScheduleData in UniverseConfig.Get().GetAllScheduleDatas())
			{
				if (!HasLevelData(allScheduleData))
				{
					return false;
				}
			}
			WorldConfig worldConfig = SingletonClass<AAOConfig>.Get().GetWorldConfig(-1);
			if (worldConfig == null)
			{
				return false;
			}
			foreach (Point point in worldConfig.GetPoints())
			{
				if (!HasLevelData(-1, point.X, point.Y))
				{
					return true;
				}
			}
			return false;
		}

		public int GetMaxMasterLevels()
		{
			WorldData worldData = worlds.Find((WorldData e) => e.world == -1);
			if (worldData == null)
			{
				return 0;
			}
			ExpertWorldConfig worldConfig = SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig();
			if (worldConfig == null)
			{
				return worldData.GetLevels() + 1;
			}
			return Mathf.Min(worldData.GetLevels() + 1, worldConfig.GetPoints().Count);
		}

		public ScheduleData GetMaxPlayInPlayWorld()
		{
			WorldData worldData = worlds.Find((WorldData e) => e.world == playWorld);
			if (worldData != null && worldData.chapters.Count > 0)
			{
				ChapterData chapterData = worldData.chapters[worldData.chapters.Count - 1];
				ScheduleData scheduleData = new ScheduleData(worldData.world, worldData.chapters.Count - 1, chapterData.lvs.Count - 1);
				ScheduleData nextScheduleData = UniverseConfig.Get().GetNextScheduleData(scheduleData);
				if (nextScheduleData.world == playWorld)
				{
					return nextScheduleData;
				}
				return scheduleData;
			}
			return new ScheduleData(playWorld, 0, 0);
		}

		public ScheduleData GetMaxPlayScheduleData()
		{
			if (UniverseConfig.Get() == null)
			{
				return ScheduleData.Empty;
			}
			ScheduleData bestScheduleData = GetBestScheduleData();
			if (!HasLevelData(bestScheduleData))
			{
				return bestScheduleData;
			}
			ScheduleData nextScheduleData = UniverseConfig.Get().GetNextScheduleData(bestScheduleData);
			if (nextScheduleData.Equals(ScheduleData.Empty))
			{
				return bestScheduleData;
			}
			return nextScheduleData;
		}

		public WorldData PutWorldData(int world)
		{
			WorldData worldData = GetWorldData(world);
			if (worldData == null)
			{
				WorldData worldData2 = new WorldData();
				worldData2.world = world;
				worldData = worldData2;
				worlds.Add(worldData);
				worlds = (from e in worlds
					orderby e.world
					select e).ToList();
			}
			return worldData;
		}

		public WorldData GetWorldData(int world)
		{
			return worlds.Find((WorldData e) => e.world == world);
		}

		public RecordDataType PutLevelData(int world, int chapter, int level, LevelData levelData = null)
		{
			if (levelData == null)
			{
				levelData = new LevelData();
			}
			WorldData worldData = PutWorldData(world);
			return worldData.PutData(chapter, level, levelData);
		}

		public RecordDataType PutLevelData(ScheduleData scheduleData, LevelData levelData)
		{
			return PutLevelData(scheduleData.world, scheduleData.chapter, scheduleData.level, levelData);
		}

		public LevelData GetLevelData(int world, int chapter, int level)
		{
			return GetWorldData(world)?.GetData(chapter)?.GetData(level);
		}

		public LevelData GetLevelData(ScheduleData scheduleData)
		{
			return GetLevelData(scheduleData.world, scheduleData.chapter, scheduleData.level);
		}

		public bool HasLevelData(int world, int chapter, int level)
		{
			return GetLevelData(world, chapter, level) != null;
		}

		public bool HasLevelData(ScheduleData scheduleData)
		{
			return GetLevelData(scheduleData.world, scheduleData.chapter, scheduleData.level) != null;
		}

		public bool HasThanLevelData(int world, int chapter, int level)
		{
			return HasThanLevelData(new ScheduleData(world, chapter, level));
		}

		public bool HasThanLevelData(ScheduleData scheduleData)
		{
			return GetBestScheduleData().Than(scheduleData);
		}

		public bool IsCompleted(ScheduleData scheduleData)
		{
			LevelData levelData = GetLevelData(scheduleData.world, scheduleData.chapter, scheduleData.level);
			if (levelData == null)
			{
				return false;
			}
			return levelData.Star == 3;
		}

		public ChapterData GetChapter(int world, int chapter)
		{
			return GetWorldData(world)?.GetData(chapter);
		}

		public void SetPlayScheduleData(ScheduleData scheduleData)
		{
			playWorld = scheduleData.world;
			WorldData worldData = GetWorldData(playWorld);
			if (worldData != null)
			{
				worldData.playChapter = scheduleData.chapter;
				ChapterData chapter = GetChapter(playWorld, worldData.playChapter);
				if (chapter != null)
				{
					chapter.playLevel = scheduleData.level;
				}
			}
		}

		public List<ScheduleData> GetPlayScheduleDatas()
		{
			List<ScheduleData> list = new List<ScheduleData>();
			List<WorldConfig> list2 = UniverseConfig.Get().worlds;
			for (int i = 0; i < list2.Count; i++)
			{
				for (int j = 0; j < list2[i].chapters.Count; j++)
				{
					for (int k = 0; k < list2[i].chapters[j].LevelCount; k++)
					{
						ScheduleData scheduleData = new ScheduleData(i, j, k);
						if (PlayScheduleData(scheduleData))
						{
							list.Add(scheduleData);
						}
					}
				}
			}
			return list;
		}

		public int GetPlayChapterNumbers()
		{
			try
			{
				int num = 0;
				List<WorldConfig> list = UniverseConfig.Get().worlds;
				for (int i = 0; i < list.Count; i++)
				{
					for (int j = 0; j < list[i].chapters.Count; j++)
					{
						if (PlayScheduleData(new ScheduleData(i, j, 0)))
						{
							num++;
						}
					}
				}
				return num;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public bool IsLock(int world)
		{
			if (GetWorldData(world) != null)
			{
				return false;
			}
			if (world == 0)
			{
				return false;
			}
			ScheduleData preScheduleData = UniverseConfig.Get().GetPreScheduleData(new ScheduleData(world, 0, 0));
			return GetLevelData(preScheduleData.world, preScheduleData.chapter, preScheduleData.level) == null;
		}

		public bool IsLock(int world, int chapter)
		{
			if (HasLevelData(world, chapter, 0))
			{
				return false;
			}
			if (IsLock(world))
			{
				return true;
			}
			if (chapter == 0)
			{
				return false;
			}
			ScheduleData preScheduleData = UniverseConfig.Get().GetPreScheduleData(new ScheduleData(world, chapter, 0));
			return GetLevelData(preScheduleData.world, preScheduleData.chapter, preScheduleData.level) == null;
		}

		public int GetLastChapter(int world)
		{
			List<ChapterConfig> chapters = UniverseConfig.Get().GetWorldConfig(world).chapters;
			for (int num = chapters.Count - 1; num >= 0; num--)
			{
				if (!IsLock(world, num))
				{
					return num;
				}
			}
			return 0;
		}

		public bool HasCompleted(int world)
		{
			List<WorldConfig> list = UniverseConfig.Get().worlds;
			List<ChapterConfig> chapters = list[world].chapters;
			int chapter = chapters.Count - 1;
			int level = chapters[chapters.Count - 1].LevelCount - 1;
			return HasLevelData(world, chapter, level);
		}

		public bool PlayScheduleData(ScheduleData schedule)
		{
			if (schedule.Equals(default(ScheduleData)))
			{
				return true;
			}
			if (HasLevelData(schedule))
			{
				return true;
			}
			schedule = UniverseConfig.Get().GetPreScheduleData(schedule);
			if (HasLevelData(schedule))
			{
				return true;
			}
			return false;
		}
	}
}
