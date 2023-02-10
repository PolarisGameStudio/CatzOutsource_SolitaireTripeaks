using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksRopeCard : TripeaksCard
	{
		private bool rope = true;

		public TripeaksRopeCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "绳子牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			if (rope)
			{
				rope = false;
				desk.LinkCard();
				desk.FlyCard();
				desk.Step += $"销毁网 {GetSuit()} {GetNumber()} ！！！仅销毁了网;同时带走一张右边手牌！！！\n";
				desk.SpendTime(2f);
			}
			else
			{
				base.Destory(number);
			}
		}
	}
}
