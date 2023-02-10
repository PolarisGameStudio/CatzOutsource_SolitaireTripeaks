using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class OptimizationSystem : SingletonClass<OptimizationSystem>
	{
		public UnityEvent SaleChanged = new UnityEvent();

		public bool Analysis(UnityAction unityAction)
		{
			if (SaleData.Get().HasNormalSale())
			{
				return false;
			}
			Func<int, bool> func = delegate(int day)
			{
				if (AuxiliaryData.Get().IsDailyActive("_dailySaleView"))
				{
					string id = $"__DaySale_{day}";
					if (StatisticsData.Get().GetInstallDays() >= day && !AuxiliaryData.Get().HasView(id) && (RandomSale(boosterAnalysis: true, unityAction) || RandomSale(boosterAnalysis: false, unityAction)))
					{
						SaleData.Get().LastOptimizationTimeLeft = DateTime.Now.Ticks;
						AuxiliaryData.Get().PutDailyCompleted("_dailySaleView");
						AuxiliaryData.Get().PutView(id);
						return true;
					}
				}
				return false;
			};
			if (func(1) || func(6) || func(29))
			{
				return true;
			}
			if (MathUtility.CalcDays(DateTime.Now, new DateTime(SaleData.Get().LastOptimizationTimeLeft)) >= 3 && SaleData.Get().IsOlderPlayer())
			{
				SaleData.Get().LastOptimizationTimeLeft = DateTime.Now.Ticks;
				if (RandomSale(boosterAnalysis: true, unityAction))
				{
					return true;
				}
				BoosterCommodity commodity = PackData.Get().GetCommodity(BoosterType.Coins);
				if (commodity.sourceNumbers.Find((CommodityNumber p) => p.source == CommoditySource.Buy) == null)
				{
					if (commodity.sourceNumbers.Find((CommodityNumber p) => p.source == CommoditySource.Video) == null)
					{
						SaleData.Get().VideoOptimizationCount++;
						TipPopupHasIconScene.ShowWatchVideoOptimization(delegate
						{
							if (SaleData.Get().VideoOptimizationCount >= 2)
							{
								RandomSale(boosterAnalysis: false, unityAction);
							}
							else if (unityAction != null)
							{
								unityAction();
							}
						});
						return true;
					}
					if (commodity.sourceNumbers.Find((CommodityNumber p) => p.source == CommoditySource.Task) == null)
					{
						SaleData.Get().FacebookOptimizationCount++;
						SingletonBehaviour<GlobalConfig>.Get().ShowLoginFacebook(bonus: true, delegate
						{
							if (SaleData.Get().FacebookOptimizationCount >= 2)
							{
								RandomSale(boosterAnalysis: false, unityAction);
							}
							else if (unityAction != null)
							{
								unityAction();
							}
						});
						return true;
					}
					return RandomSale(boosterAnalysis: false, unityAction);
				}
				if (RandomSale(boosterAnalysis: false, unityAction))
				{
					return true;
				}
			}
			return false;
		}

		private bool RandomSale(bool boosterAnalysis, UnityAction unityAction)
		{
			List<string> list = new List<string>();
			switch (StatisticsData.Get().GetBoosterMaxUse())
			{
			case BoosterType.Coins:
			{
				if (boosterAnalysis)
				{
					return false;
				}
				List<string> list2 = new List<string>();
				list2.Add("SaleCoins2");
				list2.Add("SaleWild");
				list2.Add("SaleRocket");
				list = list2;
				break;
			}
			case BoosterType.Wild:
			{
				List<string> list2 = new List<string>();
				list2.Add("SaleWild");
				list2.Add("SaleCoins2");
				list2.Add("SaleRocket");
				list = list2;
				break;
			}
			case BoosterType.Rocket:
			{
				List<string> list2 = new List<string>();
				list2.Add("SaleRocket");
				list2.Add("SaleCoins2");
				list2.Add("SaleWild");
				list = list2;
				break;
			}
			default:
			{
				List<string> list2 = new List<string>();
				list2.Add("SaleWild");
				list2.Add("SaleRocket");
				list2.Add("SaleCoins2");
				list = list2;
				break;
			}
			}
			if (StatisticsData.Get().Purchaseds != null && StatisticsData.Get().Purchaseds.Find((PurchasingPackage e) => e.Type == "StorePackage") != null)
			{
				list.Insert(0, "SaleMegaPackage");
			}
			else
			{
				list.Add("SaleMegaPackage");
			}
			list.Add("SaleAssignPackage");
			list.Add("SaleSpecialStore");
			SaleData.Get().PutNormalSale(list, unityAction);
			return SaleData.Get().HasNormalSale();
		}
	}
}
