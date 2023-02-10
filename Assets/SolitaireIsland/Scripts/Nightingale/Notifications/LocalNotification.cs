using System;
using UnityEngine;

namespace Nightingale.Notifications
{
	public class LocalNotification : MonoBehaviour
	{
		public static void NotificationMessage(string title, string message, TimeSpan timeSpan)
		{
			if (!(timeSpan.TotalMilliseconds < 0.0))
			{
			}
		}

		public static void CleanNotification()
		{
		}

		public static void RegisterForNotifications()
		{
			try
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent");
				androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1]
				{
					"android.settings.APP_NOTIFICATION_SETTINGS"
				});
				AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
				{
					"app_package",
					@static.Call<string>("getPackageName", new object[0])
				});
				androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
				{
					"app_uid",
					@static.Call<AndroidJavaObject>("getApplicationInfo", new object[0]).Get<int>("uid")
				});
				@static.Call("startActivity", androidJavaObject);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public static bool isNotificationsEnabled()
		{
			try
			{
				AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.support.v4.app.NotificationManagerCompat");
				return androidJavaClass.CallStatic<AndroidJavaObject>("from", new object[1]
				{
					@static
				}).Call<bool>("areNotificationsEnabled", new object[0]);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			return true;
		}

		public static void UnregisterForRemoteNotifications()
		{
		}
	}
}
