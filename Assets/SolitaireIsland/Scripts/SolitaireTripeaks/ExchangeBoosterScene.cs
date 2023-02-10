using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ExchangeBoosterScene : SoundScene
	{
		public Image Icon;

		public Text Title;

		public Text Des;

		public Button CoinButton;

		public Button BoosterButton;

		public void OnStart(BoosterType boosterType, UnityAction unityAction)
		{
			Title.text = AppearNodeConfig.Get().GetBoosterTitle(boosterType);
			Des.text = AppearNodeConfig.Get().GetBoosterDescription(boosterType);
			Icon.sprite = AppearNodeConfig.Get().GetBoosterSprite(boosterType);
			BoosterButton.gameObject.SetActive(PackData.Get().GetCommodity(BoosterType.RandomBooster).GetTotal() > 0);
			CoinButton.gameObject.SetActive(!BoosterButton.gameObject.activeSelf);
			CoinButton.onClick.AddListener(delegate
			{
				if (SessionData.Get().UseCommodity(BoosterType.Coins, 6000L, $"{boosterType}"))
				{
					AudioUtility.GetSound().Play("Audios/buy_booster.mp3");
					SingletonClass<MySceneManager>.Get().Close();
					SessionData.Get().PutCommodity(boosterType, CommoditySource.Buy, 1L);
					PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
					{
						new PurchasingCommodity
						{
							boosterType = boosterType,
							count = 1
						}
					}, delegate
					{
						SaleData.Get().PutStoreSale(boosterType);
						if (unityAction != null)
						{
							unityAction();
						}
					});
				}
				else if (PlayScene.Get() == null)
				{
					StoreScene.ShowOutofCoinsInLevelScene();
				}
				else
				{
					StoreScene.ShowOutofCoins();
				}
			});
			BoosterButton.onClick.AddListener(delegate
			{
				if (SessionData.Get().UseCommodity(BoosterType.RandomBooster, 1L, $"{boosterType}"))
				{
					AudioUtility.GetSound().Play("Audios/buy_booster.mp3");
					SingletonClass<MySceneManager>.Get().Close();
					SessionData.Get().PutCommodity(boosterType, CommoditySource.Buy, 1L);
					PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
					{
						new PurchasingCommodity
						{
							boosterType = boosterType,
							count = 1
						}
					}, delegate
					{
						SaleData.Get().PutStoreSale(boosterType);
						if (unityAction != null)
						{
							unityAction();
						}
					});
				}
				else
				{
					TipPopupNoIconScene.ShowOutOfSpecialActivityNumbers(AppearNodeConfig.Get().GetBoosterQuestTitle(BoosterType.RandomBooster));
				}
			});
		}
	}
}
