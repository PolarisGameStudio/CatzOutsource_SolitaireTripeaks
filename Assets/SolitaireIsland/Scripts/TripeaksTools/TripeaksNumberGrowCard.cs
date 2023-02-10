using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksNumberGrowCard : TripeaksCard
	{
		public TripeaksNumberGrowCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "自增牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void DONext(bool top)
		{
			if (!top)
			{
				return;
			}
			NumberGrowExtra extra = baseCard.GetExtra<NumberGrowExtra>();
			if (!(extra == null))
			{
				desk.Step += $"自增减 {GetSuit()} {GetNumber()}";
				int suit = baseCard.GetSuit();
				int number = baseCard.GetNumber();
				if (extra.Index % 2 == 0)
				{
					number = (number + 13 + 1) % 13 + 1;
					baseCard.SetIndex(number + suit * 13);
				}
				else
				{
					number = (number + 13 - 1) % 13 + 1;
					baseCard.SetIndex(number + suit * 13);
				}
				desk.Step += $"变成 {GetSuit()} {GetNumber()}\n";
			}
		}
	}
}
