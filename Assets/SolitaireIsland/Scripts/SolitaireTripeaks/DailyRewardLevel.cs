using System;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyRewardLevel
	{
		public DailyReward[] dailyRewards;

		public DailyReward RandomDailyReward()
		{
			int max = dailyRewards.Sum((DailyReward e) => e.weight);
			int num = UnityEngine.Random.Range(0, max);
			for (int i = 0; i < dailyRewards.Length; i++)
			{
				num -= dailyRewards[i].weight;
				if (num <= 0)
				{
					return dailyRewards[i];
				}
			}
			return dailyRewards[0];
		}
	}
}
