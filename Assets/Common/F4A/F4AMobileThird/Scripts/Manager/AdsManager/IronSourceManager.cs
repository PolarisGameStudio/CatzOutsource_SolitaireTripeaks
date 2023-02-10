using System;
using System.Collections.Generic;
using AudienceNetwork;
using SolitaireTripeaks;
using UnityEngine;

public class IronSourceManager : SingletonMonoBehaviour<IronSourceManager>
{
    private const string KeyRemovedInterstitialAds = "iron_source_manager_removed_interstitial_ads";

    [SerializeField] private string ironSourceKeyAndroid;
    [SerializeField] private string ironSourceKeyIos;

    private bool _isFinishVideo;
    private bool _isShowingAds;

    private void Start()
    {
#if !UNITY_EDITOR
        AudienceNetworkAds.Initialize();
#if UNITY_IOS
        AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif
#endif

#if UNITY_ANDROID
        IronSource.Agent.init(ironSourceKeyAndroid);

#elif UNITY_IOS
        IronSource.Agent.init(ironSourceKeyIos);
#else
        //adInterstitalUnitID = "unexpected_platform";
        //appID = "unexpected_platform";
#endif

        IronSource.Agent.validateIntegration();
        // IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP);

        if (!IsRemovedInterstitialAds())
        {
            IronSource.Agent.loadInterstitial();
            IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialOpenEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialClosedEvent;
        }

        IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedOpenEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedClosed;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedCompleted;

        lastShow = Time.time;
    }


    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    public void ShowBanner()
    {
        IronSource.Agent.displayBanner();
    }

    private void OnRewardedOpenEvent()
    {
        _isShowingAds = true;
    }

    private void OnRewardedClosed()
    {
        IronSource.Agent.displayBanner();
        lastShow = Time.time;
        _isShowingAds = false;

        if (_isFinishVideo)
        {
            _onRewardedComplete?.Invoke();
        }
    }

    private void OnRewardedCompleted(IronSourcePlacement obj)
    {
        _isFinishVideo = true;
    }

    private void OnInterstitialOpenEvent()
    {
        _isShowingAds = true;
    }

    private void OnInterstitialClosedEvent()
    {
        _isShowingAds = false;

        lastShow = Time.time;

        IronSource.Agent.displayBanner();
        IronSource.Agent.loadInterstitial();

        _onInterstitialClosed?.Invoke();
    }

    /// <summary>
    /// hiển thị quảng cáo intertitial
    /// </summary>
    /// <param name="onInterstitialClosed">gọi khi user xem hết ads</param>
    /// <param name="showAdsResult">trả về kết quả show ads</param>
    /// <param name="where">vị trí đặt ads</param>
    /// <param name="level">level cao nhất của user</param>
    private static float TIME_SHOW_ADS = 30f;

    private float lastShow = 0;

    public void ShowInterstitialAd(string _logInterstitialAd, Action onInterstitialClosed = null,
        Action<ShowAdsResult> showAdsResult = null, bool _forceShow = false)
    {
#if UNITY_EDITOR
        Debug.LogError("ShowInterstitialAd --- " + _logInterstitialAd);
#endif
        if (IsInterstitialAdsAvailable())
        {
            if (_forceShow == false)
            {
                if (Time.time - lastShow < TIME_SHOW_ADS)
                {
                    onInterstitialClosed?.Invoke();
                    return;
                }
            }

            IronSource.Agent.hideBanner();
            IronSource.Agent.showInterstitial();
            _onInterstitialClosed = onInterstitialClosed;

            if (!_isShowingAds)
            {
                showAdsResult?.Invoke(ShowAdsResult.Success);
            }
        }
        else
        {
            onInterstitialClosed?.Invoke();
            showAdsResult?.Invoke(ShowAdsResult.AdsNotAvailable);
            IronSource.Agent.loadInterstitial();
        }
    }

    private Action _onRewardedComplete;
    private Action _onInterstitialClosed;

    /// <summary>
    /// hiển thị quảng cáo rewarded 
    /// </summary>
    /// <param name="onRewardedComplete">gọi khi user xem hết video</param>
    /// <param name="showAdsResult">trả về kết quả show ads</param>
    /// <param name="where">vị trí đặt ads</param>
    /// <param name="level">level cao nhất của user</param>
    /// <param name="showRewardInstead"></param>
    public void ShowRewardedAds(Action onRewardedComplete, Action<ShowAdsResult> showAdsResult = null)
    {
#if UNITY_EDITOR
        onRewardedComplete?.Invoke();
        return;
#endif
        if (!IsRewardedVideoAvailable())
        {
            TipPopupNoIconScene.ShowVideoWaiting(null);
            return;
        }

        _onRewardedComplete = onRewardedComplete;

        _isFinishVideo = false;

        IronSource.Agent.hideBanner();
        IronSource.Agent.showRewardedVideo();

        if (!_isShowingAds)
        {
            showAdsResult?.Invoke(ShowAdsResult.Success);
        }
    }

    /// <summary>
    /// set flag để xem xét có hiển thị interstitial ads cho user hay ko
    /// 0 = vẫn show interstitial ads
    /// 1 = ko show interstitial ads
    /// </summary>
    /// <param name="flag"></param>
    public void SetRemoveInterstitialAds(int flag)
    {
        PlayerPrefs.SetInt(KeyRemovedInterstitialAds, flag);
    }


    /// <summary>
    /// kiểm tra user có mua gói bỏ ads ko
    /// 0 = chưa mua; 1 = mua
    /// </summary>
    /// <returns></returns>
    public bool IsRemovedInterstitialAds()
    {
        return false;
        if (!PlayerPrefs.HasKey(KeyRemovedInterstitialAds))
        {
            return false;
        }

        return PlayerPrefs.GetInt(KeyRemovedInterstitialAds) == 1;
    }

    /// <summary>
    /// kiểm tra có video interstitial để hiển thị hay ko
    /// </summary>
    /// <returns></returns>
    public bool IsInterstitialAdsAvailable()
    {
        return IronSource.Agent.isInterstitialReady();
    }

    /// <summary>
    /// kiểm tra có reward video để hiển thị hay ko
    /// </summary>
    /// <returns></returns>
    public bool IsRewardedVideoAvailable()
    {
#if UNITY_EDITOR
        return true;
#else
        return IronSource.Agent.isRewardedVideoAvailable();
#endif
    }

    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }
    // #endif
}

public enum ShowAdsResult
{
    AdsNotAvailable,
    RemovedInterstitial,
    Success
}