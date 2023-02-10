using UnityEngine.Events;

namespace Nightingale.Ads
{
	public abstract class BaseVideoAd
	{
		protected UnityAction<bool> unityAction;

		protected ThirdPartyAdData thirdPartyAdData;

		public virtual void Initialization(ThirdPartyAdData thirdPartyAdData, UnityAction<bool> unityAction)
		{
			this.thirdPartyAdData = thirdPartyAdData;
			this.unityAction = unityAction;
		}

		public virtual bool IsReady()
		{
			return false;
		}

		public virtual void Show()
		{
		}

		public virtual void Dispose()
		{
		}
	}
}
