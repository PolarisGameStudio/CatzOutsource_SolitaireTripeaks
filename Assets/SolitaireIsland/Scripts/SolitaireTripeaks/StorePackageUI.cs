using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class StorePackageUI : MonoBehaviour
	{
		public PurchasingPackage purchasingPackage;

		public Image IconImage;

		public Text PriceLabel;

		public Button BuyButton;

		public Transform CoinTranform;

		public Transform OtherTranform;

		private void Start()
		{
			PurchasingCommodity[] commoditys = purchasingPackage.commoditys;
			foreach (PurchasingCommodity purchasingCommodity in commoditys)
			{
				if (purchasingCommodity.boosterType == BoosterType.Coins)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(StoreScene).Name, "UI/Stores/CoinPackItem"));
					gameObject.transform.SetParent(CoinTranform, worldPositionStays: false);
					gameObject.GetComponent<StorePackageItem>().SetNumber(purchasingCommodity);
				}
				else
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(StoreScene).Name, "UI/Stores/OtherPackItem"));
					gameObject2.transform.SetParent(OtherTranform, worldPositionStays: false);
					gameObject2.GetComponent<StorePackageItem>().SetNumber(purchasingCommodity);
				}
			}
			if (PriceLabel != null)
			{
				PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
			}
			if (!SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				InvokeRepeating("PriceRepeating", 1f, 1f);
			}
			BuyButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
			});
		}

		private void PriceRepeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				if (PriceLabel != null)
				{
					PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
				}
				CancelInvoke("PriceRepeating");
			}
		}
	}
}
