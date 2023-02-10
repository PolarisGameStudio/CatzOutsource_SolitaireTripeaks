using com.F4A.MobileThird;
using System;
using UnityEngine.Events;

namespace Nightingale.Ads
{
	public class AdmobVideoAd : BaseVideoAd
	{
		private bool completed;

		public override void Initialization(ThirdPartyAdData thirdPartyAdData, UnityAction<bool> unityAction)
		{
			base.Initialization(thirdPartyAdData, unityAction);
            AdsManager.OnRewardedAdCompleted += AdsManager_OnRewardedAdCompleted;
		}

        private void AdsManager_OnRewardedAdCompleted(ERewardedAdNetwork adNetwork, string adName, double value)
        {
            completed = true;
            unityAction?.Invoke(completed);
        }

        public override void Dispose()
		{
            AdsManager.OnRewardedAdCompleted -= AdsManager_OnRewardedAdCompleted;
        }

        public override bool IsReady()
		{
            return AdsManager.Instance.IsRewardAdsReady();
		}

		public override void Show()
		{
            AdsManager.Instance.ShowRewardAds();
		}
	}
}
