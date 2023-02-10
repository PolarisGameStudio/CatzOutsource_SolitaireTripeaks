using UnityEngine.Events;

namespace Nightingale.Ads
{
	public class VungleVideoAd : BaseVideoAd
	{
		public override void Initialization(ThirdPartyAdData thirdPartyAdData, UnityAction<bool> unityAction)
		{
			base.Initialization(thirdPartyAdData, unityAction);
		}
	}
}
