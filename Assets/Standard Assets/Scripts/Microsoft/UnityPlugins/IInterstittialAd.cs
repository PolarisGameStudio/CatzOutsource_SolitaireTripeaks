using System;

namespace Microsoft.UnityPlugins
{
	public interface IInterstittialAd : IDisposable
	{
		InterstitialAdState State
		{
			get;
		}

		void AddCallback(AdCallback type, Action<object> cb);

		void ClearCallback(AdCallback type);

		void RequestAndShow(string appId, string adUnitId);

		void Request(string appId, string adUnitId, AdType type);

		void Show();
	}
}
