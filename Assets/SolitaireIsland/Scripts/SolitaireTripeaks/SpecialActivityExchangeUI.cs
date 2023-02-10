using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SpecialActivityExchangeUI : MonoBehaviour
	{
		public Image ExchangeImage;

		public Text ExchangeImageDes;

		public Text ExchangeRewardCount;

		public Text ExchangeNeedCount;

		public Button ExchangeButton;

		public Image IconImage;

		public LabelUI RemainExchangeUI;

		public static SpecialActivityExchangeUI Create(Transform parent, ExchangeConfig config, UnityAction unityAction)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SpecialActivityScene).Name, "UI/SpecialActivityExchangeUI"));
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			SpecialActivityExchangeUI component = gameObject.GetComponent<SpecialActivityExchangeUI>();
			component.OnStart(config, unityAction);
			return component;
		}

		public void OnStart(ExchangeConfig config, UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_SpecialActivity.json");
			if (config.boosterType == BoosterType.Poker)
			{
				PokerThemeConfig poker = PokerThemeGroup.Get().GetPoker(config.count);
				ExchangeImage.gameObject.transform.localScale = new Vector3(0.6f, 0.6f);
				ExchangeImage.sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(AchievementScene).Name, "Sprites/Themes/thumbnail").GetSprite(poker.thumbnail);
				ExchangeRewardCount.text = localizationUtility.GetString("Poker");
				ExchangeImageDes.gameObject.SetActive(value: false);
			}
			else if (config.boosterType == BoosterType.DoubleStar)
			{
				ExchangeImageDes.gameObject.SetActive(value: false);
				ExchangeImage.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(config.boosterType);
				ExchangeRewardCount.text = localizationUtility.GetString("x3 hours");
			}
			else
			{
				ExchangeImageDes.gameObject.SetActive(value: false);
				ExchangeImage.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(config.boosterType);
				ExchangeRewardCount.text = $"x{config.count}";
			}
			ExchangeImage.SetNativeSize();
			ExchangeNeedCount.text = $"{config.need}";
			IconImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetMiniSprite();
			IconImage.SetNativeSize();
			UpdateRemainExchangeUI(config);
			ExchangeButton.onClick.AddListener(delegate
			{
				if (SpecialActivityData.Get().Use(config.need))
				{
					SpecialActivityData.Get().ExhangeBooster(config.boosterType);
					if (config.boosterType == BoosterType.Poker)
					{
						PokerData.Get().PutPoker(config.count);
						SingletonClass<MySceneManager>.Get().Popup<PokerThemeScene>("Scenes/PokerThemeScene", new JoinEffect());
						UnityEngine.Object.Destroy(base.gameObject);
					}
					else
					{
						SessionData.Get().PutCommodity(config.boosterType, CommoditySource.Task, config.count, changed: false);
						PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
						{
							new PurchasingCommodity
							{
								boosterType = config.boosterType,
								count = config.count
							}
						});
						if (config.limit > 0)
						{
							UpdateRemainExchangeUI(config);
							if (config.limit <= SpecialActivityData.Get().GetNumber(config.boosterType))
							{
								UnityEngine.Object.Destroy(base.gameObject);
							}
						}
					}
					if (unityAction != null)
					{
						unityAction();
					}
				}
				else
				{
					TipPopupNoIconScene.ShowOutOfSpecialActivityNumbers(SpecialActivityConfig.Get().Name);
				}
			});
		}

		private void UpdateRemainExchangeUI(ExchangeConfig config)
		{
			if (config.limit > 0)
			{
				int num = config.limit - SpecialActivityData.Get().GetNumber(config.boosterType);
				RemainExchangeUI.SetString(string.Format(LocalizationUtility.Get("Localization_SpecialActivity.json").GetString("Exchange_Remain_Number"), num));
			}
			else
			{
				RemainExchangeUI.SetString(string.Format(LocalizationUtility.Get("Localization_SpecialActivity.json").GetString("Exchange_Remain_Number"), "100+"));
			}
		}
	}
}
