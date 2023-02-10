namespace com.F4A.MobileThird
{
    using System.Collections;
    using System.Collections.Generic;
    using com.F4A.MobileThird;
    using UnityEngine;
    using UnityEngine.UI;

    public class GenericObjectManager : SingletonMono<GenericObjectManager>
    {
        public static event System.Action OnShareFacebook = delegate { };

        [SerializeField]
        private UIDailyReward dailyReward = null;
        public UIDailyReward DailyReward
        {
            get { return dailyReward; }
        }

        [SerializeField]
        private UILoadingPanel loadingPanel;
        public UILoadingPanel LoadingPanel
        {
            get { return loadingPanel; }
        }

        [SerializeField]
        private Transform groupButtonTF;
        public Transform GroupButtonTF
        {
            get { return groupButtonTF; }
        }

        [SerializeField]
        private Button btnShare = null, btnAchievement = null, btnHelp = null, btnRate = null;

        private void Start()
        {
#if DEFINE_FACEBOOK_SDK
            btnShare.gameObject.SetActive(true);
#else
            btnShare.gameObject.SetActive(false);
#endif
            btnShare.onClick.AddListener(HandleBtnShare_Click);
#if DEFINE_GAMESERVICES
            btnAchievement.gameObject.SetActive(true);
#else
            btnAchievement.gameObject.SetActive(false);
#endif
            btnAchievement.onClick.AddListener(HandleBtnAchievement_Click);
            btnHelp.onClick.AddListener(HandleBtnHelp_Click);
            btnRate.onClick.AddListener(HandleBtnRate_Click);
        }


        private void HandleBtnAchievement_Click()
        {
            SocialManager.Instance.ShowAchievementsUI();
        }

        private void HandleBtnShare_Click()
        {
            //SocialManager.Instance.ShareFacebook();
            OnShareFacebook?.Invoke();
        }

        private void HandleBtnRate_Click()
        {
            SocialManager.Instance.OpenRateGame();
        }

        private void HandleBtnHelp_Click()
        {
            SocialManager.Instance.OpenLinkDeveloper();
        }
    }
}