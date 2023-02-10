using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class LoaderUtility : SingletonBehaviour<LoaderUtility>
	{
		private List<AssetData> loadeds = new List<AssetData>();

		public string GetText(string scene, string path)
		{
			TextAsset asset = GetAsset<TextAsset>(path);
			if (asset == null)
			{
				return string.Empty;
			}
			return asset.text;
		}

		public string GetText(string path)
		{
			return GetText("Application", path);
		}

		public void LoadAsync(List<AssetPath> datas, UnityAction unityAction)
		{
			int index = 0;
			int loading = 0;
			foreach (AssetPath data in datas)
			{
				if (!HasAsset(data.path))
				{
					loading++;
					StartCoroutine(GetAsync(data.scene, data.path, delegate
					{
						index++;
						if (index >= loading && unityAction != null)
						{
							unityAction();
						}
					}));
				}
			}
			if (index == 0 && unityAction != null)
			{
				unityAction();
			}
		}

		public T GetAsset<T>(string scene, string fileName) where T : UnityEngine.Object
		{
			T val = TryGetAsset<T>(scene, fileName);
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = Load<T>(fileName);
				loadeds.Add(new AssetData
				{
					scene = scene,
					path = fileName,
					asset = val
				});
			}
			return val;
		}

		public T GetAsset<T>(string fileName) where T : UnityEngine.Object
		{
			return GetAsset<T>("Application", fileName);
		}

		public T GetAssetComponent<T>(string scene, string fileName) where T : Component
		{
			T val = TryGetAsset<T>(scene, fileName);
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = Load<GameObject>(fileName).GetComponent<T>();
				loadeds.Add(new AssetData
				{
					scene = scene,
					path = fileName,
					asset = val
				});
			}
			return val;
		}

		public T GetAssetComponent<T>(string fileName) where T : Component
		{
			return GetAssetComponent<T>("Application", fileName);
		}

		public void UnLoad(UnityEngine.Object asset)
		{
			List<AssetData> list = (from e in loadeds
				where e.asset.Equals(asset)
				select e).ToList();
			foreach (AssetData item in list)
			{
				UnLoad(item);
			}
		}

		public void UnLoad(AssetData assetData)
		{
			loadeds.Remove(assetData);
		}

		public void UnLoad(string scene, string path)
		{
			IEnumerable<AssetData> enumerable = from e in loadeds
				where e.IsMatch(scene, path)
				select e;
			foreach (AssetData item in enumerable)
			{
				UnLoad(item);
			}
		}

		public void UnLoadScene(string scene)
		{
			List<AssetData> list = (from e in loadeds
				where e.scene.Equals(scene)
				select e).ToList();
			foreach (AssetData item in list)
			{
				UnLoad(item);
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		private T Load<T>(string path) where T : UnityEngine.Object
		{
			string directoryName = Path.GetDirectoryName(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return Resources.Load<T>(Path.Combine(directoryName, fileNameWithoutExtension));
		}

		private IEnumerator GetAsync(string scene, string path, UnityAction unityAction)
		{
			string folder = Path.GetDirectoryName(path);
			string newFolder = Path.GetFileNameWithoutExtension(path);
			if (!HasAsset(scene, path))
			{
				loadeds.Add(new AssetData
				{
					scene = scene,
					path = path,
					asset = Resources.Load(Path.Combine(folder, newFolder))
				});
			}
			yield return new WaitForSeconds(0.1f);
			unityAction?.Invoke();
		}

		private T TryGetAsset<T>(string scene, string fileName) where T : UnityEngine.Object
		{
			AssetData assetData = loadeds.Find((AssetData e) => e.IsMatch(scene, fileName));
			if (assetData != null)
			{
				return (T)assetData.asset;
			}
			return (T)null;
		}

		private bool HasAsset(string fileName)
		{
			AssetData assetData = loadeds.Find((AssetData e) => e.IsMatch(fileName));
			if (assetData != null)
			{
				return true;
			}
			return false;
		}

		private bool HasAsset(string scene, string fileName)
		{
			AssetData assetData = loadeds.Find((AssetData e) => e.IsMatch(scene, fileName));
			if (assetData != null)
			{
				return true;
			}
			return false;
		}
	}
}
