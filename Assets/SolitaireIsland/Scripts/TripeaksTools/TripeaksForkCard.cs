using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksForkCard : TripeaksCard
	{
		public TripeaksForkCard(BaseCard card, TripeaksDesk desk = null)
		{
			base.desk = desk;
			baseCard = card;
			if (desk != null)
			{
				desk.cardInfo += "叉子牌。\n";
			}
		}

		public override bool IsFree()
		{
			return false;
		}

		public override void Destory(int number)
		{
			desk.RemoveCard(this);
			desk.LinkCard();
			desk.SpendTime(2f);
			desk.Step += "销毁叉子牌\n";
		}
	}
}
