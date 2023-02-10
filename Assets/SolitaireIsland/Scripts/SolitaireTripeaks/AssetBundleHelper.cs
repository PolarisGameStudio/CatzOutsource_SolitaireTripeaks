using Nightingale.Extensions;
using Nightingale.JSONUtilitys;
using Nightingale.Utilitys;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class AssetBundleHelper : SingletonBehaviour<AssetBundleHelper>
	{
		private List<string> assets = new List<string>();

		private AssetBundleEvenet bundleEvenet = new AssetBundleEvenet();

		private AssetBundleCompletedEvenet assetBundleCompletedEvenet = new AssetBundleCompletedEvenet();

		private bool downing;

		private int retryTotal;

		public void Download(string fileName)
		{
			if (!assets.Contains(fileName))
			{
				assets.Add(fileName);
				if (!downing)
				{
					download();
				}
			}
		}

		public void TryDownload(string fileName)
		{
			if (!File.Exists(Path.Combine(Application.persistentDataPath, GlobalConfig.GetPathByRuntimePlatform(fileName))) 
                && !SingletonBehaviour<StreamingAssetsPathUtility>.Get().Exists(GlobalConfig.GetPathByRuntimePlatform(fileName)))
			{
				Download(fileName);
			}
		}

		public void AddLister(UnityAction<string, float> unityAction)
		{
			bundleEvenet.AddListener(unityAction);
		}

		public void AddLister(UnityAction<AssetBundle> unityAction)
		{
			assetBundleCompletedEvenet.AddListener(unityAction);
		}

		public void RemoveListener(UnityAction<string, float> unityAction)
		{
			bundleEvenet.RemoveListener(unityAction);
		}

		public void RemoveListener(string key, UnityAction<AssetBundle> unityAction)
		{
			assetBundleCompletedEvenet.RemoveListener(unityAction);
		}

		private void download()
		{
			if (assets.Count > 0)
			{
				StartCoroutine(download(assets[0], 0f, string.Empty));
			}
		}

		private IEnumerator download(string fileName, float process = 0f, string tag = "")
		{
			AssetBundle bundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(fileName, e.name));
			if (bundle != null)
			{
				downing = false;
				retryTotal = 0;
				assets.Remove(fileName);
				bundleEvenet.Invoke(fileName, 1f);
				assetBundleCompletedEvenet.Invoke(bundle);
				yield return new WaitForEndOfFrame();
				yield break;
			}
			downing = true;
			UnityWebRequest Request = UnityWebRequest.Get(Path.Combine(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform(fileName)));
			Request.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
			Request.SetRequestHeader("Pragma", "no-cache");
			Request.SetRequestHeader("___did", SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier);
			Request.SetRequestHeader("___pid", SolitaireTripeaksData.Get().GetPlayerId());
			Request.SetRequestHeader("___pf", Application.platform.ToString());
			Request.SetRequestHeader("___cv", Application.version);
			Request.SetRequestHeader("___sv", "1");
			Request.SetRequestHeader("___aid", "tripeaks");
			Request.SetRequestHeader("___pp", process.ToString());
			Request.SetRequestHeader("___tag", tag.TOBase64());
			yield return Request.SendWebRequest();
			if (!Request.isDone)
			{
				yield break;
			}
			Dictionary<string, object> datas = Json.Deserialize(Request.downloadHandler.text) as Dictionary<string, object>;
			if (datas != null && datas.ContainsKey("Url") && !string.IsNullOrEmpty(datas["Url"].ToString()))
			{
				Request = UnityWebRequest.Get(datas["Url"].ToString());
				Request.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
				Request.SetRequestHeader("Pragma", "no-cache");
				UnityWebRequestAsyncOperation Operation = Request.SendWebRequest();
				do
				{
					bundleEvenet.Invoke(fileName, Request.downloadProgress);
					if (Request.isDone)
					{
						if (string.IsNullOrEmpty(Request.error))
						{
							bundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(fileName, e.name));
							if (bundle != null)
							{
								bundle.Unload(unloadAllLoadedObjects: false);
							}
							if (Request.downloadHandler.data.Length > 0)
							{
								yield return AssetBundle.LoadFromMemoryAsync(Request.downloadHandler.data);
								bundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(fileName, e.name));
								if (bundle == null)
								{
									yield return new WaitForSeconds(Mathf.Min(Mathf.Pow(3f, ++retryTotal) + 12f, 300f));
									StartCoroutine(download(fileName, 1f, "decompress error"));
								}
							}
							else
							{
								yield return new WaitForSeconds(Mathf.Min(Mathf.Pow(3f, ++retryTotal) + 12f, 300f));
								StartCoroutine(download(fileName, 1f, "data=0"));
							}
						}
						if (bundle != null)
						{
							assets.Remove(fileName);
							retryTotal = 0;
							assetBundleCompletedEvenet.Invoke(bundle);
							bundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(fileName, e.name));
							if (bundle != null)
							{
								bundle.Unload(unloadAllLoadedObjects: false);
							}
							FileUtility.SaveFile(Path.Combine(Application.persistentDataPath, GlobalConfig.GetPathByRuntimePlatform(fileName)), Request.downloadHandler.data);
							download();
							downing = false;
						}
						else
						{
							yield return new WaitForSeconds(Mathf.Min(Mathf.Pow(3f, ++retryTotal) + 12f, 300f));
							StartCoroutine(download(fileName, 1f, Request.error));
						}
						Request.Dispose();
						break;
					}
					yield return new WaitForSeconds(0.5f);
				}
				while (Application.isPlaying);
				yield return Operation;
			}
			else if (datas != null && datas.ContainsKey("Url") && string.IsNullOrEmpty(datas["Url"].ToString()))
			{
				yield return new WaitForSeconds(Mathf.Min(Mathf.Pow(3f, ++retryTotal) + 12f, 300f));
				StartCoroutine(download(fileName, 1f, "shield user"));
			}
			else
			{
				yield return new WaitForSeconds(Mathf.Min(Mathf.Pow(3f, ++retryTotal) + 12f, 300f));
				StartCoroutine(download(fileName, 1f, "unknow"));
			}
		}
	}
}
