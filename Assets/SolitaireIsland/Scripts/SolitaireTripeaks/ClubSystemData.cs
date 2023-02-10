using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ClubSystemData
	{
		public List<string> views;

		public List<string> thanks;

		public List<RequestClubData> requests;

		public string ClubId;

		public int Score;

		public int PlayNumber;

		public List<RankRewardData> RankRewardDatas;

		public static ClubSystemData Get()
		{
			if (SolitaireTripeaksData.Get().ClubSystem == null)
			{
				SolitaireTripeaksData.Get().ClubSystem = new ClubSystemData();
			}
			return SolitaireTripeaksData.Get().ClubSystem;
		}

		public List<RankRewardData> GetLeaderboardDatas()
		{
			if (RankRewardDatas == null)
			{
				RankRewardDatas = new List<RankRewardData>();
			}
			return (from e in RankRewardDatas
				where !e.reward && e.syn && e.commoditys != null && e.commoditys.Count((PurchasingCommodity c) => c.count > 0) > 0
				select e).ToList();
		}

		public void SetLeaderboardData(string id, int rank, SegmentType currentStage, SegmentType nextStage, List<PurchasingCommodity> commodities)
		{
			if (!string.IsNullOrEmpty(id))
			{
				if (RankRewardDatas == null)
				{
					RankRewardDatas = new List<RankRewardData>();
				}
				RankRewardData rankRewardData = RankRewardDatas.Find((RankRewardData e) => e.id == id);
				if (rankRewardData != null && rankRewardData.upload)
				{
					rankRewardData.rank = rank;
					rankRewardData.NextStage = nextStage;
					rankRewardData.CurrentStage = currentStage;
					rankRewardData.syn = true;
					rankRewardData.commoditys = commodities;
				}
			}
		}

		public void SetLeaderboardData(string id, bool upload = false)
		{
			if (!string.IsNullOrEmpty(id))
			{
				if (RankRewardDatas == null)
				{
					RankRewardDatas = new List<RankRewardData>();
				}
				RankRewardData rankRewardData = RankRewardDatas.Find((RankRewardData e) => e.id == id);
				if (rankRewardData == null)
				{
					RankRewardData rankRewardData2 = new RankRewardData();
					rankRewardData2.id = id;
					rankRewardData2.syn = false;
					rankRewardData = rankRewardData2;
					RankRewardDatas.Add(rankRewardData);
				}
				if (upload)
				{
					rankRewardData.upload = true;
				}
			}
		}

		public bool ContainsThanks(string id)
		{
			if (thanks == null)
			{
				return false;
			}
			return thanks.Contains(id);
		}

		public void AppendThanks(string id)
		{
			if (thanks == null)
			{
				thanks = new List<string>();
			}
			if (!thanks.Contains(id))
			{
				thanks.Add(id);
			}
		}

		public void ClearThanks(List<string> ids = null)
		{
			if (thanks != null)
			{
				if (ids == null)
				{
					thanks.Clear();
				}
				else
				{
					thanks.ToList().ForEach(delegate(string e)
					{
						if (!ids.Contains(e))
						{
							thanks.Remove(e);
						}
					});
				}
			}
		}

		public bool ContainsGift(string id)
		{
			if (views == null)
			{
				return false;
			}
			return views.Contains(id);
		}

		public void AppendGift(string id)
		{
			if (views == null)
			{
				views = new List<string>();
			}
			if (!views.Contains(id))
			{
				views.Add(id);
			}
		}

		public void ClearGifts(List<string> ids = null)
		{
		}

		public void AppendRequest(string clubId, string clubName, string clubIcon)
		{
			RequestClubData item = new RequestClubData(clubId, clubName, clubIcon);
			if (requests == null)
			{
				requests = new List<RequestClubData>();
			}
			if (!requests.Contains(item))
			{
				requests.Add(item);
			}
		}

		public void RemoveRequest(string exclude)
		{
			if (requests != null && exclude != null)
			{
				requests.RemoveAll((RequestClubData e) => exclude.Equals(e.clubId));
			}
		}

		public void RemoveRequest(RequestClubData request)
		{
			if (requests != null)
			{
				requests.Remove(request);
			}
		}

		public List<RequestClubData> GetRequestClub()
		{
			if (requests == null)
			{
				return new List<RequestClubData>();
			}
			return requests;
		}

		public void AppendScore(string clubId, int score)
		{
			if (clubId == ClubId)
			{
				Score += score;
				PlayNumber++;
			}
			else
			{
				PlayNumber = 1;
				Score = score;
				ClubId = clubId;
			}
		}

		public void ClearScore()
		{
			PlayNumber = 0;
			Score = 0;
		}
	}
}
