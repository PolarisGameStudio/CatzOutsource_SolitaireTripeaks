using Nightingale.Extensions;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class StatisticsData
	{
		public string TransactionID;

		public long InstallTicks;

		public long LoginTicks;

		public List<PurchasingPackage> Purchaseds;

		public List<UseNumber> Uses;

		public StatisticsData()
		{
			InstallTicks = DateTime.UtcNow.Ticks;
		}

		public static StatisticsData Get()
		{
			if (SolitaireTripeaksData.Get().Statistics == null)
			{
				SolitaireTripeaksData.Get().Statistics = new StatisticsData();
			}
			return SolitaireTripeaksData.Get().Statistics;
		}

		public bool IsCheatPlayer()
		{
			if (Purchaseds == null)
			{
				Purchaseds = new List<PurchasingPackage>();
			}
			if (PackData.Get().GetCommodity(BoosterType.FreePlay).GetTotal() > Purchaseds.Sum((PurchasingPackage e) => e.commoditys.Sum((PurchasingCommodity c) => (c.boosterType == BoosterType.FreePlay) ? c.count : 0)) + 5000)
			{
				return true;
			}
			if (PackData.Get().GetCommodity(BoosterType.Rocket).GetTotal() > Purchaseds.Sum((PurchasingPackage e) => e.commoditys.Sum((PurchasingCommodity c) => (c.boosterType == BoosterType.Rocket) ? c.count : 0)) + 5000)
			{
				return true;
			}
			if (PackData.Get().GetCommodity(BoosterType.Wild).GetTotal() > Purchaseds.Sum((PurchasingPackage e) => e.commoditys.Sum((PurchasingCommodity c) => (c.boosterType == BoosterType.Wild) ? c.count : 0)) + 5000)
			{
				return true;
			}
			return false;
		}

		public void PutPurchasingPackage(string transactionID, PurchasingPackage package)
		{
			if (Purchaseds == null)
			{
				Purchaseds = new List<PurchasingPackage>();
			}
			Purchaseds.Add(package);
			PurchasingCommodity purchasingCommodity = package.commoditys.ToList().Find((PurchasingCommodity e) => e.boosterType == BoosterType.Coins);
			if (purchasingCommodity != null)
			{
				TransactionID = transactionID;
			}
		}

		public void UseBooster(BoosterType booster, long number)
		{
			if (booster == BoosterType.Wild || booster == BoosterType.Rocket || booster >= BoosterType.RandomBooster)
			{
				try
				{
					if (Uses == null)
					{
						Uses = new List<UseNumber>();
					}
					UseNumber useNumber = Uses.Find((UseNumber e) => e.Type == booster.ToString() && DateTime.Parse(e.Source).Date == DateTime.Now);
					if (useNumber == null)
					{
						UseNumber useNumber2 = new UseNumber();
						useNumber2.Numbers = number;
						useNumber2.Type = booster.ToString();
						useNumber2.Source = DateTime.Now.ToString();
						useNumber = useNumber2;
						Uses.Add(useNumber);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public BoosterType GetBoosterMaxUse()
		{
			if (Uses == null)
			{
				return BoosterType.Coins;
			}
			Uses.RemoveAll((UseNumber e) => DateTime.Now.Subtract(DateTime.Parse(e.Source).Date).Days >= 5);
			var list = (from e in Uses
				group e by e.Type into e
				select new
				{
					Key = e.Key,
					Sum = e.Sum((UseNumber x) => x.Numbers)
				} into e
				orderby e.Sum descending
				select e).ToList();
			if (list.Count == 0)
			{
				return BoosterType.Coins;
			}
			return (BoosterType)Enum.Parse(typeof(BoosterType), list[0].Key);
		}

		public List<BoosterType> GetBoosters(int number)
		{
			List<BoosterType> boosters = new List<BoosterType>();
			if (Uses == null)
			{
				boosters = AppearNodeConfig.Get().GetBoosters().Random(number);
			}
			else
			{
				Uses.RemoveAll((UseNumber e) => DateTime.Now.Subtract(DateTime.Parse(e.Source).Date).Days >= 5);
				try
				{
					boosters = (from e in Uses
						group e by e.Type into e
						select new
						{
							Key = e.Key,
							Sum = e.Sum((UseNumber x) => x.Numbers)
						} into e
						orderby e.Sum descending
						select EnumUtility.GetEnumType(e.Key, BoosterType.Coins)).ToList();
				}
				catch (Exception)
				{
				}
			}
			boosters.Remove(BoosterType.Wild);
			boosters.Remove(BoosterType.Rocket);
			if (boosters.Count >= number)
			{
				boosters = boosters.GetRange(0, number);
			}
			else
			{
				boosters.AddRange((from b in AppearNodeConfig.Get().GetBoosters()
					where !boosters.Contains(b)
					select b).ToList().Random(number - boosters.Count));
			}
			return boosters;
		}

		public void UseCoins(string behaviour)
		{
			if (!string.IsNullOrEmpty(TransactionID))
			{
				SingletonBehaviour<TripeaksLogUtility>.Get().UploadUsePurchasingPackage(TransactionID, behaviour);
				TransactionID = string.Empty;
			}
		}

		public bool IsPurchasingPlayer()
		{
			if (Purchaseds == null)
			{
				return false;
			}
			return Purchaseds.Count > 0;
		}

		public bool IsLowPlayer()
		{
			if (DateTime.Now.Subtract(new DateTime(InstallTicks)).TotalDays >= 30.0)
			{
				return Purchaseds == null || Purchaseds.Count == 0;
			}
			return false;
		}

		public int GetInstallDays()
		{
			return (int)DateTime.Now.Subtract(new DateTime(InstallTicks)).TotalDays;
		}
	}
}
