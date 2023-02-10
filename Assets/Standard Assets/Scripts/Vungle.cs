using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

public class Vungle
{
	private const string PLUGIN_VERSION = "5.3.0";

	private const string IOS_SDK_VERSION = "5.3.0";

	private const string WIN_SDK_VERSION = "5.1.0";

	private const string ANDROID_SDK_VERSION = "5.3.0";

	[CompilerGenerated]
	private static Action<string> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<string, AdFinishedEventArgs> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<string, bool> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<string> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action _003C_003Ef__mg_0024cache4;

	public static string VersionInfo
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder("unity-");
			return stringBuilder.Append("5.3.0").Append("/android-").Append("5.3.0")
				.ToString();
		}
	}

	public static event Action<string> onAdStartedEvent;

	public static event Action<string, AdFinishedEventArgs> onAdFinishedEvent;

	public static event Action<string, bool> adPlayableEvent;

	public static event Action onInitializeEvent;

	public static event Action<string> onLogEvent;

	static Vungle()
	{
		VungleManager.OnAdStartEvent += adStarted;
		VungleManager.OnAdFinishedEvent += adFinished;
		VungleManager.OnAdPlayableEvent += adPlayable;
		VungleManager.OnSDKLogEvent += onLog;
		VungleManager.OnSDKInitializeEvent += onInitialize;
	}

	private static void adStarted(string placementID)
	{
		if (Vungle.onAdStartedEvent != null)
		{
			Vungle.onAdStartedEvent(placementID);
		}
	}

	private static void adPlayable(string placementID, bool playable)
	{
		if (Vungle.adPlayableEvent != null)
		{
			Vungle.adPlayableEvent(placementID, playable);
		}
	}

	private static void onLog(string log)
	{
		if (Vungle.onLogEvent != null)
		{
			Vungle.onLogEvent(log);
		}
	}

	private static void adFinished(string placementID, AdFinishedEventArgs args)
	{
		if (Vungle.onAdFinishedEvent != null)
		{
			Vungle.onAdFinishedEvent(placementID, args);
		}
	}

	private static void onInitialize()
	{
		if (Vungle.onInitializeEvent != null)
		{
			Vungle.onInitializeEvent();
		}
	}

	public static void init(string appId, string[] placements)
	{
	}

	public static void setSoundEnabled(bool isEnabled)
	{
	}

	public static bool isAdvertAvailable(string placementID)
	{
		return false;
	}

	public static void loadAd(string placementID)
	{
	}

	public static void playAd(string placementID)
	{
	}

	public static void playAd(Dictionary<string, object> options, string placementID)
	{
		if (options == null)
		{
			options = new Dictionary<string, object>();
		}
	}

	public static void clearSleep()
	{
	}

	public static void setEndPoint(string endPoint)
	{
	}

	public static void setLogEnable(bool enable)
	{
	}

	public static string getEndPoint()
	{
		return string.Empty;
	}

	public static void onResume()
	{
	}

	public static void onPause()
	{
	}
}
