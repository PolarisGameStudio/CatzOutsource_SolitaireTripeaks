using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class LevelConfig
	{
		public int HandCount;

		public float Scale;

		public List<CardConfig> Cards;

		public List<ObjectConfig> Objects;

		public List<BoosterType> GetInsideBoosters()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			List<BoosterType> Boosters = new List<BoosterType>();
			Func<BoosterType, bool> func = delegate(BoosterType type)
			{
				if (!Boosters.Contains(type))
				{
					Boosters.Add(type);
				}
				return Boosters.Count >= 3;
			};
			Boosters = new List<BoosterType>();
			CardTypeNodeConfig cardType = AppearNodeConfig.Get().GetCardType(playSchedule);
			ExtraTypeNodeConfig extraConfig = AppearNodeConfig.Get().GetExtraConfig(playSchedule);
			CardTypeNodeConfig[] cardTypeNodeConfigs = AppearNodeConfig.Get().CardTypeNodeConfigs;
			foreach (CardTypeNodeConfig config in cardTypeNodeConfigs)
			{
				if (config.booster != BoosterType.None && (cardType == null || cardType.cardType != config.cardType))
				{
					int num = Cards.Count((CardConfig e) => e.CardType == config.cardType);
					if (num > 0 && func(config.booster))
					{
						return (from e in Boosters
							orderby e descending
							select e).ToList();
					}
				}
			}
			ExtraTypeNodeConfig[] extraTypeNodeConfigs = AppearNodeConfig.Get().ExtraTypeNodeConfigs;
			foreach (ExtraTypeNodeConfig config2 in extraTypeNodeConfigs)
			{
				if (extraConfig == null || extraConfig.extraType != config2.extraType)
				{
					int num2 = Cards.Count((CardConfig e) => e.HasExtraType(config2.extraType));
					if (num2 > 0 && func(config2.booster))
					{
						return (from e in Boosters
							orderby e descending
							select e).ToList();
					}
				}
			}
			if (func(BoosterType.MultipleStreaks))
			{
				return (from e in Boosters
					orderby e descending
					select e).ToList();
			}
			if (func(BoosterType.FullFlip))
			{
				return (from e in Boosters
					orderby e descending
					select e).ToList();
			}
			return (from e in Boosters
				orderby e descending
				select e).ToList();
		}

		public List<BoosterType> GetOutsideBoosters()
		{
			List<BoosterType> list = new List<BoosterType>();
			list.Add(BoosterType.BellaBlessing);
			list.Add(BoosterType.MultipleStreaks);
			list.Add(BoosterType.FullFlip);
			return list;
		}

		public List<BoosterType> GetBoosters()
		{
			List<BoosterType> insideBoosters = GetInsideBoosters();
			insideBoosters.AddRange(GetOutsideBoosters());
			return insideBoosters.Distinct().ToList();
		}
	}
}
