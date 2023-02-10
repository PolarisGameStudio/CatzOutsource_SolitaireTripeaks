using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class CommonPopScene : SoundScene
	{
		private UnityAction<bool> unityAction;

		public void OnStart(UnityAction<bool> unityAction)
		{
			this.unityAction = unityAction;
		}

		public void OnButtonClick(bool sure)
		{
			if (unityAction != null)
			{
				unityAction(sure);
			}
		}
	}
}
