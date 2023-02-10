#if ENABLE_FB_SDK
using Facebook.Unity;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Socials
{
	public class UnityFacebook : IBaseSocial
	{
		public void API(string query, int method, Dictionary<string, string> parameters = null, UnityAction<string> unityAction = null)
		{
#if ENABLE_FB_SDK
			FB.API(query, (HttpMethod)method, delegate(IGraphResult result)
			{
				if (string.IsNullOrEmpty(result.Error) && unityAction != null)
				{
					unityAction(result.RawResult);
				}
			}, parameters);
#endif
		}

		public void AppRequest(string message, IEnumerable<string> to = null, IEnumerable<object> filters = null, string data = "", string title = "", UnityAction<int> unityAction = null)
		{
#if ENABLE_FB_SDK
			FB.AppRequest(message, to, filters, null, null, data, title, delegate(IAppRequestResult result)
			{
				if (unityAction != null && string.IsNullOrEmpty(result.Error) && !result.Cancelled)
				{
					try
					{
						unityAction((result.To != null) ? result.To.ToArray().Length : 0);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex.Message);
					}
				}
			});
#endif
		}

		public void Init(UnityAction callBack)
        {
#if ENABLE_FB_SDK
			InitDelegate initDelegate = delegate
			{
                FB.ActivateApp();
                callBack?.Invoke();
			};
            if (!FB.IsInitialized)
            {
				FB.Init(initDelegate);
			}
			else
			{
				initDelegate();
			}
#else
            callBack?.Invoke();
#endif
        }

		public bool IsInitialized()
		{
#if ENABLE_FB_SDK
			return FB.IsInitialized;
#endif
            return false;
		}

		public bool IsLogin()
		{
#if ENABLE_FB_SDK
			return FB.IsLoggedIn;
#endif
            return false;
		}

		public void Login(UnityAction unityAction)
		{
			List<string> list = new List<string>();
			list.Add("public_profile");
			list.Add("email");
			list.Add("user_friends");
#if ENABLE_FB_SDK
			FB.LogInWithReadPermissions(list, delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
			});
#endif
		}

		public void Logout()
		{
#if ENABLE_FB_SDK
			FB.LogOut();
#endif
		}

		public string TokenString()
		{
#if ENABLE_FB_SDK
			if (AccessToken.CurrentAccessToken == null)
			{
				return string.Empty;
			}
			return AccessToken.CurrentAccessToken.TokenString;
#endif
            return string.Empty;
		}

		public void TryRefreshCurrentAccessToken(UnityAction unityAction)
		{
#if ENABLE_FB_SDK
			if (AccessToken.CurrentAccessToken != null && AccessToken.CurrentAccessToken.ExpirationTime.Subtract(DateTime.Now).TotalHours < 1.0)
			{
				FB.Mobile.RefreshCurrentAccessToken(delegate
				{
					if (unityAction != null)
					{
						unityAction();
					}
				});
			}
#endif
		}

		public string UserId()
		{
#if ENABLE_FB_SDK
			if (AccessToken.CurrentAccessToken == null)
			{
				return string.Empty;
			}
			return AccessToken.CurrentAccessToken.UserId;
#endif
            return string.Empty;
		}

		public void LogAppEvent(string logEvent, float? valueToSum = default(float?), Dictionary<string, object> parameters = null)
		{
#if ENABLE_FB_SDK
			FB.LogAppEvent(logEvent, valueToSum, parameters);
#endif
		}

		public SocialPlatform GetPlatform()
		{
			return SocialPlatform.Facebook;
		}

		public void GetAppLink(UnityAction<IDictionary<string, object>> unityAction)
		{
#if ENABLE_FB_SDK
			FB.GetAppLink(delegate(IAppLinkResult result)
			{
				if (unityAction != null)
				{
					unityAction(result.ResultDictionary);
				}
			});
#endif
		}
	}
}
