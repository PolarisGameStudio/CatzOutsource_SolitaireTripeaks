using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class StoreItemUI : MonoBehaviour
	{
		public PurchasingPackage purchasingPackage;

		public Button BuyButton;

		public Text PriceLabel;

		public Text CountLabel;

		public Text OldLabel;

		public string Content;

		private void Start()
		{
			if (CountLabel != null)
			{
				CountLabel.text = $"{purchasingPackage.GetBoosters(BoosterType.Coins):N0}";
			}
			if (OldLabel != null)
			{
				OldLabel.text = $"{purchasingPackage.GetBoosters(BoosterType.Coins) / 2:N0}";
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
				purchasingPackage.Content = Content;
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
