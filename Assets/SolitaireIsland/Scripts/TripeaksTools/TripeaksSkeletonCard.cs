using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksSkeletonCard : TripeaksCard
	{
		private float Index = 5f;

		public TripeaksSkeletonCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "骷髅牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			desk.FlyCard();
			desk.RemoveCard(this);
			desk.LinkCard();
			desk.SpendTime(2f);
			desk.Step += string.Empty;
			desk.Step += $"销毁骷髅牌 {GetSuit()} {GetNumber()} ;同时带走一张右边手牌\n";
		}

		public override void RemoveOnce()
		{
			Index -= 1f;
			if (Index <= 0f)
			{
				desk.Over();
			}
		}
	}
}
