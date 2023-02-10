using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SpecialActivityData
	{
		public int Numbers;

		public int TotalNumbers;

		public int SpecialSaleNumbers;

		public int OpenSpecialSaleNumbers;

		public int OpenSpecialNumbers;

		public long Ticks;

		public string identifier;

		public List<ScheduleData> ScheduleDatas;

		public List<PurchasingCommodity> Exchanges;

		public SpecialActivityData()
		{
			Exchanges = new List<PurchasingCommodity>();
			ScheduleDatas = new List<ScheduleData>();
			Numbers = 0;
			TotalNumbers = 0;
			OpenSpecialSaleNumbers = 0;
			OpenSpecialNumbers = 0;
			SpecialSaleNumbers = 0;
			identifier = string.Empty;
			Ticks = DateTime.Now.Ticks;
		}

		public void Put()
		{
			Ticks = DateTime.Now.Ticks;
		}

		public static SpecialActivityData Get()
		{
			if (SolitaireTripeaksData.Get().SpecialActivity == null)
			{
				SolitaireTripeaksData.Get().SpecialActivity = new SpecialActivityData();
			}
			return SolitaireTripeaksData.Get().SpecialActivity;
		}

		public int GetNumber(BoosterType boosterType)
		{
			return Exchanges.Find((PurchasingCommodity e) => e.boosterType == boosterType)?.count ?? 0;
		}

		public void ExhangeBooster(BoosterType boosterType)
		{
			PurchasingCommodity purchasingCommodity = Exchanges.Find((PurchasingCommodity e) => e.boosterType == boosterType);
			if (purchasingCommodity == null)
			{
				Exchanges.Add(new PurchasingCommodity
				{
					boosterType = boosterType,
					count = 1
				});
			}
			else
			{
				purchasingCommodity.count++;
			}
		}

		public ScheduleData RandomScheduleData()
		{
			return ScheduleDatas[UnityEngine.Random.Range(0, ScheduleDatas.Count)];
		}

		public void SetNewEvent(string id)
		{
			if (string.IsNullOrEmpty(identifier) && ScheduleDatas.Count > 0)
			{
				identifier = id;
			}
			if (!identifier.Equals(id))
			{
				OpenSpecialSaleNumbers = 0;
				OpenSpecialNumbers = 0;
				SpecialSaleNumbers = 0;
				Ticks = DateTime.Now.Ticks;
				Exchanges.Clear();
				ScheduleDatas.Clear();
				Numbers = 0;
				TotalNumbers = 0;
				identifier = id;
				AuxiliaryData.Get().RemoveView("Special_Active");
			}
		}

		public void Collect()
		{
			Numbers++;
			TotalNumbers++;
		}

		public bool Use(int number)
		{
			if (Numbers >= number)
			{
				Numbers -= number;
				return true;
			}
			return false;
		}
	}
}
