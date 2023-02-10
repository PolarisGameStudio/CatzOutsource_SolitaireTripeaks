using Nightingale.Utilitys;

namespace Nightingale.Ads
{
	public enum ThirdPartyAdType
	{
		[Type(typeof(VungleVideoAd))]
		Vungle,
		[Type(typeof(AdmobVideoAd))]
		Admob,
		[Type(typeof(UnityVideoAd))]
		Unity,
		[Type(typeof(FacebookVideoAd))]
		Facebook,
		[Type(typeof(MicrosoftVideoAd))]
		Microsoft,
		None
	}
}
