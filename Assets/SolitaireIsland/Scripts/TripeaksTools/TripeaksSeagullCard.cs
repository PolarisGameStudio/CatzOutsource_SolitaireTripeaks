using SolitaireTripeaks;

namespace TripeaksTools
{
	public class TripeaksSeagullCard : TripeaksCard
	{
		public TripeaksSeagullCard(BaseCard card, TripeaksDesk desk = null)
		{
			baseCard = card;
			base.desk = desk;
			if (desk != null)
			{
				desk.cardInfo += "海鸥：\n";
			}
		}

		public override void Destory(int number)
		{
			desk.SpendTime(1f);
			desk.RemoveCard(this);
			string[] array = baseCard.Config.ExtraContent.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('-');
				if (array3.Length == 2)
				{
					if (array3[0] == "1")
					{
						desk.AddLinkCoins(int.Parse(array3[1]));
					}
					else
					{
						desk.AddHand(int.Parse(array3[1]));
					}
				}
			}
		}
	}
}
