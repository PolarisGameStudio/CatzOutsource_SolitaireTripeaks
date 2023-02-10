using System.Collections.Generic;
using UnityEngine.Events;

namespace Nightingale.Socials
{
	public interface IBaseSocial
	{
		bool IsInitialized();

		bool IsLogin();

		string UserId();

		string TokenString();

		SocialPlatform GetPlatform();

		void Init(UnityAction unityAction);

		void Logout();

		void Login(UnityAction unityAction);

		void API(string query, int method, Dictionary<string, string> parameters = null, UnityAction<string> unityAction = null);

		void AppRequest(string message, IEnumerable<string> to = null, IEnumerable<object> filters = null, string data = "", string title = "", UnityAction<int> unityAction = null);

		void TryRefreshCurrentAccessToken(UnityAction unityAction);

		void LogAppEvent(string logEvent, float? valueToSum = default(float?), Dictionary<string, object> parameters = null);

		void GetAppLink(UnityAction<IDictionary<string, object>> unityAction);
	}
}
