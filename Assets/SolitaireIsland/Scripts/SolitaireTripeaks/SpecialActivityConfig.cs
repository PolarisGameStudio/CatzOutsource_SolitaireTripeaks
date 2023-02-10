using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	[CreateAssetMenu(fileName = "SpecialActivityConfig.asset", menuName = "Nightingale/SpecialActivityConfig", order = 1)]
	public class SpecialActivityConfig : ScriptableObject
	{
		public string StartTime;

		public string EndTime;

		public string Name;

		public string SpecialId;

		public ExchangeConfig[] Configs;

		private static SpecialActivityConfig group;

		public string GetSpecialId()
		{
			if (string.IsNullOrEmpty(SpecialId))
			{
				SpecialId = $"{StartTime}_{Name}";
			}
			return SpecialId;
		}

		public DateTime GetStartTime()
		{
			return DateTime.Parse(StartTime);
		}

		public DateTime GetEndTime()
		{
			return DateTime.Parse(EndTime);
		}

		public bool IsActive()
		{
			if (GetEndTime().Subtract(DateTime.Now).TotalSeconds <= 0.0)
			{
				return false;
			}
			return DateTime.Now.Subtract(GetStartTime()).TotalSeconds >= 0.0;
		}

		public static SpecialActivityConfig Get()
		{
			if (group == null)
			{
				group = SingletonBehaviour<LoaderUtility>.Get().GetAsset<SpecialActivityConfig>("Configs/SpecialActivityConfig");
				List<ExchangeConfig> configs = group.Configs.ToList();
				ExchangeConfig finder = configs.Find((ExchangeConfig e) => e.boosterType > BoosterType.RandomBooster);
				(from e in AppearNodeConfig.Get().GetAllRandomBoosters()
					where configs.Find((ExchangeConfig p) => p.boosterType == e) == null
					select e).ToList().ForEach(delegate(BoosterType e)
				{
					configs.Add(new ExchangeConfig
					{
						boosterType = e,
						count = ((finder == null) ? 1 : finder.count),
						limit = ((finder != null) ? finder.limit : (-1)),
						need = ((finder != null) ? finder.need : 8)
					});
				});
				group.Configs = configs.ToArray();
			}
			return group;
		}
	}
}
