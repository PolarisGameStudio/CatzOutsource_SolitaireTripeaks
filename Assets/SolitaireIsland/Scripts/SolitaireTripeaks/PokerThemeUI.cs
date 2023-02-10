using Nightingale.Localization;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PokerThemeUI : MonoBehaviour
	{
		public Image ThemeThumtil;

		public Button ThemeButton;

		public Text ButtonLabel;

		public Text TitleLabel;

		public LabelUI WasLabelUI;

		public Image LockImage;

		public Image Background;

		private PokerThemeConfig Config;

		private void Start()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().Append(PurchasingCompleted);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingCompleted);
		}

		private void PurchasingCompleted(string transactionID, PurchasingPackage package)
		{
			PokerThemeUI[] componentsInChildren = base.transform.parent.GetComponentsInChildren<PokerThemeUI>();
			PokerThemeUI[] array = componentsInChildren;
			foreach (PokerThemeUI pokerThemeUI in array)
			{
				pokerThemeUI.UpdateThemeUI();
			}
		}

		private void PriceRepeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				ButtonLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(Config.purchasId);
				WasLabelUI.SetString(UnityPurchasingConfig.Get().GetLocalizedPriceString(Config.waspurchasId));
				CancelInvoke("PriceRepeating");
			}
		}

		public bool UpdateThemeUI()
		{
			CancelInvoke("PriceRepeating");
			WasLabelUI.gameObject.SetActive(value: false);
			if (PokerThemeGroup.Get().GetPoker().identifier == Config.identifier)
			{
				Background.color = new Color32(byte.MaxValue, 251, 122, byte.MaxValue);
				LockImage.gameObject.SetActive(value: false);
				ButtonLabel.text = LocalizationUtility.Get("Localization_poker.json").GetString("theme_use_ing");
				ThemeButton.image.enabled = false;
				return true;
			}
			Background.color = Color.white;
			ThemeButton.image.enabled = true;
			if (Config.IsCanUse())
			{
				LockImage.gameObject.SetActive(value: false);
				ButtonLabel.text = LocalizationUtility.Get("Localization_poker.json").GetString("btn_use");
				ThemeButton.onClick.RemoveAllListeners();
				ThemeButton.onClick.AddListener(delegate
				{
					PokerData.Get().currentUsePoker = Config.identifier;
					PokerThemeGroup.Get().ChangePoker();
					PokerThemeUI[] componentsInChildren = base.transform.parent.GetComponentsInChildren<PokerThemeUI>();
					PokerThemeUI[] array = componentsInChildren;
					foreach (PokerThemeUI pokerThemeUI in array)
					{
						pokerThemeUI.UpdateThemeUI();
					}
				});
			}
			else if (Config.GetThemeType() == ThemeType.Chapter)
			{
				LockImage.gameObject.SetActive(value: true);
				ButtonLabel.text = LocalizationUtility.Get("Localization_poker.json").GetString("theme_lock");
				ThemeButton.onClick.RemoveAllListeners();
				ThemeButton.onClick.AddListener(delegate
				{
					TipPopupNoIconScene.ShowTitleDescription(LocalizationUtility.Get("Localization_poker.json").GetString("theme_lock_title"), Config.GetDescription());
				});
			}
			else
			{
				LockImage.gameObject.SetActive(value: false);
				ButtonLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(Config.purchasId);
				WasLabelUI.gameObject.SetActive(value: true);
				WasLabelUI.SetString(UnityPurchasingConfig.Get().GetLocalizedPriceString(Config.waspurchasId));
				if (!SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
				{
					InvokeRepeating("PriceRepeating", 1f, 1f);
				}
				ThemeButton.onClick.RemoveAllListeners();
				ThemeButton.onClick.AddListener(delegate
				{
					SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(new PurchasingPackage
					{
						id = Config.purchasId,
						Type = "Poker",
						commoditys = new PurchasingCommodity[1]
						{
							new PurchasingCommodity
							{
								boosterType = BoosterType.Poker,
								count = Config.Index
							}
						}
					});
				});
			}
			return false;
		}

		public bool SetPokerThemeConfig(PokerThemeConfig config)
		{
			Config = config;
			TitleLabel.text = Config.name;
			ThemeThumtil.sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(AchievementScene).Name, "Sprites/Themes/thumbnail").GetSprite(Config.thumbnail);
			ThemeThumtil.SetNativeSize();
			return UpdateThemeUI();
		}
	}
}
