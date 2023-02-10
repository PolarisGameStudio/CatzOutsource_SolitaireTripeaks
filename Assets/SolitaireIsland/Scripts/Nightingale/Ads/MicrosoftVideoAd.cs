using UnityEngine.Events;

namespace Nightingale.Ads
{
	public class MicrosoftVideoAd : BaseVideoAd
	{
		public override void Initialization(ThirdPartyAdData thirdPartyAdData, UnityAction<bool> unityAction)
		{
			base.Initialization(thirdPartyAdData, unityAction);
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
