using Nightingale.Utilitys;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class WorldHandConfig
	{
		private static WorldHandConfig config;

		private Dictionary<int, int> levels = new Dictionary<int, int>();

		public static WorldHandConfig Get()
		{
			if (config == null)
			{
				string text = SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/WorldHandConfig");
				WorldHandConfig worldHandConfig = new WorldHandConfig();
				worldHandConfig.levels = new Dictionary<int, int>();
				config = worldHandConfig;
				string[] array = text.Split('\n');
				foreach (string text2 in array)
				{
					string[] array2 = text2.Split(',');
					if (array2.Length == 2)
					{
						config.levels.Add(int.Parse(array2[0]), int.Parse(array2[1]));
					}
				}
			}
			return config;
		}

		public int GetHand(ScheduleData schedule)
		{
			int key = SingletonClass<AAOConfig>.Get().GetLevel(schedule) + 1;
			if (levels.ContainsKey(key))
			{
				return levels[key];
			}
			return 0;
		}
	}
}
