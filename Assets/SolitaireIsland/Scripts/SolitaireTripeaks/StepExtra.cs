namespace SolitaireTripeaks
{
	public class StepExtra : DebuffExtra
	{
		private int StepIndex;

		protected override void StartInitialized()
		{
			base.StartInitialized();
			StepIndex = base.Index;
		}

		public override void OnHandChange()
		{
			if (PlayDesk.Get().Uppers.Contains(baseCard))
			{
				StepIndex--;
				LifeUpdate(StepIndex);
				if (StepIndex <= 0)
				{
					LifeOver();
				}
			}
		}

		public override void OnUndo(bool match)
		{
			if (PlayDesk.Get().Uppers.Contains(baseCard))
			{
				StepIndex++;
				LifeUpdate(StepIndex);
			}
		}

		protected void ResetStep()
		{
			StepIndex = base.Index;
		}

		protected virtual void LifeOver()
		{
		}

		protected virtual void LifeUpdate(int stepIndex)
		{
		}
	}
}
