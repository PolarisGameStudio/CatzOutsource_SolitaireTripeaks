using Nightingale.Utilitys;
using System;

namespace Nightingale.Ads
{
	[Serializable]
	public class VideoEcpm
	{
		public string Identifier;

		public float ecpm;

		public ThirdPartyAdType GetAdType()
		{
			return EnumUtility.GetEnumType(Identifier, ThirdPartyAdType.None);
		}
	}
}
