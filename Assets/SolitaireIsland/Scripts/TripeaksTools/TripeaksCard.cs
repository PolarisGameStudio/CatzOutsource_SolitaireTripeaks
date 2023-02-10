using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksCard
	{
		public BaseCard baseCard;

		public TripeaksDesk desk;

		public TripeaksCard()
		{
		}

		public TripeaksCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			this.desk = desk;
			if (desk != null)
			{
				string cardInfo = desk.cardInfo;
				desk.cardInfo = cardInfo + "普通牌：" + desk.GetSuit(baseCard.GetSuit()) + (baseCard.GetNumber() + 1) + "\n";
			}
		}

		public virtual void LinkOnce()
		{
		}

		public virtual void RemoveOnce()
		{
		}

		public virtual void DONext(bool top)
		{
		}

		public virtual bool IsFree()
		{
			return true;
		}

		public virtual void SpendTime(float deltaTime)
		{
		}

		public virtual void Destory(int number)
		{
			desk.LinkCard();
			desk.SpendTime(1f);
			desk.AppendToRightHand(this);
			TripeaksDesk tripeaksDesk = desk;
			string step = tripeaksDesk.Step;
			tripeaksDesk.Step = step + "销毁 " + GetSuit() + "_" + GetNumber() + "\n";
		}

		public string GetNumber()
		{
			string result = (baseCard.GetNumber() + 1).ToString();
			switch (baseCard.GetNumber())
			{
			case 10:
				result = "J";
				break;
			case 11:
				result = "Q";
				break;
			case 12:
				result = "K";
				break;
			case 0:
				result = "A";
				break;
			}
			return result;
		}

		public string GetSuit()
		{
			return desk.GetSuit(baseCard.GetSuit());
		}
	}
}
