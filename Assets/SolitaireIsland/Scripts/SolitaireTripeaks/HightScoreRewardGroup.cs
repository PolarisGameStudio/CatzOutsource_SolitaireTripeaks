using com.F4A.MobileThird;
using Nightingale.Extensions;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class HightScoreRewardGroup
	{
		public List<HightScoreRewardConfig> configs;

		private static HightScoreRewardGroup group;

		public HightScoreRewardConfig Find(int rank)
		{
			return configs.Find((HightScoreRewardConfig e) => e.LowRank <= rank && e.BestRank >= rank);
		}

		public int GetCoins(int rank)
		{
			return Find(rank)?.rewardCoins ?? 0;
		}

		public static HightScoreRewardGroup Get()
		{
			if (group == null)
			{
				group = JsonUtility.FromJson<HightScoreRewardGroup>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/HightScoreRewardGroup.json"));
			}
			return group;
		}

		public static void PutAssetBundle(AssetBundle assetBundle)
		{
#if ENABLE_DATA_LOCAL
			group = assetBundle.Read<HightScoreRewardGroup>("HightScoreRewardGroup.json");
            DMCFileUtilities.SaveFileByData(group, "HightScoreRewardGroup.json");
#else
            group = DMCFileUtilities.LoadContentFromResource<HightScoreRewardGroup>("datagame/HightScoreRewardGroup.json");
#endif
        }
    }
}
