using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TipPopupDoubleButtonScene : SoundScene
	{
		private UnityAction<bool> unityAction;

		public Text TitleLabel;

		public Text DescriptionLabel;

		public Text SureButtonLabel;

		public Text ClosedButtonLabel;

		public void OnStart(string title, string description, string sure, string close, UnityAction<bool> unityAction)
		{
			TitleLabel.text = title;
			DescriptionLabel.text = description;
			SureButtonLabel.text = sure;
			ClosedButtonLabel.text = close;
			this.unityAction = unityAction;
		}

		public void OnButtonClick(bool sure)
		{
			if (unityAction != null)
			{
				unityAction(sure);
			}
		}

		public static void ShowLogout()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility.GetString("Logout_Facebook_Title"), localizationUtility.GetString("Logout_Facebook_description"), localizationUtility.GetString("Btn_LOGOUT"), localizationUtility.GetString("Btn_Cancel"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (sure)
				{
					SingletonBehaviour<FacebookMananger>.Get().Logout();
				}
			});
		}
	}
}
