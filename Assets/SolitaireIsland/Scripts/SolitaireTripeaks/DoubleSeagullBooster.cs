using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class DoubleSeagullBooster : GlobalBooster
	{
		public override void OnStart(UnityAction unityAction)
		{
			ShowEffectEveryOnce(unityAction);
		}
	}
}
