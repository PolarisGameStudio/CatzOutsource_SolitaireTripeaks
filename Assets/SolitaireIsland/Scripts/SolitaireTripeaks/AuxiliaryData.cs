using Nightingale.Localization;
using Nightingale.Notifications;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class AuxiliaryData
	{
		public int DailyBonusCount;

		public int FreeCoinsCount;

		public bool FreeCoinsTips;

		public long CoinBankReward;

		public bool RewardsFacebook;

		public int WatchVideoCount;

		public bool WatchVideoRewardWild;

		public int WatchVideoRewardWildCount;

		public int WatchVideoTotal;

		public int DailyBonusRewards;

		public int PlayNumber;

		public int RowDays;

		public bool RewardRowDay;

		public bool RewardRowOpen;

		public bool RocketOpen;

		public bool RopeTips;

		public bool SpecialSaleReward;

		public int RewardRowDays;

		public bool IsFacebookReward;

		public bool IsInboxOpen;

		public List<string> _Friends;

		public List<string> _RequestInvitableFriends;

		public int _RequestInvitableFriendsCount;

		public int _FacebookRewardNumber;

		public string AvaterFileName;

		public string NickName;

		public List<string> InvitableFriended;

		public List<ChapterScheduleData> UnLocks;

		public List<ChapterScheduleData> VisitIslands;

		public int JackpotNumber;

		public string JackpotId;

		public int PlaySlotsNumber;

		public int PlayWheelNumber;

		public bool PlayerRated;

		public int BuyHandNumber;

		public bool HelpHand;

		public int FreeUndoTotal;

		public int FreeHandTotal;

		public bool IsCheckPlayNumber;

		public bool ShowWeekEvent;

		public bool ShowDailyEvent;

		public List<ScheduleData> rates;

		public bool LeaderBoardOpen;

		public int WatchAdInLevelCount;

		public int WatchAdInLevelShow;

		public int LeaderBoardInLevelEndShow;

		public int LeaderBoardInLevelEndClick;

		public int LeaderBoardInLevelEndBoost;

		public List<string> Views;

		public List<PurchasingCommodity> Commoditys;

		public List<string> Dailys;

		public List<string> DailyNumbers;

		public long UnlimitedPlayTicks;

		public long ClearBoostersTicks;

		public List<ScheduleData> TreasureSchedules;

		public int MaxLevel;

		public int MaxMasterLevel;

		public SystemMessageData __ReceiveMessage;

		public SystemMessageData __SendMessages;

		private const int OneStepMinutes = 20;

		private const int OneStepCoins = 1000;

		private const int TotalMinutes = 720;

		public AuxiliaryData()
		{
			UnLocks = new List<ChapterScheduleData>
			{
				default(ChapterScheduleData)
			};
			AvaterFileName = string.Empty;
			RowDays = 1;
			_Friends = new List<string>();
			InvitableFriended = new List<string>();
			_RequestInvitableFriends = new List<string>();
			VisitIslands = new List<ChapterScheduleData>();
			Views = new List<string>();
			PutView("_BoosterCurrencyTipPopup");
			HelpHand = true;
		}

		public bool IsTreasure(ScheduleData schedule)
		{
			if (TreasureSchedules == null)
			{
				TreasureSchedules = new List<ScheduleData>();
			}
			if (PlayData.Get().HasLevelData(schedule))
			{
				return false;
			}
			if (TreasureSchedules.Contains(schedule))
			{
				return false;
			}
			if (schedule.world >= 0 && (SingletonClass<AAOConfig>.Get().GetLevelInWorld(schedule) - 9) % 10 == 0)
			{
				return true;
			}
			return false;
		}

		public void CollectTreasure(ScheduleData schedule)
		{
			if (TreasureSchedules == null)
			{
				TreasureSchedules = new List<ScheduleData>();
			}
			if (!TreasureSchedules.Contains(schedule))
			{
				TreasureSchedules.Add(schedule);
			}
		}

		public static AuxiliaryData Get()
		{
			if (SolitaireTripeaksData.Get().Auxiliary == null)
			{
				SolitaireTripeaksData.Get().Auxiliary = new AuxiliaryData();
			}
			return SolitaireTripeaksData.Get().Auxiliary;
		}

		public void PutView(string id)
		{
			if (Views == null)
			{
				Views = new List<string>();
			}
			if (!Views.Contains(id))
			{
				Views.Add(id);
			}
		}

		public void RemoveView(string id)
		{
			if (Views != null && !Views.Contains(id))
			{
				Views.Remove(id);
			}
		}

		public bool HasView(string id)
		{
			if (Views == null)
			{
				return false;
			}
			return Views.Contains(id);
		}

		public string GetCommoditys()
		{
			if (Commoditys == null)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < Commoditys.Count; i++)
			{
				PurchasingCommodity purchasingCommodity = Commoditys[i];
				if (i != 0)
				{
					text += ";";
				}
				text += $"{purchasingCommodity.boosterType.ToString()}_{purchasingCommodity.count}_{PackData.Get().GetCommodity(purchasingCommodity.boosterType).GetTotal()}";
			}
			return text;
		}

		public void PutCommodity(BoosterType booster, int count = 1)
		{
			if (Commoditys == null)
			{
				Commoditys = new List<PurchasingCommodity>();
			}
			PurchasingCommodity purchasingCommodity = Commoditys.Find((PurchasingCommodity e) => e.boosterType == booster);
			if (purchasingCommodity == null)
			{
				Commoditys.Add(new PurchasingCommodity
				{
					boosterType = booster,
					count = count
				});
			}
			else
			{
				purchasingCommodity.count += count;
			}
		}

		public void CheckPlayNumber()
		{
			if (!IsCheckPlayNumber)
			{
				if (PlayNumber == 0)
				{
					FreeUndoTotal = 1;
					FreeHandTotal = 1;
					UnityEngine.Debug.Log("...... New User Tip Undo/Add Hand.");
				}
				IsCheckPlayNumber = true;
			}
		}

		public bool HasVisitIsland(int world, int chapter)
		{
			if (VisitIslands == null)
			{
				return false;
			}
			return VisitIslands.Count((ChapterScheduleData p) => p.world == world && p.chapter == chapter) > 0;
		}

		public void PutVisitIsland(int world, int chapter)
		{
			if (!HasVisitIsland(world, chapter))
			{
				if (VisitIslands == null)
				{
					VisitIslands = new List<ChapterScheduleData>();
				}
				VisitIslands.Add(new ChapterScheduleData(world, chapter));
			}
		}

		public void PutFriends(List<string> id)
		{
			_Friends.AddRange(id);
		}

		public void PutRequestInvitableFriends(List<string> id)
		{
			_RequestInvitableFriendsCount += id.Count;
			if (_RequestInvitableFriends == null)
			{
				_RequestInvitableFriends = new List<string>();
			}
			_RequestInvitableFriends.AddRange(id);
		}

		public void RestbyNewDay()
		{
			_FacebookRewardNumber = 0;
			_RequestInvitableFriendsCount = 0;
			WatchVideoCount = 0;
			DailyBonusRewards = 3000;
			_Friends = new List<string>();
			_RequestInvitableFriends = new List<string>();
			RewardRowDay = false;
			WatchVideoRewardWild = false;
			ShowWeekEvent = false;
			ShowDailyEvent = false;
			Dailys = new List<string>();
			DailyNumbers = new List<string>();
		}

		public void DoRowDay(bool isRowDay)
		{
			if (isRowDay)
			{
				if (RewardRowOpen)
				{
					RewardRowDays++;
				}
				RowDays++;
			}
			else
			{
				RewardRowDays = 1;
				RowDays = 1;
			}
		}

		public string GetNickName()
		{
			if (string.IsNullOrEmpty(NickName))
			{
				return "Guest";
			}
			return NickName;
		}

		public void PutDailyCompleted(string daily)
		{
			if (Dailys == null)
			{
				Dailys = new List<string>();
			}
			if (!Dailys.Contains(daily))
			{
				Dailys.Add(daily);
			}
		}

		public bool IsDailyActive(string daily)
		{
			if (Dailys == null)
			{
				return true;
			}
			return !Dailys.Contains(daily);
		}

		public void PutDailyOnce(string daily)
		{
			if (!string.IsNullOrEmpty(daily))
			{
				if (DailyNumbers == null)
				{
					DailyNumbers = new List<string>();
				}
				DailyNumbers.Add(daily);
			}
		}

		public int GetDailyNumber(string daily)
		{
			if (Dailys == null)
			{
				return 0;
			}
			return DailyNumbers.Count((string e) => e == daily);
		}

		private void RefUnlimitedPlay()
		{
			if (UnlimitedPlayTicks != 0 && new DateTime(UnlimitedPlayTicks).Subtract(DateTime.Now).TotalHours > 24.0)
			{
				UnlimitedPlayTicks = DateTime.Now.Ticks;
			}
		}

		public bool IsUnlimitedPlay()
		{
			if (UnlimitedPlayTicks == 0)
			{
				return false;
			}
			RefUnlimitedPlay();
			return new DateTime(UnlimitedPlayTicks).Subtract(DateTime.Now).TotalSeconds > 0.0;
		}

		public double GetUnlimitedPlayRemain()
		{
			RefUnlimitedPlay();
			return new DateTime(UnlimitedPlayTicks).Subtract(DateTime.Now).TotalSeconds;
		}

		public void AppendUnlimitedByMinutes(long minutes)
		{
			RefUnlimitedPlay();
			if (IsUnlimitedPlay())
			{
				UnlimitedPlayTicks = new DateTime(UnlimitedPlayTicks).AddMinutes(minutes).Ticks;
			}
			else
			{
				UnlimitedPlayTicks = DateTime.Now.AddMinutes(minutes).Ticks;
			}
		}

		public bool IsCoinBankMax()
		{
			return GetCoinBack() >= CoinsLimit();
		}

		public bool IsCollect()
		{
			return GetCoinBack() >= 1000 && SystemTime.IsConnect;
		}

		public int CoinsLimit()
		{
			if (WatchVideoRewardWildCount < 6)
			{
				return 5000;
			}
			return 1000;
		}

		public int GetCoinBack()
		{
			if (SystemTime.IsConnect)
			{
				if (CoinBankReward == 0)
				{
					CoinBankReward = SystemTime.Now.AddMinutes(-19.5).Ticks;
				}
				DateTime value = new DateTime(CoinBankReward);
				double totalMinutes = SystemTime.Now.Subtract(value).TotalMinutes;
				int num = 0;
				return Mathf.Min(b: Mathf.Max(0, (!(totalMinutes <= 20.0)) ? (1000 + (int)((totalMinutes - 20.0) * (double)(CoinsLimit() - 1000) / 700.0)) : ((int)(totalMinutes * 1000.0 / 20.0))), a: CoinsLimit());
			}
			return 0;
		}

		public void NotificationOfflineCoins()
		{
			if (PlayData.Get().HasThanLevelData(0, 0, 0))
			{
				LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Notification.json");
				TimeSpan timeSpan = new DateTime(CoinBankReward).AddMinutes(20.0).Subtract(DateTime.Now);
				if (timeSpan.TotalSeconds > 0.0)
				{
#if ENABLE_LOCAL_NOTIFICATION
                    Nightingale.Notifications.LocalNotification.NotificationMessage(localizationUtility.GetString("FREE_COINS_Notification_Title"), 
                        localizationUtility.GetString("FREE_COINS_Notification_Message"), timeSpan);
#endif
				}
			}
		}

		public void CollectCoins()
		{
			FreeCoinsCount++;
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, GetCoinBack());
			CoinBankReward = SystemTime.Now.Ticks;
		}

		public string GetTimeLeft()
		{
			if (CoinBankReward == 0)
			{
				CoinBankReward = SystemTime.Now.AddMinutes(-19.5).Ticks;
			}
			TimeSpan timeSpan = new DateTime(CoinBankReward).AddMinutes(20.0).Subtract(SystemTime.Now);
			if (timeSpan.TotalSeconds <= 0.0)
			{
				return "00:00";
			}
			return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}
	}
}
