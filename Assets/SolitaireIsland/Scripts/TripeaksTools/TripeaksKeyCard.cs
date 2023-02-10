using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksKeyCard : TripeaksCard
	{
		public TripeaksKeyCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "钥匙牌：" + GetSuit() + GetNumber() + "\n";
			}
		}

		public override void Destory(int number)
		{
			base.Destory(number);
			if (desk != null)
			{
				TripeaksCard[] allCard = desk.GetAllCard<TripeaksLockCard>();
				TripeaksCard[] array = allCard;
				for (int i = 0; i < array.Length; i++)
				{
					TripeaksLockCard tripeaksLockCard = (TripeaksLockCard)array[i];
					tripeaksLockCard.UnLock();
				}
			}
		}
	}
}
