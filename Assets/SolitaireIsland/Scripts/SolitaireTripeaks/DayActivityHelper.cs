using Nightingale.Localization;
using Nightingale.U2D;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class DayActivityHelper : SingletonClass<DayActivityHelper>
	{
		public bool HasDayActivty(DayActivityType type)
		{
			if (QuestData.Get().QuestOpen)
			{
				List<DayActivityConfig> dayActivityConfigs = GetDayActivityConfigs();
				return dayActivityConfigs.Find((DayActivityConfig e) => e.Type == type && e.Time.Date.Equals(DateTime.Now.Date)) != null;
			}
			return false;
		}

		public List<DayActivityConfig> GetDayActivityConfigs()
		{
			return GetDayActivityConfigs(DateTime.Today);
		}

		public List<DayActivityConfig> GetNextDayActivityConfigs()
		{
			List<DayActivityConfig> dayActivityConfigs = GetDayActivityConfigs(DateTime.Today);
			dayActivityConfigs.AddRange(GetDayActivityConfigs(DateTime.Today.AddDays(7.0)));
			return dayActivityConfigs;
		}

		public List<DayActivityConfig> GetDayActivityConfigs(DateTime Today)
		{
			DateTime dateTime = Today.Date.AddDays(0 - Today.Date.DayOfWeek);
			if (Today.Date.DayOfWeek == DayOfWeek.Sunday)
			{
				dateTime = Today.Date.AddDays(-7.0);
			}
			UnityEngine.Debug.LogFormat("Week Start : {0}", dateTime);
			int num = (int)Today.Date.Subtract(new DateTime(2018, 6, 18)).TotalDays;
			if (num < 0)
			{
				return new List<DayActivityConfig>();
			}
			if (num / 7 % 2 == 0)
			{
				List<DayActivityConfig> list = new List<DayActivityConfig>();
				list.Add(new DayActivityConfig
				{
					Time = dateTime.AddDays(6.0),
					Type = DayActivityType.DoubleTreause
				});
				return list;
			}
			if (num / 7 % 2 == 1)
			{
				List<DayActivityConfig> list = new List<DayActivityConfig>();
				list.Add(new DayActivityConfig
				{
					Time = dateTime.AddDays(5.0),
					Type = DayActivityType.DoubleBuyStep
				});
				return list;
			}
			return new List<DayActivityConfig>();
		}

		public List<NewsConfig> GetNewsConfigs()
		{
			List<DayActivityConfig> dayActivityConfigs = GetDayActivityConfigs();
			dayActivityConfigs.RemoveAll((DayActivityConfig e) => e.IsInvalid());
			return (from e in dayActivityConfigs
				select new NewsConfig
				{
					icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_fox"),
					identifier = e.Time.ToString("yyyy/M/d"),
					title = LocalizationUtility.Get("Localization_inbox.json").GetString("JOIN_THE_FUN"),
					description = e.GetDescriptionInBox(),
					Order = e.Time
				}).ToList();
		}
	}
}
