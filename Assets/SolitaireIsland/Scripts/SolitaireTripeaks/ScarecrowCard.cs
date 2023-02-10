namespace SolitaireTripeaks
{
	public class ScarecrowCard : NumberCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/ScarecrowPoker";
		}

		protected override string GetBackground()
		{
			return "Prefabs/Pokers/ScarecrowBackgroundPoker";
		}

		public override void CollectedToRightHand()
		{
			base.CollectedToRightHand();
			if (PlayDesk.Get().Pokers.Find((BaseCard e) => e is ScarecrowCard) == null)
			{
				PlayDesk.Get().LevelCompleted();
			}
		}
	}
}
