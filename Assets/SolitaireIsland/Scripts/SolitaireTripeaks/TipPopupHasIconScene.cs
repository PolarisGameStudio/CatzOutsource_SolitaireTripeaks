using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TipPopupHasIconScene : SoundScene
	{
		private UnityAction<bool> unityAction;

		public Text TitleLabel;

		public Text DescriptionLabel;

		public Text SureButtonLabel;

		public Image IconImage;

		public void OnStart(string title, string description, string sure, Sprite iconSprite, UnityAction<bool> unityAction)
		{
			TitleLabel.text = title;
			DescriptionLabel.text = description;
			SureButtonLabel.text = sure;
			if (iconSprite != null)
			{
				IconImage.sprite = iconSprite;
				IconImage.SetNativeSize();
			}
			this.unityAction = unityAction;
		}

		public void OnButtonClick(bool sure)
		{
			if (unityAction != null)
			{
				unityAction(sure);
			}
		}

		public static void ShowWatchVideoOptimization(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupHasIconScene>("Scenes/Pops/WatchAdTipPopup").OnStart(localizationUtility.GetString("watch_ad_title"), localizationUtility.GetString("watch_ad_desc"), localizationUtility.GetString("btn_ok"), SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("free_icon_ad"), delegate(bool success)
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				if (success)
				{
					SingletonClass<MySceneManager>.Get().Popup<FreeCoinScene>("Scenes/FreeCoinScene", new JoinEffect()).OnStart(unityAction);
				}
				else if (unityAction != null)
				{
					unityAction();
				}
			});
		}
	}
}
