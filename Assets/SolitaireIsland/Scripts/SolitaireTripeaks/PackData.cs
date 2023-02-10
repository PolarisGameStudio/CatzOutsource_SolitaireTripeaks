using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PackData
	{
		public List<BoosterCommodity> commoditys;

		public List<BoosterType> boosters;

		public PackData()
		{
			commoditys = new List<BoosterCommodity>();
			boosters = new List<BoosterType>();
		}

		public static PackData Get()
		{
			if (SolitaireTripeaksData.Get().Pack == null)
			{
				SolitaireTripeaksData.Get().Pack = new PackData();
			}
			return SolitaireTripeaksData.Get().Pack;
		}

		public void AddBoosterType(BoosterType type)
		{
			if (boosters == null)
			{
				boosters = new List<BoosterType>();
			}
			if (!ContainsBooster(type))
			{
				boosters.Add(type);
			}
		}

		public void RemoveBoosterType(BoosterType type)
		{
			if (boosters != null && ContainsBooster(type))
			{
				boosters.Remove(type);
			}
		}

		public bool ContainsBooster(BoosterType type)
		{
			if (boosters == null)
			{
				return false;
			}
			return boosters.Contains(type);
		}

		public List<BoosterType> UseBoosters(List<BoosterType> allow)
		{
			if (boosters == null)
			{
				return new List<BoosterType>();
			}
			List<BoosterType> list = (from e in allow
				where ContainsBooster(e)
				select e).ToList();
			foreach (BoosterType item in list)
			{
				RemoveBoosterType(item);
				SessionData.Get().UseCommodity(item, 1L);
			}
			return list;
		}

		public void UseBoosters(BoosterType boosterType)
		{
			RemoveBoosterType(boosterType);
			SessionData.Get().UseCommodity(boosterType, 1L);
		}

		public BoosterCommodity GetCommodity(BoosterType boosterType)
		{
			BoosterCommodity boosterCommodity = commoditys.Find((BoosterCommodity e) => e.boosterType == boosterType);
			if (boosterCommodity == null)
			{
				boosterCommodity = new BoosterCommodity(boosterType);
				commoditys.Add(boosterCommodity);
			}
			return boosterCommodity;
		}

		public CommoditySource GetCoinHight()
		{
			BoosterCommodity commodity = GetCommodity(BoosterType.Coins);
			if (commodity != null)
			{
				for (int num = commodity.sourceNumbers.Count - 1; num >= 0; num--)
				{
					if (commodity.sourceNumbers[num].total > 0)
					{
						return commodity.sourceNumbers[num].source;
					}
				}
			}
			return CommoditySource.Free;
		}

		public void ClearTotal()
		{
			foreach (BoosterCommodity commodity in commoditys)
			{
				for (int i = 0; i < commodity.sourceNumbers.Count; i++)
				{
					CommodityNumber commodityNumber = commodity.sourceNumbers[i];
				}
				foreach (CommodityNumber sourceNumber in commodity.sourceNumbers)
				{
					sourceNumber.total = sourceNumber.current;
				}
			}
		}

		public string ToJson()
		{
			string text = string.Empty;
			foreach (BoosterCommodity commodity in commoditys)
			{
				if (commodity.sourceNumbers.Count > 0)
				{
					text = text + commodity.boosterType.ToString() + ":";
					for (int i = 0; i < commodity.sourceNumbers.Count; i++)
					{
						CommodityNumber commodityNumber = commodity.sourceNumbers[i];
						string text2 = text;
						text = text2 + commodityNumber.source.ToString() + "|" + commodityNumber.current;
						text = ((i != commodity.sourceNumbers.Count - 1) ? (text + ",") : (text + ";"));
					}
				}
			}
			return text;
		}
	}
}
