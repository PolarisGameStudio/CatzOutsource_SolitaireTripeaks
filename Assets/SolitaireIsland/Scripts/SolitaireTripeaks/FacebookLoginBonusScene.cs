using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class FacebookLoginBonusScene : SoundScene
	{
		public Button FacebookButton;

		private UnityAction unityAction;

		public void OnStart(UnityAction unityAction)
		{
			this.unityAction = unityAction;
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(UpdateLogin);
			FacebookButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<FacebookMananger>.Get().Login();
			});
		}

		public void Btn_Close()
		{
			SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
			if (unityAction != null)
			{
				unityAction();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.RemoveListener(UpdateLogin);
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(FacebookLoginBonusScene).Name);
		}

		private void UpdateLogin(bool login)
		{
			if (login)
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				if (unityAction != null)
				{
					unityAction();
				}
			}
		}
	}
}
