using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class BoosterCommodity
	{
		public BoosterType boosterType;

		public List<CommodityNumber> sourceNumbers;

		private BoosterCommodityChanged onChanged;

		public BoosterCommodityChanged OnChanged
		{
			get
			{
				if (onChanged == null)
				{
					onChanged = new BoosterCommodityChanged();
				}
				return onChanged;
			}
		}

		public BoosterCommodity(BoosterType booster)
		{
			boosterType = booster;
			sourceNumbers = new List<CommodityNumber>();
		}

		public long GetTotal()
		{
			long num = 0L;
			foreach (CommodityNumber sourceNumber in sourceNumbers)
			{
				num += sourceNumber.GetCurrent();
			}
			return num;
		}

		public bool Use(long number = 1L)
		{
			if (GetTotal() < number)
			{
				return false;
			}
			foreach (CommodityNumber sourceNumber in sourceNumbers)
			{
				number = sourceNumber.Use(number);
				if (number <= 0)
				{
					break;
				}
			}
			PutChanged(CommoditySource.None);
			AuxiliaryData.Get().PutCommodity(boosterType, (int)number);
			return true;
		}

		public void ForceUse(long number)
		{
			foreach (CommodityNumber sourceNumber in sourceNumbers)
			{
				number = sourceNumber.Use(number);
				if (number <= 0)
				{
					break;
				}
			}
			PutChanged(CommoditySource.None);
		}

		public void PutChanged(CommoditySource source)
		{
			OnChanged.Invoke(source);
		}

		public void Put(CommoditySource source, long count, bool changed = true)
		{
			CommodityNumber commodityNumber = sourceNumbers.Find((CommodityNumber e) => e.source == source);
			if (commodityNumber == null)
			{
				sourceNumbers.Add(new CommodityNumber(source, count));
				sourceNumbers = (from e in sourceNumbers
					orderby e.source descending
					select e).ToList();
				if (changed)
				{
					PutChanged(source);
				}
			}
			else
			{
				commodityNumber.Put(count);
				if (changed)
				{
					PutChanged(source);
				}
			}
		}

		public void Set(CommoditySource source, int count)
		{
			CommodityNumber commodityNumber = sourceNumbers.Find((CommodityNumber e) => e.source == source);
			if (commodityNumber == null)
			{
				sourceNumbers.Add(new CommodityNumber(source, count));
				sourceNumbers = (from e in sourceNumbers
					orderby e.source descending
					select e).ToList();
				PutChanged(source);
			}
			else
			{
				commodityNumber.Set(count);
				PutChanged(source);
			}
		}

		public long TotalUse()
		{
			long num = 0L;
			for (int num2 = sourceNumbers.Count - 1; num2 >= 0; num2--)
			{
				num += sourceNumbers[num2].total - sourceNumbers[num2].current;
			}
			return num;
		}

		public void ForUseBack(CommoditySource source)
		{
			CommodityNumber commodityNumber = sourceNumbers.Find((CommodityNumber e) => e.source == source);
			if (commodityNumber != null)
			{
				commodityNumber.total--;
				if (commodityNumber.total < 0)
				{
					commodityNumber.total = 0L;
				}
			}
		}
	}
}
