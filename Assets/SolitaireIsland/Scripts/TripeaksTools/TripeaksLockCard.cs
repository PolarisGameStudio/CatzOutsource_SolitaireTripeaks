using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksLockCard : TripeaksCard
	{
		private bool isFree;

		public TripeaksLockCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "锁牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public void UnLock()
		{
			isFree = true;
		}

		public override bool IsFree()
		{
			return isFree;
		}
	}
}
