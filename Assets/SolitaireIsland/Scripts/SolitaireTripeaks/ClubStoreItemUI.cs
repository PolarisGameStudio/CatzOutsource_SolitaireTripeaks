using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubStoreItemUI : MonoBehaviour
	{
		public PurchasingPackage purchasingPackage;

		public Text PriceLabel;

		public Text TitleLabel;

		private void Start()
		{
			if (PriceLabel != null)
			{
				PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
			}
			if (!SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				InvokeRepeating("PriceRepeating", 1f, 1f);
			}
		}

		public void Buy()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
		}

		public void SetInfo(ClubStoreItemConfig config)
		{
			TitleLabel.text = config.title;
			purchasingPackage = config.GetPackage();
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
