using System;

namespace Nightingale.Ads
{
	[Serializable]
	public class ThirdPartyAdData
	{
		public ThirdPartyAdType type;

		public ThirdPartyPlatform platform;

		public string app_id;

		public string placement_id;
	}
}
