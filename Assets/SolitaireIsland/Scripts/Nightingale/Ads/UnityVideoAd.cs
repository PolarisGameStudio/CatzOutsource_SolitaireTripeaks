using com.F4A.MobileThird;
using System;
using UnityEngine.Events;

namespace Nightingale.Ads
{
	public class UnityVideoAd : BaseVideoAd
	{
		public override void Initialization(ThirdPartyAdData thirdPartyAdData, UnityAction<bool> unityAction)
		{
			base.Initialization(thirdPartyAdData, unityAction);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool IsReady()
		{
            return false;
		}

		public override void Show()
		{
		}
	}
}
