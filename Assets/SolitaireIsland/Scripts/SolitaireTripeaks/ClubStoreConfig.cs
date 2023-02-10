using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[CreateAssetMenu(fileName = "ClubStoreConfig.asset", menuName = "Nightingale/Club Store Config", order = 1)]
	public class ClubStoreConfig : ScriptableObject
	{
		public List<ClubStoreItemConfig> configs;

		private static ClubStoreConfig finder;

		public PurchasingCommodity[] GetCommoditysByGiftId(string giftId)
		{
			ClubStoreItemConfig clubStoreItemConfig = configs.Find((ClubStoreItemConfig e) => e.GetGiftId() == giftId);
			if (clubStoreItemConfig == null)
			{
				clubStoreItemConfig = configs[0];
			}
			return clubStoreItemConfig.receives;
		}

		public string GetTitleByGiftId(string giftId)
		{
			ClubStoreItemConfig clubStoreItemConfig = configs.Find((ClubStoreItemConfig e) => e.GetGiftId() == giftId);
			if (clubStoreItemConfig == null)
			{
				clubStoreItemConfig = configs[0];
			}
			return clubStoreItemConfig.title;
		}

		public string GetIconByGiftId(string giftId)
		{
			ClubStoreItemConfig clubStoreItemConfig = configs.Find((ClubStoreItemConfig e) => e.GetGiftId() == giftId);
			if (clubStoreItemConfig == null)
			{
				clubStoreItemConfig = configs[0];
			}
			return clubStoreItemConfig.icon;
		}

		public ClubStoreItemConfig GetConfig(int index)
		{
			if (index > configs.Count - 1)
			{
				return configs[0];
			}
			return configs[index];
		}

		public static ClubStoreConfig Get()
		{
			if (finder == null)
			{
				finder = SingletonBehaviour<LoaderUtility>.Get().GetAsset<ClubStoreConfig>("Configs/ClubStoreConfig");
			}
			return finder;
		}
	}
}
