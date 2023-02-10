using Nightingale.U2D;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PokerThemeGroup
	{
		public List<PokerThemeConfig> pokers;

		private PokerThemeConfig config;

		private static PokerThemeGroup group;

		public void ChangePoker()
		{
			config = pokers.Find((PokerThemeConfig e) => e.identifier.Equals(PokerData.Get().currentUsePoker));
			if (config == null)
			{
				config = pokers.Find((PokerThemeConfig e) => e.GetThemeType() == ThemeType.None);
			}
		}

		public PokerThemeConfig GetPoker()
		{
			if (config == null)
			{
				config = pokers.Find((PokerThemeConfig e) => e.identifier.Equals(PokerData.Get().currentUsePoker));
				if (config == null)
				{
					config = pokers.Find((PokerThemeConfig e) => e.GetThemeType() == ThemeType.None);
				}
			}
			return config;
		}

		public PokerThemeConfig GetPoker(int index)
		{
			PokerThemeConfig pokerThemeConfig = pokers.Find((PokerThemeConfig e) => e.Index == index);
			if (pokerThemeConfig == null)
			{
				pokerThemeConfig = pokers.Find((PokerThemeConfig e) => e.IsCanUse());
			}
			return pokerThemeConfig;
		}

		public int UseableCount()
		{
			return pokers.Count((PokerThemeConfig e) => e.IsCanUse() && e.GetThemeType() != 0 && !PokerData.Get().purchasings.Contains(e.identifier));
		}

		public void CalcLock()
		{
			PokerThemeConfig[] array = (from e in pokers
				where e.IsCanUse()
				select e).ToArray();
			PokerThemeConfig[] array2 = array;
			foreach (PokerThemeConfig pokerThemeConfig in array2)
			{
				PokerData.Get().PutPoker(pokerThemeConfig.identifier);
			}
		}

		public SpriteManager GetSpriteManager()
		{
			return SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, GetPoker().fileName);
		}

		public static PokerThemeGroup Get()
		{
			if (group == null)
			{
				group = JsonUtility.FromJson<PokerThemeGroup>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/PokerThemeConfigs.json"));
				for (int i = 0; i < group.pokers.Count; i++)
				{
					group.pokers[i].Index = i;
				}
			}
			return group;
		}
	}
}
