using com.F4A.MobileThird;
using Newtonsoft.Json;
using Nightingale.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class LevelRetrunCoinConfig
	{
		public int LevelTicketCoins;

		public int CompletionCoin;

		public int LimitTime;

		public int TimeRewardReferenceCoin;

		public int TimeRewardMaxLimitCoin;

		public List<int> StreaksRewardCoin;

		public List<int> StreaksFormulaParameters;

		public List<int> StreaksNodeRewardCoin;

		[JsonIgnore]
		private static Dictionary<ScheduleData, LevelRetrunCoinConfig> hands = new Dictionary<ScheduleData, LevelRetrunCoinConfig>();

		public static void PutAssetBundle(AssetBundle assetBundle)
		{
			hands.Clear();

#if ENABLE_DATA_LOCAL
			string[] allAssetNames = assetBundle.GetAllAssetNames();
            string[] array = allAssetNames;
            var temps = new Dictionary<string, LevelRetrunCoinConfig>();
            foreach (string path in array)
            {
                if (Path.GetDirectoryName(path).Contains("coin"))
                {
                    ScheduleData key = ScheduleData.Parse(Path.GetFileNameWithoutExtension(path));
                    var level = assetBundle.Read<LevelRetrunCoinConfig>(path);
                    hands.Add(key, level);
                    temps.Add(path, level);
                }
            }
            DMCFileUtilities.SaveFileByData<Dictionary<string, LevelRetrunCoinConfig>>(temps, "LevelRetrunCoinConfig.json");
#else
            var temps = DMCFileUtilities.LoadContentFromResource<Dictionary<string, LevelRetrunCoinConfig>>("datagame/LevelRetrunCoinConfig.json");
			foreach(var pair in temps)
            {
				ScheduleData key = ScheduleData.Parse(Path.GetFileNameWithoutExtension(pair.Key));
				hands.Add(key, pair.Value);
            }
#endif
			Debug.Log("@LOG LevelRetrunCoinConfig PutAssetBundle count:" + hands.Count);
        }

        public static LevelRetrunCoinConfig Read(ScheduleData scheduleData)
		{
			if (hands.ContainsKey(scheduleData))
			{
				return hands[scheduleData];
			}
			if (hands.ContainsKey(ScheduleData.Empty))
			{
				return hands[ScheduleData.Empty];
			}
			return hands[new ScheduleData(0, 0, 0)];
		}
	}
}
