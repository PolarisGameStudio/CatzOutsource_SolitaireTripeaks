using System;

namespace Nightingale.Ads
{
	[Serializable]
	public class VideoConfig
	{
		public VideoEcpm[] ecpms;

		public int VideoMax;

		public VideoEcpm[] GetVideoEcpm()
		{
			return ecpms;
		}
	}
}
