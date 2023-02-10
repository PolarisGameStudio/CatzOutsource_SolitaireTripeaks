using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ShellExchangeUI : MonoBehaviour
	{
		public Text BoosterNumber;

		public Image Icon;

		public Button ExchangeButton;

		public void OnStart(BoosterType boosterType)
		{
			Icon.sprite = AppearNodeConfig.Get().GetBoosterSprite(boosterType);
			ExchangeButton.onClick.AddListener(delegate
			{
				if (SessionData.Get().UseCommodity(BoosterType.RandomBooster, 1L))
				{
					AudioUtility.GetSound().Play("Audios/buy_booster.mp3");
					SessionData.Get().PutCommodity(boosterType, CommoditySource.Buy, 1L);
					BoosterNumber.text = $"OWN: {PackData.Get().GetCommodity(boosterType).GetTotal()}";
					PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
					{
						new PurchasingCommodity
						{
							boosterType = boosterType,
							count = 1
						}
					});
				}
				else
				{
					TipPopupNoIconScene.ShowOutOfSpecialActivityNumbers(AppearNodeConfig.Get().GetBoosterQuestTitle(BoosterType.RandomBooster));
				}
			});
			BoosterNumber.text = $"OWN: {PackData.Get().GetCommodity(boosterType).GetTotal()}";
		}
	}
}
