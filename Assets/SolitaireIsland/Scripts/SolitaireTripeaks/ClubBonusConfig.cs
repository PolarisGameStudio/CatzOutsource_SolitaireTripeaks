using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[CreateAssetMenu(fileName = "ClubBonusConfig.asset", menuName = "Nightingale/Club Bonus Config", order = 1)]
	public class ClubBonusConfig : ScriptableObject
	{
		private class ProbabilityData
		{
			public int Probability;

			public int Number;
		}

		public List<ClubBonusLevelConfig> configs;

		private static ClubBonusConfig finder;

		public static ClubBonusConfig Get()
		{
			if (finder == null)
			{
				finder = SingletonBehaviour<LoaderUtility>.Get().GetAsset<ClubBonusConfig>("Configs/ClubBonusConfig");
			}
			return finder;
		}

		public List<PurchasingCommodity> GetClubBonus(int level)
		{
			if (level > configs.Count)
			{
				level = configs.Count;
			}
			if (level <= 0)
			{
				level = 1;
			}
			return (from e in configs.GetRange(0, level).SelectMany((ClubBonusLevelConfig e) => e.commoditys)
				group e by e.boosterType into g
				select new PurchasingCommodity
				{
					boosterType = g.Key,
					count = g.Sum((PurchasingCommodity x) => x.count)
				}).ToList();
		}

		public int GetDailyLimit(int level)
		{
			if (level > configs.Count)
			{
				level = configs.Count;
			}
			if (level <= 0)
			{
				level = 1;
			}
			return (from e in configs.GetRange(0, level)
				where e.Skill == ClubSkill.Daily
				select e).Sum((ClubBonusLevelConfig e) => e.SkillNumber);
		}

		public float GetSkillRatioCoins(ClubSkill skill, int level)
		{
			if (level > configs.Count)
			{
				level = configs.Count;
			}
			if (level <= 0)
			{
				level = 1;
			}
			return (from e in configs.GetRange(0, level)
				where e.Skill == skill
				select e).Sum((ClubBonusLevelConfig e) => e.SkillRatio);
		}

		public bool HasSuperTreasure(int level)
		{
			if (level > configs.Count)
			{
				level = configs.Count;
			}
			if (level <= 0)
			{
				level = 1;
			}
			return configs.GetRange(0, level).Find((ClubBonusLevelConfig e) => e.Skill == ClubSkill.SuperTreasure) != null;
		}

		public List<PurchasingCommodity> GetSuperTreasure()
		{
			List<PurchasingCommodity> list = new List<PurchasingCommodity>();
			list.Add(new PurchasingCommodity
			{
				boosterType = BoosterType.ExpiredPlay,
				count = Probability(new List<ProbabilityData>
				{
					new ProbabilityData
					{
						Probability = 4,
						Number = 4
					},
					new ProbabilityData
					{
						Probability = 3,
						Number = 5
					},
					new ProbabilityData
					{
						Probability = 2,
						Number = 6
					},
					new ProbabilityData
					{
						Probability = 1,
						Number = 7
					}
				}).Number
			});
			list.Add(new PurchasingCommodity
			{
				boosterType = AppearNodeConfig.Get().GetRandomBooster(),
				count = Probability(new List<ProbabilityData>
				{
					new ProbabilityData
					{
						Probability = 55,
						Number = 4
					},
					new ProbabilityData
					{
						Probability = 30,
						Number = 6
					},
					new ProbabilityData
					{
						Probability = 10,
						Number = 8
					},
					new ProbabilityData
					{
						Probability = 5,
						Number = 10
					}
				}).Number
			});
			List<int> list2 = new List<int>();
			list2.Add(10);
			list2.Add(40);
			list2.Add(3);
			list2.Add(10);
			switch (MathUtility.Probability(list2))
			{
			case 1:
				list.Add(new PurchasingCommodity
				{
					boosterType = BoosterType.UnlimitedDoubleStar,
					count = Probability(new List<ProbabilityData>
					{
						new ProbabilityData
						{
							Probability = 4,
							Number = 180
						},
						new ProbabilityData
						{
							Probability = 3,
							Number = 360
						},
						new ProbabilityData
						{
							Probability = 2,
							Number = 720
						},
						new ProbabilityData
						{
							Probability = 1,
							Number = 1440
						}
					}).Number
				});
				break;
			case 2:
				list.Add(new PurchasingCommodity
				{
					boosterType = BoosterType.Wild,
					count = Probability(new List<ProbabilityData>
					{
						new ProbabilityData
						{
							Probability = 4,
							Number = 2
						},
						new ProbabilityData
						{
							Probability = 3,
							Number = 3
						},
						new ProbabilityData
						{
							Probability = 2,
							Number = 4
						},
						new ProbabilityData
						{
							Probability = 1,
							Number = 5
						}
					}).Number
				});
				break;
			case 3:
				list.Add(new PurchasingCommodity
				{
					boosterType = BoosterType.Rocket,
					count = Probability(new List<ProbabilityData>
					{
						new ProbabilityData
						{
							Probability = 4,
							Number = 1
						},
						new ProbabilityData
						{
							Probability = 3,
							Number = 2
						},
						new ProbabilityData
						{
							Probability = 2,
							Number = 3
						},
						new ProbabilityData
						{
							Probability = 1,
							Number = 4
						}
					}).Number
				});
				break;
			}
			return list;
		}

		private ProbabilityData Probability(List<ProbabilityData> probabilitys)
		{
			int num = UnityEngine.Random.Range(0, probabilitys.Sum((ProbabilityData e) => e.Probability) + 1);
			for (int i = 0; i < probabilitys.Count; i++)
			{
				num -= probabilitys[i].Probability;
				if (num <= 0)
				{
					return probabilitys[i];
				}
			}
			return probabilitys[0];
		}
	}
}
