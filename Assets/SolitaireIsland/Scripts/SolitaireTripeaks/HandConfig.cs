using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class HandConfig
	{
		public List<CardProbability> CardProbabilitys;

		public List<CardProbability> DeskCardProbabilitys;

		private static HandConfig NormalConfig;

		private static HandConfig BellaConfig;

		public static HandConfig GetNormal()
		{
			if (NormalConfig == null)
			{
				HandConfig handConfig = new HandConfig();
				handConfig.CardProbabilitys = new List<CardProbability>
				{
					new CardProbability
					{
						cardType = "Fox",
						probability = 0.01f
					},
					new CardProbability
					{
						cardType = "Golden",
						probability = 0.01f
					},
					new CardProbability
					{
						cardType = "Color",
						Index = 0,
						probability = 0.01f
					},
					new CardProbability
					{
						cardType = "Color",
						Index = 1,
						probability = 0.01f
					},
					new CardProbability
					{
						cardType = "Rocket",
						probability = 0.01f
					}
				};
				handConfig.DeskCardProbabilitys = new List<CardProbability>
				{
					new CardProbability
					{
						cardType = "Coin",
						probability = 0.1f
					}
				};
				NormalConfig = handConfig;
			}
			return NormalConfig;
		}

		public static HandConfig GetBella()
		{
			if (BellaConfig == null)
			{
				HandConfig handConfig = new HandConfig();
				handConfig.CardProbabilitys = new List<CardProbability>
				{
					new CardProbability
					{
						cardType = "Fox",
						probability = 10f
					},
					new CardProbability
					{
						cardType = "Golden",
						probability = 10f
					},
					new CardProbability
					{
						cardType = "Color",
						Index = 0,
						probability = 10f
					},
					new CardProbability
					{
						cardType = "Color",
						Index = 1,
						probability = 10f
					},
					new CardProbability
					{
						cardType = "Rocket",
						probability = 7f
					}
				};
				BellaConfig = handConfig;
			}
			return BellaConfig;
		}

		public CardProbability Random()
		{
			if (SingletonClass<OnceGameData>.Get().IsTutorial())
			{
				CardProbability cardProbability = new CardProbability();
				cardProbability.cardType = "Golden";
				cardProbability.probability = 20f;
				return cardProbability;
			}
			float num = UnityEngine.Random.Range(0f, CardProbabilitys.Sum((CardProbability e) => e.probability));
			foreach (CardProbability cardProbability2 in CardProbabilitys)
			{
				num -= cardProbability2.probability;
				if (num <= 0f)
				{
					return cardProbability2;
				}
			}
			return CardProbabilitys[0];
		}
	}
}
