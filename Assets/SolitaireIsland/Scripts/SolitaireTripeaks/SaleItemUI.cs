using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SaleItemUI : MonoBehaviour
	{
		public Button BuyButton;

		public Text PriceLabel;

		public Text SaleOffLabel;

		public List<SaleStoreItem> saleStoreItems;

		private string id;

		private void PriceRepeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(id);
				CancelInvoke("PriceRepeating");
			}
		}

		public void SetPurchasingPackage(SalePackage salePackage, string content)
		{
			id = salePackage.id;
			BuyButton.onClick.AddListener(delegate
			{
				PurchasingPackage purchasingPackage = salePackage.ToPurchasingPackage();
				purchasingPackage.Content = content;
				SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
			});
			PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(salePackage.id);
			if (!SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				InvokeRepeating("PriceRepeating", 1f, 1f);
			}
			List<SaleCommodity> list = salePackage.commoditys.ToList();
			foreach (SaleStoreItem v in saleStoreItems)
			{
				SaleCommodity saleCommodity = list.FirstOrDefault((SaleCommodity e) => (e.boosterType >= BoosterType.RandomBooster && v.boosterType >= BoosterType.RandomBooster) || e.boosterType == v.boosterType);
				if (saleCommodity == null)
				{
					v.IconImage.transform.parent.gameObject.SetActive(value: false);
				}
				else
				{
					list.Remove(saleCommodity);
					v.UpdateInfo(saleCommodity);
					if (saleCommodity.boosterType != BoosterType.Coins && SaleOffLabel != null)
					{
						SaleOffLabel.text = $"{saleCommodity.moreValue}%";
					}
				}
			}
		}
	}
}
