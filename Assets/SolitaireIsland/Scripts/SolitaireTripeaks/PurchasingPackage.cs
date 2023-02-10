using System;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PurchasingPackage
	{
		public string id;

		public string Type;

		public string Content;

		public PurchasingCommodity[] commoditys;

		public int GetBoosters(BoosterType boosterType)
		{
			int num = 0;
			PurchasingCommodity[] array = commoditys;
			foreach (PurchasingCommodity purchasingCommodity in array)
			{
				if (purchasingCommodity.boosterType == boosterType)
				{
					num += purchasingCommodity.count;
				}
			}
			return num;
		}

		public bool HasBoosters(BoosterType boosterType)
		{
			return commoditys.Count((PurchasingCommodity e) => e.boosterType == boosterType) > 0;
		}
	}
}
