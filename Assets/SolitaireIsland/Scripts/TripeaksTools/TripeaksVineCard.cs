using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksVineCard : TripeaksCard
	{
		private bool rope = true;

		private float Index = 2f;

		public TripeaksVineCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "藤蔓牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void SpendTime(float deltaTime)
		{
			if (!rope)
			{
				Index -= deltaTime;
				if (Index <= 0f)
				{
					Index = 14f;
					rope = true;
					desk.Step += $"藤蔓 {GetSuit()} {GetNumber()} 生长出来。。。\n";
				}
			}
		}

		public override void Destory(int number)
		{
			if (rope)
			{
				desk.SpendTime(1f);
				desk.LinkCard();
				desk.FlyCard();
				desk.Step += $"销毁藤蔓 {GetSuit()} {GetNumber()} ！！！仅销毁了藤蔓;同时带走一张右边手牌！！！\n";
				rope = false;
			}
			else
			{
				base.Destory(number);
			}
		}
	}
}
