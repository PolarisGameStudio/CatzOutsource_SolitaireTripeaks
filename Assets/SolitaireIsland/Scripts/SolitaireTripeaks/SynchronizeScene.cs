using Nightingale.ScenesManager;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class SynchronizeScene : BaseScene
	{
		public SynchronizeUI LocalSynchronizeUI;

		public SynchronizeUI RemoteSynchronizeUI;

		public void OnStart(SolitaireTripeaksData data, UnityAction unityAction)
		{
			base.IsFixed = true;
			LocalSynchronizeUI.SetInfo(SolitaireTripeaksData.Get(), remote: false, unityAction);
			RemoteSynchronizeUI.SetInfo(data, remote: true, unityAction);
		}
	}
}
