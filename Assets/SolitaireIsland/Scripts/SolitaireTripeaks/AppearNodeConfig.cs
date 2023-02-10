using Nightingale.Localization;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AppearNodeConfig : ScriptableObject
	{
		public CardTypeNodeConfig[] CardTypeNodeConfigs;

		public ExtraTypeNodeConfig[] ExtraTypeNodeConfigs;

		public List<BoosterTypeConfig> BoosterTypeConfigs;

		private static AppearNodeConfig boosterAppearNodeConfig;

		public static AppearNodeConfig Get()
		{
			if (boosterAppearNodeConfig == null)
			{
				boosterAppearNodeConfig = SingletonBehaviour<LoaderUtility>.Get().GetAsset<AppearNodeConfig>("Configs/AppearNodeConfig");
			}
			return boosterAppearNodeConfig;
		}

		public List<BoosterType> GetAllRandomBoosters()
		{
			List<BoosterType> list = (from e in CardTypeNodeConfigs
				select e.booster).ToList();
			list.AddRange((from e in ExtraTypeNodeConfigs
				select e.booster).ToList());
			list.AddRange(new List<BoosterType>
			{
				BoosterType.BurnRope,
				BoosterType.SnakeEliminate,
				BoosterType.FullFlip,
				BoosterType.BellaBlessing,
				BoosterType.MultipleStreaks
			});
			list = list.Distinct().ToList();
			list.RemoveAll((BoosterType e) => e == BoosterType.None);
			list.RemoveAll((BoosterType e) => e <= BoosterType.RandomBooster);
			return list;
		}

		public BoosterType GetRandomBooster()
		{
			List<BoosterType> list = (from e in GetAllRandomBoosters()
				where IsUsable(e)
				select e).ToList();
			if (!list.Contains(BoosterType.RandomBooster))
			{
				list.Add(BoosterType.RandomBooster);
			}
			if (!list.Contains(BoosterType.BurnRope))
			{
				list.Add(BoosterType.BurnRope);
			}
			if (!list.Contains(BoosterType.BombEliminate))
			{
				list.Add(BoosterType.BombEliminate);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public List<BoosterType> GetAllPackBoosters()
		{
			List<BoosterType> allRandomBoosters = GetAllRandomBoosters();
			allRandomBoosters.Add(BoosterType.FreePlay);
			allRandomBoosters.Add(BoosterType.FreeSlotsPlay);
			allRandomBoosters.Add(BoosterType.FreeWheelPlay);
			allRandomBoosters.Add(BoosterType.Rocket);
			allRandomBoosters.Add(BoosterType.Wild);
			allRandomBoosters.Add(BoosterType.DoubleStar);
			return (from e in allRandomBoosters
				orderby e
				select e).ToList();
		}

		public List<BoosterType> GetBoosters()
		{
			ScheduleData best = PlayData.Get().GetBestScheduleData();
			List<BoosterType> list = (from e in CardTypeNodeConfigs
				where best.Equals(e.scheduleData) || best.Than(e.scheduleData)
				select e.booster).ToList();
			list.AddRange((from e in ExtraTypeNodeConfigs
				where best.Equals(e.scheduleData) || best.Than(e.scheduleData)
				select e.booster).ToList());
			list.Insert(0, BoosterType.SnakeEliminate);
			list.Insert(0, BoosterType.BurnRope);
			list.Insert(0, BoosterType.FullFlip);
			list.Insert(0, BoosterType.MultipleStreaks);
			list.Insert(0, BoosterType.BellaBlessing);
			list = list.Distinct().ToList();
			list.RemoveAll((BoosterType e) => e == BoosterType.None);
			return (from e in list
				orderby e
				select e).ToList();
		}

		public bool HasCardType(ScheduleData playSchedule)
		{
			return CardTypeNodeConfigs.ToList().Find((CardTypeNodeConfig e) => e.scheduleData.Equals(playSchedule)) != null;
		}

		public bool IsUsable(BoosterType type)
		{
			if (type <= BoosterType.BellaBlessing)
			{
				return true;
			}
			return GetBoosters().Contains(type);
		}

		public CardTypeNodeConfig GetCardType(ScheduleData playSchedule)
		{
			return CardTypeNodeConfigs.ToList().Find((CardTypeNodeConfig e) => e.scheduleData.Equals(playSchedule));
		}

		public bool HasExtraType(ScheduleData playSchedule)
		{
			return ExtraTypeNodeConfigs.ToList().Find((ExtraTypeNodeConfig e) => e.scheduleData.Equals(playSchedule)) != null;
		}

		public ExtraTypeNodeConfig GetExtraConfig(ScheduleData playSchedule)
		{
			return ExtraTypeNodeConfigs.ToList().Find((ExtraTypeNodeConfig e) => e.scheduleData.Equals(playSchedule));
		}

		public bool HasExtraType(ExtraType type)
		{
			return ExtraTypeNodeConfigs.ToList().Find((ExtraTypeNodeConfig e) => e.extraType == type && PlayData.Get().GetBestScheduleData().Than(e.scheduleData)) != null;
		}

		public bool HasCardType(CardType type)
		{
			return CardTypeNodeConfigs.ToList().Find((CardTypeNodeConfig e) => e.cardType == type && PlayData.Get().GetBestScheduleData().Than(e.scheduleData)) != null;
		}

		public string GetBoosterTitle(BoosterType boosterType)
		{
			BoosterTypeConfig boosterTypeConfig = BoosterTypeConfigs.Find((BoosterTypeConfig e) => e.Type == boosterType);
			if (boosterTypeConfig == null)
			{
				return string.Empty;
			}
			return LocalizationUtility.Get("Localization_guide.json").GetString(boosterTypeConfig.GuidTitle);
		}

		public string GetBoosterByNumber(BoosterType boosterType, int number)
		{
			switch (boosterType)
			{
			case BoosterType.UnlimitedPlay:
			case BoosterType.UnlimitedDoubleStar:
			{
				TimeSpan timeSpan = TimeSpan.FromMinutes(number);
				if (timeSpan.TotalHours >= 1.0)
				{
					return $"x{timeSpan.TotalHours:N0} Hours";
				}
				return $"x{timeSpan.TotalMinutes:N0} Min";
			}
			case BoosterType.Coins:
				return $"x{number:N0}";
			default:
				return $"x{number:N0}";
			}
		}

		public string GetBoosterDescription(BoosterType boosterType)
		{
			BoosterTypeConfig boosterTypeConfig = BoosterTypeConfigs.Find((BoosterTypeConfig e) => e.Type == boosterType);
			if (boosterTypeConfig == null)
			{
				return string.Empty;
			}
			return LocalizationUtility.Get("Localization_guide.json").GetString(boosterTypeConfig.GuidDescription);
		}

		public string GetBoosterQuestTitle(BoosterType boosterType)
		{
			BoosterTypeConfig boosterTypeConfig = BoosterTypeConfigs.Find((BoosterTypeConfig e) => e.Type == boosterType);
			if (boosterTypeConfig == null)
			{
				return string.Empty;
			}
			return LocalizationUtility.Get("Localization_quest.json").GetString(boosterTypeConfig.QuestTitle);
		}

		public Sprite GetBoosterSprite(BoosterType boosterType)
		{
			return BoosterTypeConfigs.Find((BoosterTypeConfig e) => e.Type == boosterType)?.BoosterSprite;
		}

		public Sprite GetBoosterMiniSprite(BoosterType boosterType)
		{
			return BoosterTypeConfigs.Find((BoosterTypeConfig e) => e.Type == boosterType)?.BoosterMiniSprite;
		}

		public BoosterType GetUnLockBooster(ScheduleData schedule)
		{
			ScheduleData preSchedule = SingletonClass<AAOConfig>.Get().GetPreSchedule(schedule);
			return CardTypeNodeConfigs.FirstOrDefault((CardTypeNodeConfig e) => schedule.Equals(e.scheduleData))?.booster ?? ExtraTypeNodeConfigs.FirstOrDefault((ExtraTypeNodeConfig e) => schedule.Equals(e.scheduleData))?.booster ?? BoosterType.None;
		}
	}
}
