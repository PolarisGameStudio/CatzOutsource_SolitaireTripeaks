using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class RankCoinData
	{
		public List<RankLevelDataGroup> RankLevelDataGroups;

		public int RankCoinNumbers;

		public string NewRankId;

		public SegmentType Staged;

		public string CurrentRankId;

		public List<RankRewardData> Rewardings;

		public List<RankRewardData> NewRewardings;

		public long DoubleBoosterEnd;

		public bool IsTips;

		public bool Cleaned;

		public RankCoinData()
		{
			Rewardings = new List<RankRewardData>();
			NewRewardings = new List<RankRewardData>();
			RankLevelDataGroups = new List<RankLevelDataGroup>();
			RankCoinNumbers = 0;
			DoubleBoosterEnd = DateTime.MinValue.Ticks;
			Staged = SegmentType.Bronze;
		}

		public static RankCoinData Get()
		{
			if (SolitaireTripeaksData.Get().RankCoin == null)
			{
				SolitaireTripeaksData.Get().RankCoin = new RankCoinData();
			}
			return SolitaireTripeaksData.Get().RankCoin;
		}

		public LevelData GetLevelData(int level)
		{
			if (HasTreasure(level))
			{
				RankLevelDataGroup rankLevelDataGroup = RankLevelDataGroups.Find((RankLevelDataGroup e) => e.Level == level);
				if (rankLevelDataGroup != null)
				{
					return rankLevelDataGroup.LevelData;
				}
				return new LevelData();
			}
			return null;
		}

		public LevelData GetLevelData(int level, bool content)
		{
			if (content)
			{
				RankLevelDataGroup rankLevelDataGroup = RankLevelDataGroups.Find((RankLevelDataGroup e) => e.Level == level);
				if (rankLevelDataGroup != null)
				{
					return rankLevelDataGroup.LevelData;
				}
				return new LevelData();
			}
			return null;
		}

		public RecordDataType PutLevelData(int level, LevelData levelData, bool content)
		{
			RecordDataType result = RecordDataType.Normal;
			if (content && levelData.StarComplete)
			{
				int num = (!IsDouble()) ? 1 : 2;
				RankLevelDataGroup rankLevelDataGroup = RankLevelDataGroups.Find((RankLevelDataGroup e) => e.Level == level);
				if (rankLevelDataGroup == null)
				{
					RankLevelDataGroup rankLevelDataGroup2 = new RankLevelDataGroup();
					rankLevelDataGroup2.Level = level;
					rankLevelDataGroup2.LevelData = new LevelData();
					rankLevelDataGroup = rankLevelDataGroup2;
					RankLevelDataGroups.Add(rankLevelDataGroup);
					rankLevelDataGroup.LevelData = levelData;
					result = RecordDataType.FirstRecord;
					RankCoinNumbers += levelData.Star * num;
				}
				else
				{
					if (!rankLevelDataGroup.LevelData.StarComplete)
					{
						rankLevelDataGroup.LevelData.StarComplete = true;
						RankCoinNumbers += num;
						result = RecordDataType.NewRecord;
					}
					if (levelData.StarSteaks && !rankLevelDataGroup.LevelData.StarSteaks)
					{
						rankLevelDataGroup.LevelData.StarSteaks = true;
						RankCoinNumbers += num;
						result = RecordDataType.NewRecord;
					}
					if (levelData.StarTime && !rankLevelDataGroup.LevelData.StarTime)
					{
						rankLevelDataGroup.LevelData.StarTime = true;
						RankCoinNumbers += num;
						result = RecordDataType.NewRecord;
					}
				}
			}
			return result;
		}

		public void SetRewardingRank(string id, int rank, List<PurchasingCommodity> commoditys, SegmentType nextSegment)
		{
			if (NewRewardings == null)
			{
				NewRewardings = new List<RankRewardData>();
			}
			RankRewardData rankRewardData = NewRewardings.Find((RankRewardData r) => r.id.Equals(id));
			if (rankRewardData == null)
			{
				RankRewardData rankRewardData2 = new RankRewardData();
				rankRewardData2.id = id;
				rankRewardData2.reward = false;
				rankRewardData = rankRewardData2;
				NewRewardings.Add(rankRewardData);
			}
			if (rankRewardData != null)
			{
				rankRewardData.syn = true;
				rankRewardData.rank = rank;
				rankRewardData.NextStage = nextSegment;
				rankRewardData.commoditys = commoditys;
			}
		}

		public void SetRank(string rankId)
		{
			if (!string.IsNullOrEmpty(rankId) && SingletonBehaviour<LeaderBoardUtility>.Get().IsOepn && !rankId.Equals(NewRankId))
			{
				if (!string.IsNullOrEmpty(NewRankId))
				{
					ClearRank();
				}
				NewRankId = rankId;
			}
		}

		public void ClearRank()
		{
			IsTips = false;
			RankCoinNumbers = 0;
			RankLevelDataGroups.Clear();
			Cleaned = false;
		}

		public bool HasTreasure(int level)
		{
			return SingletonBehaviour<LeaderBoardUtility>.Get().IsUploadEnable;
		}

		public List<RankRewardData> GetRewards()
		{
			Rewardings.RemoveAll((RankRewardData e) => e.reward);
			IEnumerable<RankRewardData> enumerable = from e in Rewardings
				where e.syn
				select e;
			foreach (RankRewardData item in enumerable)
			{
				item.commoditys = new List<PurchasingCommodity>
				{
					new PurchasingCommodity
					{
						boosterType = BoosterType.Coins,
						count = HightScoreRewardGroup.Get().GetCoins(item.rank)
					}
				};
				item.NextStage = SegmentType.Bronze;
			}
			NewRewardings.RemoveAll((RankRewardData e) => e.NextStage == Staged && (e.commoditys == null || e.commoditys.Count == 0));
			return enumerable.Union(from r in NewRewardings
				where !r.reward && r.syn
				select r).ToList();
		}

		public void SynOldVersion()
		{
			DateTime value = new DateTime(2018, 2, 8);
			int num = 10080;
			int num2 = (int)SystemTime.UtcNow.Subtract(value).TotalMinutes / num * num;
			value = value.AddMinutes(num2);
			if (!string.IsNullOrEmpty(CurrentRankId))
			{
				string value2 = $"TriPeaks_{value.Year}_{value.Month}_{value.Day}_{value.Hour}_{value.Minute}";
				if ((CurrentRankId.Equals(value2) && value.AddMinutes(8640.0).Subtract(SystemTime.UtcNow).TotalSeconds < 0.0) || !CurrentRankId.Equals(value2))
				{
					if (Rewardings.Find((RankRewardData e) => e.id == CurrentRankId) == null)
					{
						Rewardings.Add(new RankRewardData
						{
							id = CurrentRankId
						});
					}
					ClearRank();
				}
				else
				{
					SingletonBehaviour<LeaderBoardUtility>.Get().UploadScore(GetRankCoinNumbers(), AuxiliaryData.Get().AvaterFileName);
				}
				CurrentRankId = string.Empty;
			}
			Rewardings.RemoveAll((RankRewardData e) => e.reward);
			IEnumerable<RankRewardData> enumerable = from e in Rewardings
				where !e.syn
				select e;
			foreach (RankRewardData item in enumerable)
			{
				SingletonBehaviour<LeaderBoardUtility>.Get().SynOldRankVersion("https://tripeaksleaderboardapi20180206040239.azurewebsites.net/api/leaderboard", item);
			}
		}

		public bool IsDouble()
		{
			return new DateTime(DoubleBoosterEnd).Subtract(SystemTime.UtcNow).TotalSeconds > 0.0;
		}

		public void AppendDoubleStarByThreeHours(int number = 1)
		{
			if (number == 0)
			{
				number = 1;
			}
			if (!IsDouble())
			{
				DoubleBoosterEnd = SystemTime.UtcNow.Ticks;
			}
			DoubleBoosterEnd = new DateTime(DoubleBoosterEnd).AddHours(3.0 * (double)number).Ticks;
		}

		public void AppendDoubleStarByMinutes(long minutes = 1L)
		{
			if (minutes == 0)
			{
				minutes = 1L;
			}
			if (!IsDouble())
			{
				DoubleBoosterEnd = SystemTime.UtcNow.Ticks;
			}
			DoubleBoosterEnd = new DateTime(DoubleBoosterEnd).AddMinutes(minutes).Ticks;
		}

		public TimeSpan GetRemainTime()
		{
			return new DateTime(DoubleBoosterEnd).Subtract(SystemTime.UtcNow);
		}

		public string GetRemainTimeString()
		{
			return new DateTime(DoubleBoosterEnd).Subtract(SystemTime.UtcNow).TOString();
		}

		public int GetRankCoinNumbers()
		{
			if (RankLevelDataGroups != null)
			{
				int num = RankLevelDataGroups.Sum((RankLevelDataGroup e) => e.Stars());
				if (RankCoinNumbers < num)
				{
					RankCoinNumbers = num;
				}
			}
			return RankCoinNumbers;
		}
	}
}
