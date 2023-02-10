using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
    public class FreeCoinScene : SoundScene
    {
        public Transform InviteRewardTransform;

        public LabelButton FacebookButton;

        public Button WatchAdButton;

        public ProgressBarUI WatchProcessProgressBarUI;

        public GameObject WatchVideoRewardCoinsUI;

        public GameObject WatchVideoRewardWildUI;

        public Button SlotsButton;

        public Button WheelButton;

        private UnityAction OnCloseUnityAction;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SingletonBehaviour<FacebookMananger>.Get().LoginChanged.RemoveListener(UpdateFacebookButton);
            WatchAdButton.onClick.RemoveListener(WatchAdClick);
            SlotsButton.onClick.RemoveListener(SlotsClick);
            WheelButton.onClick.RemoveListener(WheelClick);
            if (OnCloseUnityAction != null)
            {
                OnCloseUnityAction();
            }

            SingletonBehaviour<SynchronizeUtility>.Get().UploadGameData();
        }

        public void OnStart(UnityAction unityAction = null)
        {
            WatchProcessProgressBarUI.SetFillAmount(0f);
            base.IsStay = true;
            OnCloseUnityAction = unityAction;
            SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(UpdateFacebookButton);
            FacebookButton.Button.onClick.AddListener(FacebookClick);
            WatchAdButton.onClick.AddListener(WatchAdClick);
            SlotsButton.onClick.AddListener(SlotsClick);
            WheelButton.onClick.AddListener(WheelClick);
            UpdateFacebookButton(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
            UpdateWatchUI(animation: false);
        }

        private void SlotsClick()
        {
            SingletonClass<MySceneManager>.Get().Popup<SlotsMachine>("MiniGames/Slots/SlotsMachine", new ScaleEffect());
        }

        private void WheelClick()
        {
            SingletonClass<MySceneManager>.Get()
                .Popup<WheelGamePopup>("MiniGames/Wheel/WheelGamePopup", new ScaleEffect());
        }

        private void FacebookClick()
        {
            SingletonBehaviour<GlobalConfig>.Get().ShowLoginOrInvitable(AuxiliaryData.Get().IsFacebookReward);
        }

        private void WatchAdClick()
        {
            WatchAdButton.interactable = false;
            if (IronSourceManager.Instance.IsRewardedVideoAvailable())
            {
                IronSourceManager.Instance.ShowRewardedAds(WatchAdComeleted);
            }
            else
            {
                TipPopupNoIconScene.ShowVideoWaiting(delegate { WatchAdButton.interactable = true; });
            }
        }

        private void UpdateWatchUI(bool animation)
        {
            if (animation)
            {
                SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Video, 1000L, changed: false);
                TipPopupIconNumberScene.ShowVideoRewardCoins(1000);
                WatchAdButton.interactable = true;
            }
        }

        private void WatchAdComeleted()
        {
            if (!WatchAdButton.interactable)
            {
                AuxiliaryData.Get().WatchVideoCount++;
                AuxiliaryData.Get().WatchVideoTotal++;
                UpdateWatchUI(animation: true);
            }
        }

        private void UpdateFacebookButton(bool login)
        {
            FacebookButton.Label.text = LocalizationUtility.Get("Localization_free.json")
                .GetString((!login) ? "btn_login" : "btn_invite");
            InviteRewardTransform.gameObject.SetActive(value: false);
        }
    }
}