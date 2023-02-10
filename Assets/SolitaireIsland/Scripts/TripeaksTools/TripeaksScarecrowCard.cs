using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksScarecrowCard : TripeaksCard
	{
		public TripeaksScarecrowCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "通关牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			base.Destory(number);
			desk.Step += $"销毁通关牌消除 {GetSuit()} {GetNumber()} ！！！\n";
		}
	}
}
