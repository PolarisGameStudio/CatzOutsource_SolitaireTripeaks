using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[CreateAssetMenu(fileName = "SaleConfig.asset", menuName = "Nightingale/Sale Config", order = 1)]
	public class SaleConfig : ScriptableObject
	{
		public List<SaleItemConfig> saleConfigs;

		private static SaleConfig _normalGroup;

		private static SaleConfig _specialGroup;

		public static SaleConfig GetNormalSale()
		{
			if (_normalGroup == null)
			{
				_normalGroup = SingletonBehaviour<LoaderUtility>.Get().GetAsset<SaleConfig>("Configs/SaleConfig.asset");
				_normalGroup.saleConfigs = _normalGroup.saleConfigs.SelectMany((SaleItemConfig e) => e.GetSaleConfigs()).ToList();
			}
			return _normalGroup;
		}

		public static SaleConfig GetSpecialSale()
		{
			if (_specialGroup == null)
			{
				_specialGroup = SingletonBehaviour<LoaderUtility>.Get().GetAsset<SaleConfig>("Configs/StoreSaleConfig.asset");
			}
			return _specialGroup;
		}

		public void Download()
		{
			foreach (SaleItemConfig saleConfig in saleConfigs)
			{
				saleConfig.DownloadAssetBundle();
			}
		}

		public SaleItemConfig GetSaleIndex(string type, string package)
		{
			if (saleConfigs == null)
			{
				return null;
			}
			return (from e in saleConfigs
				where e.Type == type && UnityPurchasingConfig.Get().GetLocalizedPrice(e.level) <= UnityPurchasingConfig.Get().GetLocalizedPrice(package)
				orderby UnityPurchasingConfig.Get().GetLocalizedPrice(e.level)
				select e).LastOrDefault();
		}

		public SaleItemConfig GetSaleIndex(string type, string package, string lowPackage, bool up)
		{
			if (saleConfigs == null)
			{
				return null;
			}
			SaleItemConfig saleIndex = GetSaleIndex(type, package, lowPackage, up, thinkTime: true);
			SaleItemConfig saleIndex2 = GetSaleIndex(type, package, lowPackage, up, thinkTime: false);
			if (saleIndex == null)
			{
				return saleIndex2;
			}
			if (saleIndex2 == null)
			{
				return saleIndex;
			}
			if (UnityPurchasingConfig.Get().GetLocalizedPrice(saleIndex.level) >= UnityPurchasingConfig.Get().GetLocalizedPrice(saleIndex2.level))
			{
				return saleIndex;
			}
			return saleIndex2;
		}

		private SaleItemConfig GetSaleIndex(string type, string index, string lowIndex, bool up, bool thinkTime)
		{
			if (saleConfigs == null)
			{
				return null;
			}
			List<SaleItemConfig> list = (from e in saleConfigs
				where e.Type.Equals(type) && e.IsReady(thinkTime) && UnityPurchasingConfig.Get().GetLocalizedPrice(e.level) >= UnityPurchasingConfig.Get().GetLocalizedPrice(lowIndex)
				orderby UnityPurchasingConfig.Get().GetLocalizedPrice(e.level)
				select e).ToList();
			if (list.Count == 0)
			{
				return null;
			}
			SaleItemConfig saleItemConfig = null;
			if (up)
			{
				saleItemConfig = (from e in list
					orderby UnityPurchasingConfig.Get().GetLocalizedPrice(e.level)
					select e).ToList().FirstOrDefault((SaleItemConfig e) => UnityPurchasingConfig.Get().GetLocalizedPrice(e.level) >= UnityPurchasingConfig.Get().GetLocalizedPrice(index));
				if (saleItemConfig == null)
				{
					saleItemConfig = list[list.Count - 1];
				}
			}
			else
			{
				saleItemConfig = (from e in list
					orderby UnityPurchasingConfig.Get().GetLocalizedPrice(e.level) descending
					select e).FirstOrDefault((SaleItemConfig e) => UnityPurchasingConfig.Get().GetLocalizedPrice(e.level) <= UnityPurchasingConfig.Get().GetLocalizedPrice(index));
				if (saleItemConfig == null)
				{
					saleItemConfig = list[0];
				}
			}
			return saleItemConfig;
		}

		public int FindIndex(SaleItemConfig config)
		{
			List<SaleItemConfig> list = (from e in saleConfigs
				where e.sceneName == config.sceneName
				select e).ToList();
			return list.FindIndex((SaleItemConfig e) => e.IsClone(config));
		}

		public SaleItemConfig GetSaleConfig(int index)
		{
			if (saleConfigs.Count > index)
			{
				return saleConfigs[index];
			}
			return null;
		}
	}
}
