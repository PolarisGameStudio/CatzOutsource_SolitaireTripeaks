using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksSameColorCard : TripeaksCard
	{
		public TripeaksSameColorCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "同色牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			if (baseCard.GetColor() != (number - 1) / 13 % 2)
			{
				desk.LinkCard();
				desk.FlyCard();
				desk.Step += $"销毁同色消除 {GetSuit()} {GetNumber()} ！！！;同时带走一张右边手牌！！！\n";
				desk.SpendTime(2f);
			}
			else
			{
				base.Destory(number);
				desk.Step += $"销毁同色消除 {GetSuit()} {GetNumber()} ！！！\n";
			}
		}
	}
}
