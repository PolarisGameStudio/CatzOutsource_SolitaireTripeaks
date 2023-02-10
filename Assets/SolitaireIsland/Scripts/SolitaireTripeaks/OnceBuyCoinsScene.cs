using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OnceBuyCoinsScene : BaseScene
	{
		public PurchasingPackage purchasingPackage;

		public Text PriceLabel;

		public Button BuyButton;

		public Button CloseButton;

		public static bool TryShow()
		{
			if (AuxiliaryData.Get().SpecialSaleReward)
			{
				return false;
			}
			int num = SingletonClass<AAOConfig>.Get().GetLevel() + 1;
			if (num >= 9 && num <= 15 && !PlayData.Get().HasLevelData(0, 0, 14))
			{
				SingletonClass<MySceneManager>.Get().Popup<OnceBuyCoinsScene>("Scenes/Pops/SpecialSalePopup");
				return true;
			}
			return false;
		}

		private void Awake()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().Insert(PurchasingSuccess);
			PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
			BuyButton.onClick.AddListener(delegate
			{
				purchasingPackage.Type = "OnceBuy";
				purchasingPackage.Content = "OnceBuy";
				SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				StoreScene.ShowStore();
			});
		}

		protected override void OnDestroy()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingSuccess);
			base.OnDestroy();
		}

		private void PurchasingSuccess(string transactionID, PurchasingPackage package)
		{
			AuxiliaryData.Get().SpecialSaleReward = true;
			SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
		}
	}
}
