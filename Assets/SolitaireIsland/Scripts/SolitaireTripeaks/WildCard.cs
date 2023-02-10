namespace SolitaireTripeaks
{
	public class WildCard : BaseCard
	{
		protected override void StartInitialized()
		{
			Config.Index = -100;
		}

		protected override string GetFont()
		{
			return "Prefabs/Pokers/WildPoker";
		}

		public override void CollectedToRightHand()
		{
			base.CollectedToRightHand();
			OperatingHelper.Get().ClearStep();
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
