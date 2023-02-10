using Nightingale.ScenesManager;
using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class SoundScene : BaseScene
	{
		public override void OnSceneStateChanged(SceneState state)
		{
			switch (state)
			{
			case SceneState.Opening:
				AudioUtility.GetSound().Play("Audios/pop_Scene.mp3");
				break;
			case SceneState.Closing:
				AudioUtility.GetSound().Play("Audios/close_scene.mp3");
				break;
			}
		}
	}
}
