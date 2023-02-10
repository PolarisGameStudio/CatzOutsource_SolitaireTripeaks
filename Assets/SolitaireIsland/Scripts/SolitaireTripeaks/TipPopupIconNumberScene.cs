using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TipPopupIconNumberScene : SoundScene
	{
		public ImageUI RewardUI;

		public Button OKButton;

		public Text TitleLabel;

		public Text DesLabel;

		public Text ButtonLabel;

		public GameObject DoubleGameObject;

		public void OnStart(Sprite icon, int number, string title, string des, string button, UnityAction unityAction = null)
		{
			RewardUI.SetImage(icon);
			RewardUI.SetLabel($"x{number}");
			TitleLabel.text = title;
			DesLabel.text = des;
			ButtonLabel.text = button;
			OKButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowVideoRewardCoins(int coins, UnityAction unityAction = null)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			TipPopupIconNumberScene scene = SingletonClass<MySceneManager>.Get().Popup<TipPopupIconNumberScene>("Scenes/Pops/TipPopupIconNumberScene");
			scene.OnStart(AppearNodeConfig.Get().GetBoosterSprite(BoosterType.Coins), coins, localizationUtility.GetString("watch_ad_completed_title"), string.Format(localizationUtility.GetString("watch_ad_completed_desc"), coins), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonBehaviour<EffectUtility>.Get().CreateBoosterType(BoosterType.Coins, scene.OKButton.transform.position);
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowVideoRewardWild()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupIconNumberScene>("Scenes/Pops/TipPopupIconNumberScene").OnStart(AppearNodeConfig.Get().GetBoosterSprite(BoosterType.Wild), 1, localizationUtility.GetString("watch_ad_completed_wild_title"), localizationUtility.GetString("watch_ad_completed_wild_desc"), localizationUtility.GetString("btn_ok"));
		}

		public static void ShowPurchasingCommodity(PurchasingCommodity commodity, bool doubleCommodity)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			TipPopupIconNumberScene scene = SingletonClass<MySceneManager>.Get().Popup<TipPopupIconNumberScene>("Scenes/Pops/SpinRewardsScene");
			scene.OnStart(AppearNodeConfig.Get().GetBoosterSprite(commodity.boosterType), commodity.count, localizationUtility.GetString("title_got"), string.Empty, localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonBehaviour<EffectUtility>.Get().CreateBoosterType(commodity.boosterType, scene.OKButton.transform.position);
			});
			scene.DoubleGameObject.SetActive(doubleCommodity);
		}
	}
}
