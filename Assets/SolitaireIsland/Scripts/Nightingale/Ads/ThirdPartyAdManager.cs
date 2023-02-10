using com.F4A.MobileThird;
using Nightingale.Extensions;
using Nightingale.Toasts;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nightingale.Ads
{
    public class ThirdPartyAdManager : SingletonBehaviour<ThirdPartyAdManager>
    {
        //private List<BaseVideoAd> baseVideoAds = new List<BaseVideoAd>();

        public VideoCompelete compeleted = new VideoCompelete();

        private int VideoMax;

        public void Initialization(AssetBundle assetBundle)
        {
            ThirdPartyPlatform thirdPartyPlatform = ThirdPartyPlatform.Android;
            VideoConfig videoConfig = null;
#if ENABLE_DATA_LOCAL
            string text = "video_android.json";
            switch (Application.platform)
            {
                default:
                    text = "video_android.json";
                    thirdPartyPlatform = ThirdPartyPlatform.Android;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    text = "video_ios.json";
                    thirdPartyPlatform = ThirdPartyPlatform.IOS;
                    break;
#if !UNITY_2018_1_OR_NEWER
                case RuntimePlatform.MetroPlayerX86:
                case RuntimePlatform.MetroPlayerX64:
                case RuntimePlatform.MetroPlayerARM:
#else
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
#endif
                    text = "video_winrt.json";
                    thirdPartyPlatform = ThirdPartyPlatform.WinRT;
                    break;
            }
            videoConfig = assetBundle.Read<VideoConfig>(text);
            DMCFileUtilities.SaveFileByData(videoConfig, "VideoConfig.json");
#else
            videoConfig = DMCFileUtilities.LoadContentFromResource<VideoConfig>("datagame/VideoConfig.json");
#endif
            //foreach (BaseVideoAd baseVideoAd2 in baseVideoAds)
            //{
            //    baseVideoAd2.Dispose();
            //}
            //baseVideoAds.Clear();
            //IOrderedEnumerable<VideoEcpm> orderedEnumerable = from e in videoConfig.GetVideoEcpm()
            //                                                  orderby e.ecpm descending
            //                                                  select e;
            //foreach (VideoEcpm item in orderedEnumerable)
            //{
            //    ThirdPartyAdType adType = item.GetAdType();
            //    if (adType != ThirdPartyAdType.None)
            //    {
            //        BaseVideoAd baseVideoAd = (BaseVideoAd)Activator.CreateInstance(EnumUtility.GetStringType(adType));
            //        baseVideoAds.Add(baseVideoAd);
            //        baseVideoAd.Initialization(GetPlacementId(adType, thirdPartyPlatform), VideoCompleted);
            //    }
            //}
            VideoMax = videoConfig.VideoMax;

            AdsManager.OnRewardedAdCompleted += AdsManager_OnRewardedAdCompleted;
            AdsManager.OnRewardedAdSkiped += AdsManager_OnRewardedAdSkiped;
            AdsManager.OnRewardedAdFailed += AdsManager_OnRewardedAdFailed;
        }

        public bool IsRewardedVideoAvailable(int current)
        {
            if (current > VideoMax)
            {
                return false;
            }
            return AdsManager.Instance.IsRewardAdsReady();
        }

        public void ShowRewardedVideoAd()
        {
            if(AdsManager.Instance.IsRewardAdsReady())
            {
                AdsManager.Instance.ShowRewardAds();
            }
        }

        protected override void OnDestroy()
        {
            //foreach (BaseVideoAd baseVideoAd in baseVideoAds)
            //{
            //    baseVideoAd.Dispose();
            //}
            AdsManager.OnRewardedAdCompleted -= AdsManager_OnRewardedAdCompleted;
            AdsManager.OnRewardedAdSkiped -= AdsManager_OnRewardedAdSkiped;
            AdsManager.OnRewardedAdFailed -= AdsManager_OnRewardedAdFailed;
        }

        private ThirdPartyAdData GetPlacementId(ThirdPartyAdType type, ThirdPartyPlatform platform)
        {
            return NightingaleConfig.Get().GetThirdPartyAdData(type, platform);
        }

        private void AdsManager_OnRewardedAdCompleted(ERewardedAdNetwork adNetwork, string adName, double value)
        {
            VideoCompleted(true);
        }

        private void AdsManager_OnRewardedAdFailed(ERewardedAdNetwork adNetwork, string error)
        {
            VideoCompleted(false);
        }

        private void AdsManager_OnRewardedAdSkiped(ERewardedAdNetwork adNetwork)
        {
            VideoCompleted(false);
        }

        private void VideoCompleted(bool completed)
        {
            DelayDo(new WaitForSeconds(0.1f), delegate
            {
                LoadingHelper.Get("WatchVideo").StopLoading();
                compeleted.Invoke(completed);
            });
        }
    }
}