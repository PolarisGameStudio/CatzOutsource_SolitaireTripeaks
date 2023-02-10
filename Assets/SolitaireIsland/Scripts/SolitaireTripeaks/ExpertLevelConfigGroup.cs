using com.F4A.MobileThird;
using Nightingale.Extensions;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class ExpertLevelConfigGroup : SingletonClass<ExpertLevelConfigGroup>
	{
		private ExpertWorldConfig worldConfig;

		private List<LevelRetrunCoinConfig> retrunCoinConfigs;

		private List<LevelConfig> levelConfigs;

		private AssetBundle GetBundle(string path)
		{
			Debug.Log($"@LOG ExpertLevelConfigGroup GetBundle path:{path}".Color(Color.blue));
			AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => e.name == path);
			if (assetBundle != null)
			{
				return assetBundle;
			}
			try
			{
				path = GlobalConfig.GetPathByRuntimePlatform(path);
				if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
				{
					path = Path.Combine(Application.persistentDataPath, path);
					return AssetBundle.LoadFromFile(path);
				}
				if (SingletonBehaviour<StreamingAssetsPathUtility>.Get().Exists(path))
				{
					path = Path.Combine(Application.streamingAssetsPath, path);
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
			AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => e.name == path);
			if (assetBundle != null)
			{
				assetBundle.Unload(unloadAllLoadedObjects: true);
			}
		}

		public void ReadAssetBundle()
		{
			Debug.Log("@LOG ExpertLevelConfigGroup ReadAssetBundle".Color(Color.blue));
			UnityAction<AssetBundle> UpdateAssetBundle = delegate(AssetBundle asset)
			{
				if (asset != null)
				{
					string[] allAssetNames = asset.GetAllAssetNames();
					int num = (from e in allAssetNames
						where new Version(GetVersion(e)) <= new Version(Application.version)
						select e).Count((string e) => e.Contains("levels") && Path.GetExtension(e).Equals(".json"));
					worldConfig = new ExpertWorldConfig
					{
						chapters = new List<ChapterConfig>()
					};
					while (num > 0)
					{
						int num2 = 10;
						if (num < 10)
						{
							num2 = num;
						}
						worldConfig.chapters.Add(new ChapterConfig
						{
							LevelCount = num2
						});
						num -= num2;
					}
					retrunCoinConfigs = (from e in asset.GetAllAssetNames()
						where new Version(GetVersion(e)) <= new Version(Application.version)
						where e.Contains("coin")
						orderby e
						select asset.Read<LevelRetrunCoinConfig>(e)).ToList();
					levelConfigs = (from e in asset.GetAllAssetNames()
						where new Version(GetVersion(e)) <= new Version(Application.version)
						where e.Contains("levels")
						orderby e
						select asset.Read<LevelConfig>(e)).ToList();
					asset.Unload(unloadAllLoadedObjects: true);
				}
			};
			UpdateAssetBundle(GetBundle("remote/elite.asset"));
			string expertVersion = GameConfig.Get().ExpertVersion;
			if (!string.IsNullOrEmpty(expertVersion) 
				&& !(SingletonData<TripeaksLocalData>.Get().ExpertVersion == expertVersion))
			{
				TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform("remote/elite.asset"))).RemoveAllListeners()
					.AddListener(delegate(object asset, float p)
					{
						if (asset != null)
						{
							UpdateAssetBundle(asset as AssetBundle);
							SingletonData<TripeaksLocalData>.Get().ExpertVersion = expertVersion;
							SingletonData<TripeaksLocalData>.Get().FlushData();
						}
					});
			}
		}

		private string GetVersion(string content)
		{
			content = content.Replace("assets/builds/blast/elite/", string.Empty);
			content = content.Substring(0, content.IndexOf("/"));
			return content;
		}

		public LevelRetrunCoinConfig ReadLevelRetrunCoinConfig(int index)
		{
			if (index < 0 || index > retrunCoinConfigs.Count - 1)
			{
				index = 0;
			}
			return retrunCoinConfigs[index];
		}

		public LevelConfig ReadLevelConfig(int index)
		{
			if (index < 0 || index > levelConfigs.Count - 1)
			{
				index = 0;
			}
			return levelConfigs[index];
		}

		public ExpertWorldConfig GetWorldConfig()
		{
			if (worldConfig == null)
			{
				ReadAssetBundle();
			}
			return worldConfig;
		}
	}
}
