using Nightingale;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AchievementConfigs
	{
		public AchievementConfig[] Configs;

		private static AchievementConfigs group;

		public static AchievementConfigs Get()
		{
			if (group == null)
			{
				group = JsonUtility.FromJson<AchievementConfigs>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/AchievementConfigs.json"));
			}
			return group;
		}

		public AchievementConfig[] GetConfigs()
		{
			List<AchievementConfig> list = Configs.ToList();
			Point[] array = new Point[28]
			{
				new Point(0, 0),
				new Point(0, 1),
				new Point(0, 2),
				new Point(0, 3),
				new Point(0, 4),
				new Point(0, 5),
				new Point(0, 6),
				new Point(0, 7),
				new Point(0, 8),
				new Point(0, 9),
				new Point(1, 0),
				new Point(1, 1),
				new Point(1, 2),
				new Point(1, 3),
				new Point(1, 4),
				new Point(1, 5),
				new Point(1, 6),
				new Point(1, 7),
				new Point(1, 8),
				new Point(1, 9),
				new Point(2, 0),
				new Point(2, 1),
				new Point(2, 2),
				new Point(2, 3),
				new Point(2, 4),
				new Point(2, 5),
				new Point(2, 6),
				new Point(2, 7)
			};
			for (int i = 0; i < array.Length; i++)
			{
				int x = array[i].X;
				int y = array[i].Y;
				list.Add(new AchievementConfig
				{
					identifier = $"Compeleted Chapter {i}",
					achievementType = AchievementType.CompeletedChapter,
					scheduleData = new ScheduleData(x, y, 0),
					NeedCount = 1,
					AvaterFileName = GetAvatarName(x, y, master: false),
					Title = "Island_completed",
					OrderIndex = 100 + i
				});
				list.Add(new AchievementConfig
				{
					identifier = $"Collected All Stars In Chapter {i}",
					achievementType = AchievementType.CollectedAllStarsInChapter,
					scheduleData = new ScheduleData(x, y, 0),
					NeedCount = 1,
					AvaterFileName = GetAvatarName(x, y, master: true),
					Title = "Island_completed_master",
					OrderIndex = 200 + i
				});
			}
			return list.ToArray();
		}

		private string GetAvatarName(int world, int chapter, bool master)
		{
			if (world == 0 && !master)
			{
				string[] array = new string[10]
				{
					"13",
					"17",
					"15",
					"14",
					"16",
					"33",
					"35",
					"37",
					"39",
					"205"
				};
				return array[chapter];
			}
			if (world == 0 && master)
			{
				string[] array2 = new string[10]
				{
					"28",
					"32",
					"30",
					"29",
					"31",
					"34",
					"36",
					"38",
					"40",
					"master_205"
				};
				return array2[chapter];
			}
			if (master)
			{
				return $"master_{world}_{chapter}";
			}
			return $"{world}_{chapter}";
		}
	}
}
