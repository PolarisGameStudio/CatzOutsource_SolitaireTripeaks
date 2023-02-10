namespace TripeaksTools
{
	public class TotemCard
	{
		private readonly int Suit;

		public bool IsCompleted
		{
			get;
			private set;
		}

		public TotemCard(int suit)
		{
			Suit = suit;
		}

		public bool Click(TripeaksDesk desk, int hand)
		{
			if (hand <= 0)
			{
				return false;
			}
			int num = (hand - 1) % 13 + 1;
			if (Suit != num)
			{
				return false;
			}
			if (IsCompleted)
			{
				return false;
			}
			desk.FlyCard();
			IsCompleted = true;
			return true;
		}
	}
}
