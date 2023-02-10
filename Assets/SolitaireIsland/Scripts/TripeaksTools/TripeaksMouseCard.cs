using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksMouseCard : TripeaksCard
	{
		private float Index = 2f;

		public TripeaksMouseCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "老鼠牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			desk.SpendTime(1f);
			desk.FlyCard();
			desk.Step += string.Format("老鼠临死前带走一张右边手牌！！！\n", GetSuit(), GetNumber());
			base.Destory(number);
		}

		public override void SpendTime(float deltaTime)
		{
			Index -= deltaTime;
			if (Index <= 0f)
			{
				Index = 14f;
				desk.RemoveLeftHandCard();
				desk.Step += $"老鼠 {GetSuit()} {GetNumber()} 开始偷牌！！！\n";
			}
		}
	}
}
