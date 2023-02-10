using com.F4A.MobileThird;
using Nightingale.Extensions;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ChapterConfig
	{
		public string version;

		public string fileName;

		public string thumbnail;

		public string name;

		public int LevelCount;

		private AssetBundle GetBundle(string path)
		{
			AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => e.name == path);
			if (assetBundle != null)
			{
				Debug.Log($"@LOG ChapterConfig.GetBundle path:{path} container");
				return assetBundle;
			}
			try
			{
				path = GlobalConfig.GetPathByRuntimePlatform(path);
                Debug.Log($"@LOG ChapterConfig.GetBundle path:{path} not container");

                if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
                {
					Debug.Log($"@LOG ChapterConfig.GetBundle persistentDataPath path:{path}".Color(Color.blue));
                    //path = Path.Combine(Application.persistentDataPath, path);
                    path = Path.Combine(Application.persistentDataPath, path);
                    return AssetBundle.LoadFromFile(path);
                }

                if (StreamingAssetsPathUtility.Get().Exists(path))
				{
					path = StreamingAssetsPathUtility.StreamingAssetsPath(path);

					Debug.Log($"LOG ChapterConfig.GetBundle streamingAssetsPath path:{path}".Color(Color.yellow));
                    return AssetBundle.LoadFromFile(path);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			return null;
		}

		private void DestoryBundle(string path)
		{
			AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(e.name, path));
			if (assetBundle != null)
			{
				assetBundle.Unload(unloadAllLoadedObjects: true);
			}
		}

		public bool IsSupportVersion()
		{
			return new Version(version) <= new Version(Application.version);
		}

		public bool IsThumbnailAtLocalPath()
		{
			var path = GlobalConfig.GetPathByRuntimePlatform(thumbnail);
			var isThumb = FileUtility.Exists(path);
            Debug.Log($"IsThumbnailAtLocalPath path:{path}, isThumb:{isThumb}");
			return isThumb;
		}

		public NormalTask GetThumbnailDownloadTask()
		{
			if (IsThumbnailAtLocalPath())
			{
				return new NormalTask();
			}
			return TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform(thumbnail)));
		}

		public AssetBundle GetThumbnailAssetBundle()
		{
			if (string.IsNullOrEmpty(thumbnail))
			{
				return null;
			}
			return GetBundle(thumbnail);
		}

		public void DestoryThumbnail()
		{
			DestoryBundle(thumbnail);
		}

		public bool IsDetailsAtLocalPath()
		{
			return FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(fileName));
		}

		public NormalTask GetDetailsDownloadTask()
		{
			if (IsDetailsAtLocalPath())
			{
				return new NormalTask();
			}
			return TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform(fileName)));
		}

		public AssetBundle GetDetailsAssetBundle()
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			return GetBundle(fileName);
		}

		public void DestoryDetails()
		{
			DestoryBundle(fileName);
		}

		public LevelConfig GetLevelConfig(int level)
		{
			Debug.Log($"@LOG ChapterConfig.GetLevelConfig level:{level}/fileName:{fileName}");
			AssetBundle detailsAssetBundle = GetDetailsAssetBundle();
			if(detailsAssetBundle != null)
            {
                string[] allAssetNames = detailsAssetBundle.GetAllAssetNames();
                string[] array = (from e in allAssetNames
                                  where e.Contains("levels")
                                  orderby e
                                  select e).ToArray();
                for (int counter = 0; counter < array.Length; counter++)
                {
                    TextAsset textAsset = detailsAssetBundle.LoadAsset<TextAsset>(array[counter]);
                    if (textAsset != null)
                    {
                        DMCFileUtilities.SaveFile(textAsset.text, $"{this.name}/Level_{counter}.json");
                    }
                }
                return detailsAssetBundle.Read<LevelConfig>(array[level]);
            }
            else
            {
                Debug.Log($"@LOG ChapterConfig.GetLevelConfig detailsAssetBundle null");
                return new LevelConfig();
            }
		}
	}
}
