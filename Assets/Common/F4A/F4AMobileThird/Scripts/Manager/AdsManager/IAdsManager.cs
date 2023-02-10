namespace com.F4A.MobileThird
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IAdsManager
    {
        bool IsInterstitialAdsReady();
        bool ShowInterstitialAds(int adsInterval = -1, Action onInterstitialClose = null,
            string locationId = "", EInterstitialAdNetwork[] adNetworks = null);
    }
}