using System;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SalePackage
	{
		public string id;

		public SaleCommodity[] commoditys;

		public string GetString()
		{
			string text = string.Empty;
			SaleCommodity[] array = commoditys;
			foreach (SaleCommodity saleCommodity in array)
			{
				text += $"{saleCommodity.boosterType}_{saleCommodity.saleCount}_";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public PurchasingPackage ToPurchasingPackage()
		{
			PurchasingPackage purchasingPackage = new PurchasingPackage();
			purchasingPackage.id = id;
			purchasingPackage.Type = "Sale";
			PurchasingPackage purchasingPackage2 = purchasingPackage;
			purchasingPackage2.commoditys = (from commodity in commoditys
				select new PurchasingCommodity
				{
					boosterType = ((commodity.boosterType != BoosterType.None) ? commodity.boosterType : BoosterType.RandomBooster),
					count = commodity.saleCount
				}).ToArray();
			return purchasingPackage2;
		}

		public SalePackage Clone()
		{
			SalePackage salePackage = new SalePackage();
			salePackage.id = id;
			salePackage.commoditys = (from c in commoditys
				select new SaleCommodity
				{
					boosterType = c.boosterType,
					saleCount = c.saleCount,
					moreValue = c.moreValue,
					normalCount = c.normalCount
				}).ToArray();
			return salePackage;
		}
	}
}
