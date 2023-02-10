using System;

namespace Microsoft.UnityPlugins
{
	public interface IInterstitialAdFactory
	{
		IInterstittialAd CreateAd();

		IInterstittialAd CreateAd(Action<object> readyCallback, Action<object> completedCallback, Action<object> cancelledCallback, Action<object> errorCallback);
	}
}
