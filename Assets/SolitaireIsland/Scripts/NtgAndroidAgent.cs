using UnityEngine;

public class NtgAndroidAgent
{
	public static void Init(bool debug = false)
	{
#if ENABLE_NTG_NATIVE
		AndroidJNIHelper.debug = debug;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.ntg.androidnativeplugin.Agent"))
			{
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				androidJavaClass2.CallStatic("Init", @static);
			}
		}
#endif
	}

	public static string GetInstallReferrer()
	{
#if ENABLE_NTG_NATIVE
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ntg.androidnativeplugin.Agent"))
		{
			return androidJavaClass.CallStatic<string>("GetInstallReferrer", new object[0]);
		}
#endif
        return string.Empty;
	}
}
