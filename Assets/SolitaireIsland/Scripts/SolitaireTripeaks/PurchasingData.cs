using Nightingale.Extensions;
using Nightingale.Utilitys;
using System;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PurchasingData : SingletonData<PurchasingData>
	{
		public string lastPackage;

		public bool up;

		public PurchasingData()
		{
			lastPackage = "yy_store_coin_1";
			up = true;
		}

		public void LevelDown()
		{
			if (!string.IsNullOrEmpty(lastPackage))
			{
				(from e in UnityPurchasingConfig.Get().infos
					orderby e.GetLocalizedPrice() descending
					select e).FirstOrDefault((PurchasingInfo e) => e.GetLocalizedPrice() < UnityPurchasingConfig.Get().GetLocalizedPrice(lastPackage)).HasVaule(delegate(PurchasingInfo e)
				{
					lastPackage = e.id;
				});
			}
			up = false;
			FlushData();
		}

		public string Getlevel()
		{
			if (string.IsNullOrEmpty(lastPackage))
			{
				if (StatisticsData.Get().Purchaseds == null || StatisticsData.Get().Purchaseds.Count == 0)
				{
					return (from e in UnityPurchasingConfig.Get().infos
						orderby e.id
						select e).First().id;
				}
				return StatisticsData.Get().Purchaseds.Last().id;
			}
			return lastPackage;
		}

		public void PutBuySuccessPackage(PurchasingPackage package)
		{
			if (UnityPurchasingConfig.Get().GetLocalizedPrice(package.id) > UnityPurchasingConfig.Get().GetLocalizedPrice(lastPackage))
			{
				lastPackage = package.id;
				(from e in UnityPurchasingConfig.Get().infos
					orderby e.GetLocalizedPrice()
					select e).FirstOrDefault((PurchasingInfo e) => e.GetLocalizedPrice() > UnityPurchasingConfig.Get().GetLocalizedPrice(lastPackage)).HasVaule(delegate(PurchasingInfo e)
				{
					lastPackage = e.id;
				});
			}
			up = true;
			UnityEngine.Debug.Log("本次等级：" + lastPackage);
			FlushData();
		}
	}
}
