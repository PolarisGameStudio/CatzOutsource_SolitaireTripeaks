using Nightingale.ScenesManager;
using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class PayTableScene : SoundScene
	{
		public void Btn_Close()
		{
			SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
		}
	}
}
