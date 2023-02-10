namespace SolitaireTripeaks
{
	public class GrowBatterExtra : DebuffExtra
	{
		private bool disappearing;

		private int disappearCount;

		protected override void StartInitialized()
		{
			base.StartInitialized();
			base.Index = 5;
		}
	}
}
