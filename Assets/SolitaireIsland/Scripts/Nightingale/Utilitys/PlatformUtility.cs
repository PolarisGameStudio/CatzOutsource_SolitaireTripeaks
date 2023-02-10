using System;
using com.F4A.MobileThird;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class PlatformUtility
	{
		public static bool DeviceModelIphoneX()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string text = SystemInfo.deviceModel.ToString();
				return text.Equals("iPhone10,3") || text.Equals("iPhone10,6");
			}
			return false;
		}

		public static void OnRateUs()
		{
			SocialManager.Instance.OpenRateGame();
		}

		public static void OpenSetting()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					string text = androidJavaObject.Call<string>("getPackageName", new object[0]);
					using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.net.Uri"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("fromParts", new object[3]
						{
							"package",
							text,
							null
						}))
						{
							using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", androidJavaObject2))
							{
								androidJavaObject3.Call<AndroidJavaObject>("addCategory", new object[1]
								{
									"android.intent.category.DEFAULT"
								});
								androidJavaObject3.Call<AndroidJavaObject>("setFlags", new object[1]
								{
									268435456
								});
								androidJavaObject.Call("startActivity", androidJavaObject3);
							}
						}
					}
				}
			}
		}

		public static void SendEmail(string subject, string body)
		{
            //Application.OpenURL(string.Format("mailto:{0}?subject={1}&body={2}", "NightingaleTech@outlook.com", Uri.EscapeUriString(subject), Uri.EscapeUriString(body)));

			SocialManager.Instance.SendMail(subject, body);
        }

		public static void OnMarketJoin()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				SocialManager.Instance.OpenRateGame();
			}
		}

		public static void OnApplicationQuit()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				@static.Call<bool>("moveTaskToBack", new object[1]
				{
					true
				});
			}
			else
			{
				Application.Quit();
			}
		}

		public static SystemLanguage GetLanguage()
		{
#if ENABLE_ANDROID_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("java/util/Locale");
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]);
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("US");
			string value = androidJavaObject.Call<string>("getDisplayLanguage", new object[1]
			{
				@static
			});
			try
			{
				return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), value);
			}
			catch
			{
				return Application.systemLanguage;
			}
#else
            return Application.systemLanguage;
#endif
        }

		public static string GetCountry()
		{
			return new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault", new object[0]).Call<string>("getCountry", new object[0]);
		}
	}
}
