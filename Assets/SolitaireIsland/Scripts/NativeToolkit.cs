using UnityEngine;

public class NativeToolkit : MonoBehaviour
{
	private static NativeToolkit instance;

	private static GameObject go;

	private static AndroidJavaClass obj;

	public static NativeToolkit Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "NativeToolkit";
				go = gameObject;
				instance = go.AddComponent<NativeToolkit>();
				if (Application.platform == RuntimePlatform.Android)
				{
					obj = new AndroidJavaClass("com.secondfury.nativetoolkit.Main");
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static string GetCountryCode()
	{
		Instance.Awake();
		string result = null;
		if (Application.platform == RuntimePlatform.Android)
		{
			result = obj.CallStatic<string>("getLocale", new object[0]);
		}
		return result;
	}

	public static void ScheduleLocalNotification(string title, string message, int id = 0, int delayInMinutes = 0, string sound = "default_sound", bool vibrate = false, string smallIcon = "ic_notification", string largeIcon = "ic_notification_large")
	{
		Instance.Awake();
		if (Application.platform == RuntimePlatform.Android)
		{
			obj.CallStatic("scheduleLocalNotification", title, message, id, delayInMinutes, sound, vibrate, smallIcon, largeIcon);
		}
	}

	public static void ClearLocalNotification(int id)
	{
		Instance.Awake();
		if (Application.platform == RuntimePlatform.Android)
		{
			obj.CallStatic("clearLocalNotification", id);
		}
	}

	public static void ClearAllLocalNotifications()
	{
		Instance.Awake();
		if (Application.platform == RuntimePlatform.Android)
		{
			obj.CallStatic("clearAllLocalNotifications");
		}
	}

	public static bool WasLaunchedFromNotification()
	{
		Instance.Awake();
		if (Application.platform == RuntimePlatform.Android)
		{
			return obj.CallStatic<bool>("wasLaunchedFromNotification", new object[0]);
		}
		return false;
	}
}
