namespace SolitaireTripeaks
{
	public class SellRocketCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/RocketPoker";
		}

		public override void CollectedToRightHand()
		{
			PlayScene.Get().AppendProp<RocketBooster>();
			base.CollectedToRightHand();
		}

		public override bool StayInTop()
		{
			return false;
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
