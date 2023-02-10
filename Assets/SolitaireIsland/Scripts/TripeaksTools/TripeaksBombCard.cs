using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksBombCard : TripeaksCard
	{
		private float Index = 30f;

		public TripeaksBombCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "炸弹牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			desk.FlyCard();
			desk.RemoveCard(this);
			desk.LinkCard();
			desk.SpendTime(2f);
			desk.Step += string.Empty;
			desk.Step += $"销毁炸牌 {GetSuit()} {GetNumber()} ;同时带走一张右边手牌\n";
		}

		public override void SpendTime(float deltaTime)
		{
			Index -= deltaTime;
			if (Index <= 0f)
			{
				desk.Over();
			}
		}
	}
}
