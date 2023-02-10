using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class MultipleStreaksBooster : GlobalBooster
	{
		public override void OnStart(UnityAction unityAction)
		{
			ShowEffectEveryOnce(unityAction);
		}

		public override int MultipleStreaks()
		{
			return 2;
		}
	}
}
