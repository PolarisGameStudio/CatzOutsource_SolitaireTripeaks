using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SaleData
	{
		public double hours;

		public int openDays;

		public int storeOpenCount;

		public int saleOpenCount;

		public int VideoOptimizationCount;

		public int FacebookOptimizationCount;

		public long LastOptimizationTimeLeft;

		public List<SaleInfo> Sales;

		public long DelayLowTime;

		public SaleData()
		{
			openDays = 0;
			hours = 0.0;
			Sales = new List<SaleInfo>();
		}

		public void RestbyNewDay()
		{
			openDays++;
		}

		public void Update()
		{
			hours += Time.unscaledDeltaTime / 60f / 60f;
		}

		public bool HasNormalSale()
		{
			return GetNormalSale() != null;
		}

		public SaleInfo GetNormalSale()
		{
			if (Sales == null)
			{
				return null;
			}
			return Sales.Find((SaleInfo e) => e.SaleConfig.Type.StartsWith("Sale") && !e.IsInvalid());
		}

		public bool HasSale()
		{
			return GetSaleInfos().Count((SaleInfo e) => e.IsRunning()) > 0;
		}

		public bool HasSale(string saleId)
		{
			if (Sales == null)
			{
				return false;
			}
			return Sales.Find((SaleInfo e) => e.IsRunning() && e.SaleConfig.Type.Equals(saleId)) != null;
		}

		public int GetOnlineSecond()
		{
			return (int)(hours * 3600.0);
		}

		public TimeSpan GetLeftTime()
		{
			if (Sales != null)
			{
				SaleInfo[] array = (from s in Sales
					where s.IsStart && !s.IsInvalid()
					orderby s.GetTimeSpan().TotalSeconds
					select s).ToArray();
				if (array.Length > 0)
				{
					return array[0].GetTimeSpan();
				}
			}
			return TimeSpan.FromSeconds(0.0);
		}

		public void RemoveInvalidSale()
		{
			List<SaleInfo> list = (from e in Sales
				where e.IsInvalid()
				select e).ToList();
			foreach (SaleInfo item in list)
			{
				if (item.SaleConfig.Type.StartsWith("Sale") && item.purchasings != null && item.purchasings.Count > 0)
				{
					DelayLowTime = DateTime.Now.Date.AddMonths(1).Ticks;
				}
				SingletonBehaviour<TripeaksLogUtility>.Get().UploadSale(item);
			}
			Sales.RemoveAll((SaleInfo e) => e.IsInvalid());
		}

		public bool CalcInvalidSale(UnityAction unityAction = null)
		{
			RemoveInvalidSale();
			if (Sales.Count((SaleInfo e) => e.IsRunning() && e.SaleConfig.Type.StartsWith("Sale")) == 0)
			{
				SaleInfo saleInfo = Sales.Find((SaleInfo e) => e.SaleConfig.Type.StartsWith("Sale"));
				if (saleInfo == null)
				{
					return false;
				}
				if (saleInfo.DOStart())
				{
					saleInfo.Show(unityAction);
					SingletonClass<OptimizationSystem>.Get().SaleChanged.Invoke();
					return true;
				}
			}
			return false;
		}

		public List<SaleInfo> GetSaleInfos()
		{
			if (Sales == null)
			{
				return new List<SaleInfo>();
			}
			return (from p in Sales
				where p.IsRunning()
				orderby p.GetTimeSpan().TotalMilliseconds
				select p).ToList();
		}

		public bool IsDelayByLowSale()
		{
			return DateTime.Now.Subtract(new DateTime(DelayLowTime)).TotalMilliseconds < 0.0;
		}

		public void PutNormalSale(List<string> boosterTypes, UnityAction unityAction)
		{
			if (Sales == null)
			{
				Sales = new List<SaleInfo>();
			}
			UnityEngine.Debug.LogFormat("本次促销参考等级: {0}, {1}", SingletonData<PurchasingData>.Get().Getlevel(), UnityPurchasingConfig.Get().GetLocalizedPrice(SingletonData<PurchasingData>.Get().Getlevel()));
			Sales.RemoveAll((SaleInfo e) => e.SaleConfig.Type.StartsWith("Sale"));
			foreach (string boosterType in boosterTypes)
			{
				string lowPackage = "yy_coin_local_sale_1_1";
				if (boosterType == "SaleCoins2" || boosterType == "SaleAssignPackage")
				{
					lowPackage = ((!IsDelayByLowSale()) ? "yy_coin_local_sale_1_1" : "yy_store_coin_1");
				}
				SaleItemConfig saleIndex = SaleConfig.GetNormalSale().GetSaleIndex(boosterType, SingletonData<PurchasingData>.Get().Getlevel(), lowPackage, SingletonData<PurchasingData>.Get().up);
				if (saleIndex != null)
				{
					UnityEngine.Debug.LogFormat("本次查找结果:{0}, {1}", saleIndex.level, UnityPurchasingConfig.Get().GetLocalizedPrice(saleIndex.level));
					if (saleIndex.IsUsable())
					{
						SaleInfo saleInfo = new SaleInfo();
						saleInfo.SaleConfig = saleIndex.Clone();
						SaleInfo saleInfo2 = saleInfo;
						Sales.Add(saleInfo2);
						if (Sales.Count((SaleInfo e) => e.IsRunning() && e.SaleConfig.Type.StartsWith("Sale")) == 0)
						{
							saleInfo2.DOStart();
							saleInfo2.Show();
							SingletonClass<OptimizationSystem>.Get().SaleChanged.Invoke();
						}
					}
				}
			}
			CalcInvalidSale(unityAction);
			PackData.Get().ClearTotal();
			SingletonData<PurchasingData>.Get().LevelDown();
		}

		public void PutStoreSale(PurchasingPackage package)
		{
			if (!(package.Type == "Store"))
			{
				return;
			}
			BoosterType boosterType = package.commoditys[0].boosterType;
			string type = string.Empty;
			switch (boosterType)
			{
			case BoosterType.Wild:
				type = "StoreSaleWild";
				break;
			case BoosterType.Rocket:
				type = "StoreSaleRocket";
				break;
			case BoosterType.Coins:
				type = "StoreSaleCoins";
				break;
			}
			if (string.IsNullOrEmpty(type))
			{
				return;
			}
			if (Sales == null)
			{
				Sales = new List<SaleInfo>();
			}
			SaleItemConfig saleIndex = SaleConfig.GetSpecialSale().GetSaleIndex(type, package.id);
			if (saleIndex != null)
			{
				SaleInfo saleInfo = Sales.Find((SaleInfo e) => e.SaleConfig.Type.Equals(type));
				if (saleInfo != null)
				{
					SingletonBehaviour<TripeaksLogUtility>.Get().UploadSale(saleInfo);
					Sales.Remove(saleInfo);
				}
				SaleInfo saleInfo2 = new SaleInfo();
				saleInfo2.SaleConfig = saleIndex.Clone();
				saleInfo = saleInfo2;
				saleInfo.DOStart();
				Sales.Add(saleInfo);
				saleInfo.Show();
				SingletonClass<OptimizationSystem>.Get().SaleChanged.Invoke();
			}
		}

		public void PutStoreSale(BoosterType booster)
		{
			if (AuxiliaryData.Get().IsDailyActive("SaleAssignPackage"))
			{
				SaleInfo saleInfo = Sales.Find((SaleInfo e) => e.SaleConfig.Type == "SaleAssignPackage");
				if (saleInfo != null)
				{
					SingletonBehaviour<TripeaksLogUtility>.Get().UploadSale(saleInfo);
				}
				Sales.RemoveAll((SaleInfo e) => e.SaleConfig.Type == "SaleAssignPackage");
				SaleItemConfig saleIndex = SaleConfig.GetNormalSale().GetSaleIndex("SaleAssignPackage", SingletonData<PurchasingData>.Get().Getlevel(), (!IsDelayByLowSale()) ? "yy_coin_local_sale_1_1" : "yy_store_coin_1", SingletonData<PurchasingData>.Get().up);
				if (saleIndex != null)
				{
					UnityEngine.Debug.LogFormat("本次查找结果:{0}, {1}", saleIndex.level, UnityPurchasingConfig.Get().GetLocalizedPrice(saleIndex.level));
					AuxiliaryData.Get().PutDailyCompleted("SaleAssignPackage");
					SaleInfo saleInfo2 = new SaleInfo();
					saleInfo2.SaleConfig = saleIndex.Clone();
					SaleInfo saleInfo3 = saleInfo2;
					saleInfo3.DOStart();
					saleInfo3.Show();
					Sales.Add(saleInfo3);
					SingletonClass<OptimizationSystem>.Get().SaleChanged.Invoke();
				}
			}
		}

		public bool PutHightScore(UnityAction unityAction)
		{
			if (AuxiliaryData.Get().IsDailyActive("HightScoreSale") && SingletonBehaviour<LeaderBoardUtility>.Get().GetRankTypeOffline() == RankType.Upload && SingletonBehaviour<LeaderBoardUtility>.Get().IsOepn && SingletonBehaviour<LeaderBoardUtility>.Get().GetRewardRemainTime().TotalDays < 1.0)
			{
				SaleInfo saleInfo = Sales.Find((SaleInfo e) => e.SaleConfig.Type == "HightScoreSale");
				if (saleInfo != null)
				{
					return false;
				}
				Sales.RemoveAll((SaleInfo e) => e.SaleConfig.Type == "HightScoreSale");
				SaleItemConfig saleIndex = SaleConfig.GetNormalSale().GetSaleIndex("HightScoreSale", SingletonData<PurchasingData>.Get().Getlevel(), "yy_coin_local_sale_1_1", SingletonData<PurchasingData>.Get().up);
				if (saleIndex != null)
				{
					UnityEngine.Debug.LogFormat("本次查找结果:{0}, {1}", saleIndex.level, UnityPurchasingConfig.Get().GetLocalizedPrice(saleIndex.level));
					AuxiliaryData.Get().PutDailyCompleted("HightScoreSale");
					SaleInfo saleInfo2 = new SaleInfo();
					saleInfo2.SaleConfig = saleIndex.Clone();
					SaleInfo saleInfo3 = saleInfo2;
					saleInfo3.DOStart();
					saleInfo3.Show(unityAction);
					Sales.Add(saleInfo3);
					SingletonClass<OptimizationSystem>.Get().SaleChanged.Invoke();
					return true;
				}
			}
			return false;
		}

		public bool IsOlderPlayer()
		{
			return openDays > 1 || hours > 0.33000001311302185;
		}

		public static SaleData Get()
		{
			if (SolitaireTripeaksData.Get().Promotions == null)
			{
				SolitaireTripeaksData.Get().Promotions = new SaleData();
			}
			return SolitaireTripeaksData.Get().Promotions;
		}
	}
}
