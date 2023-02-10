using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.adjust.sdk
{
	public class Adjust : MonoBehaviour
	{
		private const string errorMsgEditor = "Adjust: SDK can not be used in Editor.";

		private const string errorMsgStart = "Adjust: SDK not started. Start it manually using the 'start' method.";

		private const string errorMsgPlatform = "Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.";

		public bool startManually = true;

		public bool eventBuffering;

		public bool sendInBackground;

		public bool launchDeferredDeeplink = true;

		public string appToken = "{Your App Token}";

		public AdjustLogLevel logLevel = AdjustLogLevel.Info;

		public AdjustEnvironment environment;

		private void Awake()
		{
			if (!IsEditor())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
				if (!startManually)
				{
					AdjustConfig adjustConfig = new AdjustConfig(appToken, environment, logLevel == AdjustLogLevel.Suppress);
					adjustConfig.setLogLevel(logLevel);
					adjustConfig.setSendInBackground(sendInBackground);
					adjustConfig.setEventBufferingEnabled(eventBuffering);
					adjustConfig.setLaunchDeferredDeeplink(launchDeferredDeeplink);
					start(adjustConfig);
				}
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void start(AdjustConfig adjustConfig)
		{
			if (!IsEditor())
			{
				if (adjustConfig == null)
				{
					UnityEngine.Debug.Log("Adjust: Missing config to start.");
				}
				else
				{
					UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
				}
			}
		}

		public static void trackEvent(AdjustEvent adjustEvent)
		{
			if (!IsEditor())
			{
				if (adjustEvent == null)
				{
					UnityEngine.Debug.Log("Adjust: Missing event to track.");
				}
				else
				{
					UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
				}
			}
		}

		public static void setEnabled(bool enabled)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static bool isEnabled()
		{
			if (IsEditor())
			{
				return false;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return false;
		}

		public static void setOfflineMode(bool enabled)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void setDeviceToken(string deviceToken)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void gdprForgetMe()
		{
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
		}

		public static void appWillOpenUrl(string url)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void sendFirstPackages()
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void addSessionPartnerParameter(string key, string value)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void addSessionCallbackParameter(string key, string value)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void removeSessionPartnerParameter(string key)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void removeSessionCallbackParameter(string key)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void resetSessionPartnerParameters()
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void resetSessionCallbackParameters()
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static string getAdid()
		{
			if (IsEditor())
			{
				return string.Empty;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return string.Empty;
		}

		public static AdjustAttribution getAttribution()
		{
			if (IsEditor())
			{
				return null;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return null;
		}

		public static string getWinAdid()
		{
			if (IsEditor())
			{
				return string.Empty;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return string.Empty;
		}

		public static string getIdfa()
		{
			if (IsEditor())
			{
				return string.Empty;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return string.Empty;
		}

		public static string getSdkVersion()
		{
			if (IsEditor())
			{
				return string.Empty;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return string.Empty;
		}

		[Obsolete("This method is intended for testing purposes only. Do not use it.")]
		public static void setReferrer(string referrer)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static void getGoogleAdId(Action<string> onDeviceIdsRead)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			}
		}

		public static string getAmazonAdId()
		{
			if (IsEditor())
			{
				return string.Empty;
			}
			UnityEngine.Debug.Log("Adjust: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.");
			return string.Empty;
		}

		private static bool IsEditor()
		{
			return false;
		}

		public static void SetTestOptions(Dictionary<string, string> testOptions)
		{
			if (!IsEditor())
			{
				UnityEngine.Debug.Log("Cannot run integration tests. None of the supported platforms selected.");
			}
		}
	}
}
