using com.F4A.MobileThird;
using Nightingale.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class GameConfig
	{
		public bool ShowRateCoins;

		public int ShowRateCount;

		public string Videos;

		public string ExpertVersion;

		private static GameConfig config;

		public static GameConfig Get()
		{
			if (config == null)
			{
				GameConfig gameConfig = new GameConfig();
				gameConfig.ShowRateCoins = false;
				gameConfig.ShowRateCount = 1;
				config = gameConfig;
			}
			return config;
		}

		public static void PutAssetBundle(AssetBundle assetBundle)
		{
			try
			{
#if ENABLE_DATA_LOCAL
				config = assetBundle.Read<GameConfig>("GameConfig.json");
				DMCFileUtilities.SaveFileByData<GameConfig>(config, "GameConfig.json");
#else
				config = DMCFileUtilities.LoadContentFromResource<GameConfig>("datagame/GameConfig.json");
#endif
            }
            catch (Exception)
			{
			}
		}

		public bool HasVideo(int level)
		{
			if (string.IsNullOrEmpty(Videos))
			{
				Videos = "9,11,12,13,14,15,16,20,22,23,25,26,27,28,33,34,36,38,42,43";
			}
			List<string> list = Videos.Split(',').ToList();
			return list.Contains(level.ToString());
		}
	}
}
