using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class TimeExtra : DebuffExtra
	{
		protected bool playing;

		protected float TimeStep
		{
			get;
			private set;
		}

		protected override void StartInitialized()
		{
			playing = true;
			RestTime();
		}

		protected virtual bool IsRunning()
		{
			return playing;
		}

		private void Update()
		{
			if (IsRunning() && !(TimeStep < 0f) && PlayDesk.Get().Uppers.Contains(baseCard))
			{
				int num = (int)TimeStep;
				TimeStep -= NightingaleTime.DeltaTime * SingletonBehaviour<GlobalConfig>.Get().TimeScale;
				if (num != (int)TimeStep)
				{
					LifeStep();
				}
				LifeUpdate();
				if (TimeStep <= 0f)
				{
					LifeOver();
				}
			}
		}

		protected void RestTime()
		{
			TimeStep = base.Index;
		}

		protected void RestTime(int index)
		{
			TimeStep = index;
		}

		protected virtual void LifeOver()
		{
		}

		protected virtual void LifeUpdate()
		{
		}

		protected virtual void LifeStep()
		{
		}
	}
}
