using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class FullFlipBooster : GlobalBooster
	{
		public override void OnStart(UnityAction unityAction)
		{
			ShowEffectEveryOnce(unityAction);
			foreach (BaseCard poker in PlayDesk.Get().Pokers)
			{
				poker.TryOpenCard();
			}
			OperatingHelper.Get().ClearStep();
		}
	}
}
