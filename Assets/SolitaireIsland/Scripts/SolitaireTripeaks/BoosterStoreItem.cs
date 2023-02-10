using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[RequireComponent(typeof(Button))]
	public class BoosterStoreItem : MonoBehaviour
	{
		public BoosterType boosterType;

		public PurchasingPackage purchasingPackage;

		public Transform NumberTransform;

		public Transform PriceTransform;

		public Text CountLabel;

		public Text PriceLabel;

		public string Content;

		private Button button;

		private void UpdateNumberUI(CommoditySource commoditySource)
		{
			long total = PackData.Get().GetCommodity(boosterType).GetTotal();
			CountLabel.text = $"{total}";
			NumberTransform.gameObject.SetActive(total > 0);
			PriceTransform.gameObject.SetActive(total <= 0);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
			PackData.Get().GetCommodity(boosterType).OnChanged.RemoveListener(UpdateNumberUI);
			SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(Purchased);
		}

		private void Awake()
		{
			UpdateNumberUI(CommoditySource.None);
			PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
			if (!SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				InvokeRepeating("PriceRepeating", 1f, 1f);
			}
			PackData.Get().GetCommodity(boosterType).OnChanged.AddListener(UpdateNumberUI);
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
		}

		private void PriceRepeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
				CancelInvoke("PriceRepeating");
			}
		}

		private void Purchased(string transactionID, PurchasingPackage package)
		{
			if (PlayDesk.Get() != null && PlayDesk.Get().IsPlaying && !PlayDesk.Get().IsGameOver && package.commoditys.Length == 1 && package.commoditys[0].boosterType == boosterType)
			{
				UseBooster();
				SaleData.Get().PutStoreSale(package);
			}
		}

		private void OnClick()
		{
			if (PlayDesk.Get().IsPlaying && !UseBooster())
			{
				AudioUtility.GetSound().Play("Audios/button.mp3");
				SingletonBehaviour<UnityPurchasingHelper>.Get().Append(Purchased);
				BuyBooster();
			}
		}

		private bool UseBooster()
		{
			if (SessionData.Get().UseCommodity(boosterType, 1L))
			{
				AudioUtility.GetSound().Play("Audios/Use_Wild.mp3");
				CardType cardType = CardType.Wild;
				switch (boosterType)
				{
				case BoosterType.Wild:
					AchievementData.Get().DoAchievement(AchievementType.UseWild);
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.UseWild);
					break;
				case BoosterType.Rocket:
					cardType = CardType.SellRocket;
					AchievementData.Get().DoAchievement(AchievementType.UseRocket);
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.UseRocket);
					break;
				}
				SingletonClass<OnceGameData>.Get().Use(boosterType);
				OperatingHelper.Get().ClearStep();
				CardConfig config = default(CardConfig);
				config.CardType = cardType;
				BaseCard baseCard = SolitaireVariationExtensions.GetBaseCard(config);
				baseCard.transform.position = base.transform.position;
				HandCardSystem.Get().AppendRightCardNormal(baseCard);
				return true;
			}
			return false;
		}

		private void BuyBooster()
		{
			purchasingPackage.Type = "Store";
			purchasingPackage.Content = Content;
			SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
		}
	}
}
