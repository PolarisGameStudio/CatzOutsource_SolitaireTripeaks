using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SessionData
	{
		public string SassionId;

		public long StartTime;

		public long EndTime;

		public string RemainNumbers;

		public List<UseNumber> UseNumbers;

		public List<SoureNumber> SoureNumbers;

		public static SessionData Get()
		{
			SessionData sessionData = SingletonData<SessionGroup>.Get().GetSassionData();
			if (sessionData == null)
			{
				sessionData = new SessionData();
			}
			return sessionData;
		}

		public bool UseCommodity(BoosterType type, long number = 1L, string source = "None")
		{
			ClearBoosters();
			if (PackData.Get().GetCommodity(type).Use(number))
			{
				if (type == BoosterType.Coins)
				{
					StatisticsData.Get().UseCoins(source);
				}
				StatisticsData.Get().UseBooster(type, number);
				if (UseNumbers == null)
				{
					UseNumbers = new List<UseNumber>();
				}
				UseNumber useNumber = UseNumbers.Find((UseNumber e) => e.Source == source && e.Type == type.ToString());
				if (useNumber == null)
				{
					UseNumber useNumber2 = new UseNumber();
					useNumber2.Source = source;
					useNumber2.Type = type.ToString();
					useNumber = useNumber2;
					UseNumbers.Add(useNumber);
				}
				useNumber.Numbers += number;
				return true;
			}
			return false;
		}

		public void PutCommodity(BoosterType boosterType, CommoditySource source, long number, bool changed = true)
		{
			ClearBoosters();
			if (boosterType != BoosterType.UnlimitedPlay)
			{
				if (boosterType == BoosterType.UnlimitedDoubleStar)
				{
					RankCoinData.Get().AppendDoubleStarByMinutes(number);
				}
				else
				{
					PackData.Get().GetCommodity(boosterType).Put(source, number, changed);
				}
			}
			else
			{
				AuxiliaryData.Get().AppendUnlimitedByMinutes(number);
			}
			if (SoureNumbers == null)
			{
				SoureNumbers = new List<SoureNumber>();
			}
			SoureNumber soureNumber = SoureNumbers.Find((SoureNumber e) => e.Source == source.ToString() && e.Type == boosterType.ToString());
			if (soureNumber == null)
			{
				SoureNumber soureNumber2 = new SoureNumber();
				soureNumber2.Source = source.ToString();
				soureNumber2.Type = boosterType.ToString();
				soureNumber = soureNumber2;
				SoureNumbers.Add(soureNumber);
			}
			soureNumber.Numbers += number;
		}

		public void ClearBoosters()
		{
			if (AuxiliaryData.Get().ClearBoostersTicks == 0)
			{
				AuxiliaryData.Get().ClearBoostersTicks = DateTime.Today.AddDays(1.0).Ticks;
				return;
			}
			TimeSpan timeSpan = new DateTime(AuxiliaryData.Get().ClearBoostersTicks).Subtract(DateTime.Now);
			if (timeSpan.TotalSeconds < 0.0 || timeSpan.TotalHours > 24.0)
			{
				PackData.Get().commoditys.RemoveAll((BoosterCommodity e) => e.boosterType == BoosterType.ExpiredPlay);
				AuxiliaryData.Get().ClearBoostersTicks = DateTime.Today.AddDays(1.0).Ticks;
				UnityEngine.Debug.Log($"过期FreePlay 时间异常，被自动移除！！ 剩余总时间：{timeSpan.TotalHours}");
			}
		}

		public string ToUseJson()
		{
			string text = string.Empty;
			if (UseNumbers == null)
			{
				return text;
			}
			foreach (UseNumber useNumber in UseNumbers)
			{
				text += $"{useNumber.Type}_{useNumber.Source}_{useNumber.Numbers}|";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public string ToSoureJson()
		{
			string text = string.Empty;
			if (SoureNumbers == null)
			{
				return text;
			}
			foreach (SoureNumber soureNumber in SoureNumbers)
			{
				text += $"{soureNumber.Type}_{soureNumber.Source}_{soureNumber.Numbers}|";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}
	}
}
