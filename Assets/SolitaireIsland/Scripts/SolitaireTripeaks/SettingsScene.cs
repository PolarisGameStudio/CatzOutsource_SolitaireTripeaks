using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SettingsScene : SoundScene
	{
		public Slider musicSlider;

		public Slider soundSlider;

		public Button backButton;

		public Button loginButton;

		public Button contactButton;

		public Button helpButton;

		public Button achievementButton;

		public Button RewardButton;

		public GameObject PrivacyButton;

		public InputField NickNameInputField;

		public Text GameIdLabel;

		public Text VersionLabel;

		public LocalizationLabel StarLabel;

		public Transform _AvtarTransform;

		private void Start()
		{
			base.IsStay = true;
			UpdateLogin(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(UpdateLogin);
			musicSlider.value = SingletonData<SettingData>.Get().musicVolume;
			soundSlider.value = SingletonData<SettingData>.Get().soundVolume;
			StarLabel.SetText(PlayData.Get().GetStars());
			musicSlider.onValueChanged.AddListener(MusicSlider);
			soundSlider.onValueChanged.AddListener(SoundSlider);
			GameIdLabel.text = string.Format("ID: " + SolitaireTripeaksData.Get().GetPlayerId());
			VersionLabel.text = string.Format("v" + Application.version);
			backButton.onClick.AddListener(delegate
			{
				if (PlayDesk.Get() != null)
				{
					SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
					TipPopupNoIconScene.ShowQuitPlayScene();
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom), delegate
					{
						PlatformUtility.OnApplicationQuit();
					});
				}
			});
			loginButton.onClick.AddListener(Btn_Login_Click);
			contactButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ReportScene>("Scenes/ReportScene.prefab");
			});
			helpButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				SingletonClass<MySceneManager>.Get().Popup<HelpScene>("Scenes/HelpScene");
			});
			achievementButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				SingletonClass<MySceneManager>.Get().Popup<AchievementScene>("Scenes/AchievementScene");
			});
			RewardButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<TripeaksBonusScene>("Scenes/TripeaksBonusScene");
				UnityEngine.Debug.Log(SolitaireTripeaksData.Get().GetPlayerId());
			});
			SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().AddNickNameListener(GetInstanceID(), delegate
			{
				NickNameInputField.text = AuxiliaryData.Get().GetNickName();
			});
			NickNameInputField.onEndEdit.AddListener(ChangeNickName);
		}

		protected override void OnDestroy()
		{
			SingletonData<SettingData>.Get().FlushData();
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.RemoveListener(UpdateLogin);
			SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().RemoveNickNameListener(GetInstanceID());
			NickNameInputField.onEndEdit.RemoveListener(ChangeNickName);
			base.OnDestroy();
		}

		private void ChangeNickName(string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
			{
				SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().UpdateNickName();
			}
			else if ((!string.IsNullOrEmpty(AuxiliaryData.Get().NickName) || !playerName.Equals("Guest")) && !playerName.Equals(AuxiliaryData.Get().NickName))
			{
				TipPopupNoIconScene.ShowChangeNickNamePopup(delegate(bool sure)
				{
					if (sure)
					{
						if (SessionData.Get().UseCommodity(BoosterType.Coins, 1000L, "NickName"))
						{
							SingletonClass<MySceneManager>.Get().Close();
							SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().SetNickName(playerName);
							SingletonBehaviour<ClubSystemHelper>.Get().Profile(playerName, AuxiliaryData.Get().AvaterFileName);
						}
						else
						{
							StoreScene.ShowOutofCoins();
						}
					}
					else
					{
						SingletonClass<MySceneManager>.Get().Close();
						SingletonBehaviour<TripeaksPlayerHelper>.Get().GetSelf().UpdateNickName();
					}
				});
			}
		}

		private void MusicSlider(float volume)
		{
			SingletonData<SettingData>.Get().musicVolume = volume;
			AudioUtility.GetMusic().SetVolume(volume);
		}

		private void SoundSlider(float volume)
		{
			SingletonData<SettingData>.Get().soundVolume = volume;
			AudioUtility.GetSound().SetVolume(volume);
		}

		private void Btn_Login_Click()
		{
			if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
			{
				SingletonBehaviour<FacebookMananger>.Get().Logout();
			}
			else
			{
				SingletonBehaviour<GlobalConfig>.Get().ShowLoginFacebook(AuxiliaryData.Get().IsFacebookReward);
			}
		}

		private void UpdateLogin(bool login)
		{
			loginButton.GetComponentInChildren<Text>().text = ((!login) ? LocalizationUtility.Get("Localization_setting.json").GetString("btn_login") : LocalizationUtility.Get("Localization_setting.json").GetString("btn_logout"));
		}
	}
}
