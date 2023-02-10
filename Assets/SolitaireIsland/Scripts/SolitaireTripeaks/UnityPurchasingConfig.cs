using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace SolitaireTripeaks
{
	public class UnityPurchasingConfig : ScriptableObject
	{
		public PurchasingInfo[] infos;

		public StoreType[] Stores;

		private static UnityPurchasingConfig config;

		public static UnityPurchasingConfig Get()
		{
			if (config == null)
			{
				config = SingletonBehaviour<LoaderUtility>.Get().GetAsset<UnityPurchasingConfig>("UnityPurchasingConfig");
			}
			return config;
		}

		public string GetId(string id, int delta)
		{
			List<PurchasingInfo> list = (from e in infos
				orderby e.GetLocalizedPrice()
				select e).ToList();
			int num = list.FindIndex((PurchasingInfo e) => e.id.Equals(id));
			num += delta;
			if (num < 0)
			{
				num = 0;
			}
			if (num > list.Count - 1)
			{
				num = list.Count - 1;
			}
			return list[num].id;
		}

		public PurchasingInfo GetPurchasingInfo(string id)
		{
			PurchasingInfo[] array = infos;
			foreach (PurchasingInfo purchasingInfo in array)
			{
				if (purchasingInfo.id == id)
				{
					return purchasingInfo;
				}
			}
			return new PurchasingInfo();
		}

		public string GetLocalizedPriceString(string id)
		{
			return GetPurchasingInfo(id).GetLocalizedPriceString();
		}

		public decimal GetLocalizedPrice(string id)
		{
			return GetPurchasingInfo(id).GetLocalizedPrice();
		}

		public string GetCountryCode()
		{
			if (infos == null || infos.Length == 0)
			{
				return "Unknow";
			}
			ProductMetadata productMetadata = SingletonBehaviour<UnityPurchasingHelper>.Get().GetProductMetadata(infos[0].id);
			if (productMetadata == null)
			{
				return "Unknow";
			}
			return productMetadata.isoCurrencyCode;
		}
	}
}
