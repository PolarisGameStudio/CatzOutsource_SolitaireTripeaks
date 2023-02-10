using Nightingale;
using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class WorldConfig
	{
		public string name;

		public List<ChapterConfig> chapters;

		public List<Point> GetPoints()
		{
			List<Point> list = new List<Point>();
			for (int i = 0; i < chapters.Count; i++)
			{
				for (int j = 0; j < chapters[i].LevelCount; j++)
				{
					list.Add(new Point(i, j));
				}
			}
			return list;
		}

		public int GetLevel(ScheduleData scheduleData)
		{
			return GetPoints().FindIndex((Point e) => e.X == scheduleData.chapter && e.Y == scheduleData.level);
		}

		public ChapterConfig GetChapterConfig(int chapter)
		{
			if (chapter >= chapters.Count)
			{
				return null;
			}
			return chapters[chapter];
		}

		public ScheduleData GetNext(ScheduleData scheduleData)
		{
			List<Point> points = GetPoints();
			int num = points.FindIndex((Point e) => e.X == scheduleData.chapter && e.Y == scheduleData.level);
			num++;
			if (num > points.Count - 1)
			{
				return ScheduleData.Empty;
			}
			return new ScheduleData(scheduleData.world, points[num].X, points[num].Y);
		}

		public ScheduleData GetPre(ScheduleData scheduleData)
		{
			List<Point> points = GetPoints();
			int num = points.FindIndex((Point e) => e.X == scheduleData.chapter && e.Y == scheduleData.level);
			num--;
			if (num < 0)
			{
				return ScheduleData.Empty;
			}
			return new ScheduleData(scheduleData.world, points[num].X, points[num].Y);
		}

		public virtual LevelConfig GetLevelConfig(ScheduleData scheduleData)
		{
			return GetChapterConfig(scheduleData.chapter).GetLevelConfig(scheduleData.level);
		}

		public virtual LevelRetrunCoinConfig GetLevelRetrunCoinConfig(ScheduleData scheduleData)
		{
			return LevelRetrunCoinConfig.Read(scheduleData);
		}
	}
}
