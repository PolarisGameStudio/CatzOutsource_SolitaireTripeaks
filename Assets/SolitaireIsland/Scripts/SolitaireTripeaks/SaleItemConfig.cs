using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SaleItemConfig
	{
		public string sceneName;

		public string prefabName;

		public List<SalePackage> purchasingPackages;

		public string Type;

		public string level;

		public int minutes;

		public int startMonth;

		public int endMonth;

		public bool IsClone(SaleItemConfig config)
		{
			if (!sceneName.Equals(config.sceneName))
			{
				return false;
			}
			if (!Type.Equals(config.Type))
			{
				return false;
			}
			if (!minutes.Equals(config.minutes))
			{
				return false;
			}
			if (!startMonth.Equals(config.startMonth))
			{
				return false;
			}
			if (!config.GetString().Equals(GetString()))
			{
				return false;
			}
			return true;
		}

		public void GetAssetBundle(UnityAction<AssetBundle> unityAction)
		{
			if (sceneName.EndsWith(".asset") && unityAction != null)
			{
				TaskHelper.GetLocal().AppendTask(new LocalAssetTask(GlobalConfig.GetPathByRuntimePlatform(sceneName))).RemoveAllListeners()
					.AddListener(delegate(object asset, float p)
					{
						if (asset != null)
						{
							unityAction(asset as AssetBundle);
						}
					});
			}
		}

		public void DownloadAssetBundle()
		{
			if (sceneName.EndsWith(".asset") && !FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(sceneName)))
			{
				TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, GlobalConfig.GetPathByRuntimePlatform(sceneName)));
			}
		}

		public void DestoryAssetBundle()
		{
			if (sceneName.EndsWith(".asset"))
			{
				AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().ToList().Find((AssetBundle e) => FileUtility.IsSameFile(sceneName, e.name));
				if (assetBundle != null)
				{
					assetBundle.Unload(unloadAllLoadedObjects: true);
				}
			}
		}

		public bool IsReady(bool thinkTime = false)
		{
			if (sceneName.EndsWith(".asset") && !FileUtility.Exists(GlobalConfig.GetPathByRuntimePlatform(sceneName)))
			{
				return false;
			}
			if (thinkTime && endMonth > 0 && startMonth > 0)
			{
				int month = DateTime.Now.Month;
				if (startMonth > endMonth)
				{
					return month > startMonth || month < endMonth;
				}
				return month > startMonth && month < endMonth;
			}
			return true;
		}

		public void GetObject(UnityAction<GameObject> unityAction)
		{
			if (sceneName.EndsWith(".asset"))
			{
				GetAssetBundle(delegate(AssetBundle ass)
				{
					if (string.IsNullOrEmpty(prefabName))
					{
						prefabName = "SaleScene";
					}
					unityAction(ass.LoadAsset<GameObject>(prefabName));
				});
			}
			else
			{
				unityAction(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(sceneName));
			}
		}

		public string GetString()
		{
			string text = "|";
			foreach (SalePackage purchasingPackage in purchasingPackages)
			{
				string text2 = string.Empty;
				SaleCommodity[] commoditys = purchasingPackage.commoditys;
				foreach (SaleCommodity saleCommodity in commoditys)
				{
					text2 += $"{saleCommodity.boosterType}_{saleCommodity.saleCount},";
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2.Substring(0, text2.Length - 1);
				}
				text += $"{purchasingPackage.id}_{text2}|";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public List<SaleItemConfig> GetSaleConfigs()
		{
			string[] array = sceneName.Split(';');
			List<SaleItemConfig> list = new List<SaleItemConfig>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(',');
				int num = 1;
				if (array.Length > 1)
				{
					num = int.Parse(array[1]);
				}
				int num2 = 12;
				if (array.Length > 2)
				{
					num2 = int.Parse(array[2]);
				}
				list.Add(new SaleItemConfig
				{
					sceneName = array3[0],
					purchasingPackages = purchasingPackages.ToList(),
					level = level,
					minutes = minutes,
					startMonth = num,
					endMonth = num2,
					Type = Type,
					prefabName = prefabName
				});
			}
			return list;
		}

		public bool IsUsable()
		{
			foreach (SalePackage purchasingPackage in purchasingPackages)
			{
				if (purchasingPackage.commoditys.Count((SaleCommodity e) => !AppearNodeConfig.Get().IsUsable(e.boosterType)) != 0)
				{
					return false;
				}
			}
			return true;
		}

		public SaleItemConfig Clone()
		{
			SaleItemConfig saleItemConfig = new SaleItemConfig();
			saleItemConfig.sceneName = sceneName;
			saleItemConfig.startMonth = startMonth;
			saleItemConfig.endMonth = endMonth;
			saleItemConfig.level = level;
			saleItemConfig.minutes = minutes;
			saleItemConfig.purchasingPackages = (from e in purchasingPackages
				select e.Clone()).ToList();
			saleItemConfig.Type = Type;
			saleItemConfig.prefabName = prefabName;
			return saleItemConfig;
		}
	}
}
