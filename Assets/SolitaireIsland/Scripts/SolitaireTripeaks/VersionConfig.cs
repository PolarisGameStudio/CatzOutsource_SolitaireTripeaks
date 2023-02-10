using com.F4A.MobileThird;
using Nightingale.Extensions;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	[Serializable]
	public class VersionConfig
	{
		public string version;

		public string minVersion;

		public string url;

		public static void Initialization(AssetBundle assetBundle, UnityAction unityAction)
		{
			VersionConfig config = null;
#if ENABLE_DATA_LOCAL
			string text = "version_android.json";
			switch (Application.platform)
			{
				default:
					text = "version_android.json";
					break;
				case RuntimePlatform.IPhonePlayer:
					text = "version_ios.json";
					break;
#if !UNITY_2017_3_OR_NEWER
				case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
				text = "version_winrt.json";
				break;
#endif
			}
			config = assetBundle.Read<VersionConfig>(text);
            DMCFileUtilities.SaveFileByData(config, "VersionConfig.json");
#else
            config = DMCFileUtilities.LoadContentFromResource<VersionConfig>("datagame/VersionConfig.json");
#endif

			if (config == null) unityAction?.Invoke();

            //@TODO check new version at here
			//else if (new Version(config.version) > new Version(Application.version))
			//{
			//	bool flag = false;
			//	if (!string.IsNullOrEmpty(config.minVersion))
			//	{
			//		flag = (new Version(config.minVersion) > new Version(Application.version));
			//	}
			//	TipPopupNoIconScene.ShowNewVersion(!flag, delegate(bool sure)
			//	{
			//		if (sure)
			//		{
			//			if (string.IsNullOrEmpty(config.url))
			//			{
			//				PlatformUtility.OnMarketJoin();
			//			}
			//			else
			//			{
			//				Application.OpenURL(config.url);
			//			}
			//		}
			//		else
			//		{
			//			if (unityAction != null)
			//			{
			//				unityAction();
			//			}
			//			SingletonClass<MySceneManager>.Get().Close();
			//		}
			//	});
			//}
			else if (unityAction != null)
			{
				unityAction();
			}
		}
	}
}
