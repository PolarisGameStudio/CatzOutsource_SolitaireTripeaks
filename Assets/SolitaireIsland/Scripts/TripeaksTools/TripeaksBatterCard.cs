using SolitaireTripeaks;
using UnityEngine;

namespace TripeaksTools
{
	public class TripeaksBatterCard : TripeaksCard
	{
		private int Index = 5;

		private int MaxIndex = 5;

		public TripeaksBatterCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "龟壳牌：" + desk.GetSuit(baseCard.GetSuit()) + (baseCard.GetNumber() + 1) + "\n";
			}
		}

		public override void DONext(bool top)
		{
			Index++;
			Index = Mathf.Min(MaxIndex, Index);
		}

		public override void LinkOnce()
		{
			Index--;
			Index = Mathf.Max(0, Index);
			if (Index <= 3)
			{
				MaxIndex = 3;
			}
		}

		public override bool IsFree()
		{
			return Index <= 0;
		}

		public override void Destory(int number)
		{
			desk.Step += "龟壳 ";
			base.Destory(number);
		}
	}
}
