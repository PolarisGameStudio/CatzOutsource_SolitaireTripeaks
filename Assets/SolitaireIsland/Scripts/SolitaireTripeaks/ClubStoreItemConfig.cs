using Nightingale.Utilitys;
using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ClubStoreItemConfig
	{
		public string title;

		public string icon;

		public string id;

		public List<PurchasingCommodity> commoditys;

		public PurchasingCommodity[] receives;

		public PurchasingPackage GetPackage()
		{
			PurchasingPackage purchasingPackage = new PurchasingPackage();
			purchasingPackage.id = id;
			purchasingPackage.Type = "Club";
			purchasingPackage.commoditys = new PurchasingCommodity[1]
			{
				new PurchasingCommodity
				{
					boosterType = BoosterType.ClubStore,
					count = ClubStoreConfig.Get().configs.IndexOf(this)
				}
			};
			return purchasingPackage;
		}

		public string GetGiftId()
		{
			return SingletonBehaviour<ClubSystemHelper>.Get().GetGiftId(commoditys);
		}
	}
}
