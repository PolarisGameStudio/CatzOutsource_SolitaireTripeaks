using Nightingale.Utilitys;
using System;
using UnityEngine.Purchasing;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PurchasingInfo
	{
		public string id;

		public string inStore;

		public ProductType productType;

		public decimal GetLocalizedPrice()
		{
			return SingletonBehaviour<UnityPurchasingHelper>.Get().GetProductMetadata(id)?.localizedPrice ?? 0m;
		}

		public string GetLocalizedPriceString()
		{
			ProductMetadata productMetadata = SingletonBehaviour<UnityPurchasingHelper>.Get().GetProductMetadata(id);
			if (productMetadata == null)
			{
				return "...";
			}
			return productMetadata.localizedPriceString;
		}
	}
}
