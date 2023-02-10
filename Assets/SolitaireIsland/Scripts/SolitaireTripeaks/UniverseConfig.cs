using com.F4A.MobileThird;
using Nightingale.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class UniverseConfig
	{
		public List<WorldConfig> worlds;

		private static UniverseConfig group;

		public static UniverseConfig Get()
		{
			return group;
		}

		public static void PutAssetBundle(AssetBundle assetBundle)
		{
#if ENABLE_DATA_LOCAL
			group = assetBundle.Read<UniverseConfig>("UniverseConfig.json");
			DMCFileUtilities.SaveFileByData<UniverseConfig>(group, "UniverseConfig.json");
#else
            group = DMCFileUtilities.LoadContentFromResource<UniverseConfig>("datagame/UniverseConfig.json");
#endif
        }

        public void Download()
		{
			foreach (WorldConfig world in worlds)
			{
				foreach (ChapterConfig chapter in world.chapters)
				{
					chapter.GetThumbnailDownloadTask();
					chapter.GetDetailsDownloadTask();
				}
			}
		}

		public void DestoryThumbnails()
		{
			foreach (WorldConfig world in group.worlds)
			{
				foreach (ChapterConfig chapter in world.chapters)
				{
					chapter.DestoryThumbnail();
				}
			}
		}

		public void DestoryDetails()
		{
			foreach (WorldConfig world in group.worlds)
			{
				foreach (ChapterConfig chapter in world.chapters)
				{
					chapter.DestoryDetails();
				}
			}
		}

		public int GetAllLevelInWorld(int world)
		{
			List<ScheduleData> allScheduleDatas = GetAllScheduleDatas();
			return allScheduleDatas.Count((ScheduleData e) => e.world == world);
		}

		public WorldConfig GetWorldConfig(int world)
		{
			if (world < worlds.Count && world >= 0)
			{
				return worlds[world];
			}
			return null;
		}

		public ChapterConfig GetChapterConfig(int world, int chapter)
		{
			WorldConfig worldConfig = GetWorldConfig(world);
			if (worldConfig == null)
			{
				return null;
			}
			if (worldConfig.chapters.Count > chapter)
			{
				return worldConfig.chapters[chapter];
			}
			return null;
		}

		public ChapterConfig GetChapterConfig()
		{
			List<ChapterConfig> list = new List<ChapterConfig>();
			foreach (WorldConfig world in worlds)
			{
				list.AddRange(world.chapters);
			}
			return (from e in list
				orderby Guid.NewGuid()
				select e).ToList().Find((ChapterConfig e) => e.IsDetailsAtLocalPath());
		}

		public int GetChapterIndex(int world, int chapter)
		{
			int num = chapter;
			for (int i = 0; i < worlds.Count; i++)
			{
				if (world > i)
				{
					num += worlds[i].chapters.Count;
				}
			}
			return num;
		}

		public int GetLevelInWorld(ScheduleData schedule)
		{
			return (from e in GetAllScheduleDatas()
				where e.world == schedule.world
				select e).ToList().IndexOf(schedule);
		}

		public int GetLevels(ScheduleData schedule)
		{
			List<ScheduleData> allScheduleDatas = GetAllScheduleDatas();
			int num = allScheduleDatas.IndexOf(schedule);
			if (num == -1)
			{
				if (schedule.Equals(ScheduleData.Empty))
				{
					return 0;
				}
				return allScheduleDatas.Count;
			}
			return num;
		}

		public ScheduleData GetScheduleData(int level)
		{
			for (int i = 0; i < worlds.Count; i++)
			{
				List<ChapterConfig> chapters = worlds[i].chapters;
				for (int j = 0; j < chapters.Count; j++)
				{
					if (level > chapters[j].LevelCount)
					{
						level -= chapters[j].LevelCount;
						continue;
					}
					return new ScheduleData(i, j, level - 1);
				}
			}
			return new ScheduleData(0, 0, 0);
		}

		public ScheduleData GetPreScheduleData(ScheduleData schedule)
		{
			List<ScheduleData> allScheduleDatas = GetAllScheduleDatas();
			int num = allScheduleDatas.IndexOf(schedule);
			if (num <= 0)
			{
				return ScheduleData.Empty;
			}
			if (num > allScheduleDatas.Count - 1)
			{
				return ScheduleData.Empty;
			}
			return allScheduleDatas[num - 1];
		}

		public ScheduleData GetNextScheduleData(ScheduleData schedule)
		{
			List<ScheduleData> allScheduleDatas = GetAllScheduleDatas();
			int num = allScheduleDatas.IndexOf(schedule);
			if (num < 0)
			{
				return ScheduleData.Empty;
			}
			if (num >= allScheduleDatas.Count - 1)
			{
				return ScheduleData.Empty;
			}
			return allScheduleDatas[num + 1];
		}

		public List<ScheduleData> GetAllScheduleDatas()
		{
			List<ScheduleData> list = new List<ScheduleData>();
			for (int i = 0; i < worlds.Count; i++)
			{
				for (int j = 0; j < worlds[i].chapters.Count; j++)
				{
					for (int k = 0; k < worlds[i].chapters[j].LevelCount; k++)
					{
						list.Add(new ScheduleData(i, j, k));
					}
				}
			}
			return list;
		}

		public bool HasThumbnailInWorld(int world)
		{
			if (world >= worlds.Count)
			{
				Debug.Log($"@LOG HasThumbnailInWorld world:{world},{worlds.Count}");
				return false;
			}
			Debug.Log($"@LOG HasThumbnailInWorld world:{world},{worlds.Count}");
            List<ChapterConfig> chapters = worlds[world].chapters;
			return chapters[0].IsThumbnailAtLocalPath();
		}

		public bool HasIslandConfigInWorld(int world)
		{
			if (world >= worlds.Count)
			{
				return false;
			}
			List<ChapterConfig> chapters = worlds[world].chapters;
			return chapters[0].IsThumbnailAtLocalPath() && chapters[0].IsDetailsAtLocalPath();
		}
	}
}
