using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
    public class WatchVideoAdTipScene : SoundScene
    {
        public Button WatchAdButton;

        public Button CloseButton;

        public Text TitleLabel;

        public Text DesLabel;

        public Text ButtonLabel;

        private UnityAction<bool> CompletedUnityAction;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            WatchAdButton.onClick.RemoveListener(WatchAdClick);
        }

        public void OnStart(string title, string description, string btn, UnityAction<bool> unityAction)
        {
            CompletedUnityAction = unityAction;
            WatchAdButton.onClick.AddListener(WatchAdClick);
            CloseButton.onClick.AddListener(delegate
            {
                SingletonClass<MySceneManager>.Get().Close();
                if (unityAction != null)
                {
                    unityAction(arg0: false);
                }
            });
            TitleLabel.text = title;
            DesLabel.text = description;
            ButtonLabel.text = btn;
        }

        private void WatchAdClick()
        {
            WatchAdButton.interactable = false;
            IronSourceManager.Instance.ShowRewardedAds(WatchAdComeleted);
        }

        private void WatchAdComeleted()
        {
            WatchAdButton.interactable = true;
            AuxiliaryData.Get().WatchVideoCount++;
            AuxiliaryData.Get().WatchVideoTotal++;
            SingletonClass<MySceneManager>.Get().Close();
            if (CompletedUnityAction != null)
            {
                CompletedUnityAction(arg0: true);
            }
        }

        public static void ShowWatchAdRewardCoins()
        {
            LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
            SingletonClass<MySceneManager>.Get().Popup<WatchVideoAdTipScene>("Scenes/Pops/WatchVideoAdTipScene")
                .OnStart(localizationUtility.GetString("OUT_COINS_WatchVideoReward_Title"),
                    localizationUtility.GetString("OUT_COINS_WatchVideoReward_description"),
                    localizationUtility.GetString("btn_WATCH"), delegate(bool success)
                    {
                        if (success)
                        {
                            SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Video, 1000L,
                                changed: false);
                            TipPopupIconNumberScene.ShowVideoRewardCoins(1000);
                        }
                    });
        }

        public static void ShowWatchAdRewardDoubleBooster(UnityAction<bool> unityAction)
        {
            LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
            SingletonClass<MySceneManager>.Get().Popup<WatchVideoAdTipScene>("Scenes/Pops/WatchVideoAdTipScene")
                .OnStart(localizationUtility.GetString("Spin_Double_Title"),
                    localizationUtility.GetString("Spin_Double_description"),
                    localizationUtility.GetString("btn_WATCH"), unityAction);
        }
    }
}