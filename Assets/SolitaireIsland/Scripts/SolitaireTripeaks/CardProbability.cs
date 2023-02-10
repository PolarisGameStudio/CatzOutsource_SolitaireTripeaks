using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class CardProbability
	{
		public string cardType;

		public int Index;

		public float probability;

		public CardType GetCardType()
		{
			return EnumUtility.GetEnumType(cardType, CardType.Number);
		}
	}
}
