using Nightingale.Inputs;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class TipPopupNoIconScene : SoundScene
	{
		private UnityAction<bool> unityAction;

		public Text TitleLabel;

		public Text DescriptionLabel;

		public Text SureButtonLabel;

		public GameObject CloseButton;

		public TipPopupNoIconScene OnStart(string title, string description, string sure, UnityAction<bool> unityAction = null, bool close = false)
		{
			TitleLabel.text = title;
			DescriptionLabel.text = description;
			SureButtonLabel.text = sure;
			this.unityAction = unityAction;
			CloseButton.SetActive(close);
			return this;
		}

		public TipPopupNoIconScene AddBackInSureButton()
		{
			SureButtonLabel.transform.parent.gameObject.AddComponent<EscapeButtonControler>();
			return this;
		}

		public void OnButtonClick(bool sure)
		{
			if (unityAction != null)
			{
				unityAction(sure);
			}
		}

		public static void ShowCheatUserTips(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			TipPopupNoIconScene tipPopupNoIconScene = SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon");
			tipPopupNoIconScene.IsFixed = true;
			tipPopupNoIconScene.IsStay = true;
			tipPopupNoIconScene.OnStart(localizationUtility.GetString("Cheat_title"), localizationUtility.GetString("Cheat_desc"), localizationUtility.GetString("btn_Cheat_ok"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(), unityAction);
				if (sure)
				{
					string subject = $"Appeal_SolitaireTripeaks_{Application.version}_{SystemInfo.deviceModel}_{SystemInfo.operatingSystem}_{PlatformUtility.GetCountry()}";
					string body = $"deviceID:{SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier};\nplayerID:{SolitaireTripeaksData.Get().GetPlayerId()};\n------------------------------------------\nPlease do not delete the above string\n------------------------------------------";
					PlatformUtility.SendEmail(subject, body);
				}
			}, close: true);
		}

		public static void ShowSynchronizeConfirm(UnityAction<bool> unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_facebook.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Synchronize_min_title"), localizationUtility.GetString("Synchronize_min_desc"), localizationUtility.GetString("btn_ok"), delegate(bool sure)
			{
				if (unityAction != null)
				{
					unityAction(sure);
				}
				SingletonClass<MySceneManager>.Get().Close();
			}, close: true);
		}

		public static void ShowOutOfSpecialActivityNumbers(string name)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_SpecialActivity.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").AddBackInSureButton()
				.OnStart(localizationUtility.GetString("out_of_Special_Activity_Numbers_title"), string.Format(localizationUtility.GetString("out_of_Special_Activity_Numbers_desc"), name), localizationUtility.GetString("btn_ok"), delegate
				{
					SingletonClass<MySceneManager>.Get().Close();
				});
		}

		public static void ShowChangeNickNamePopup(UnityAction<bool> unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("change_nick_name_popup_title"), localizationUtility.GetString("change_nick_name_popup_desc"), localizationUtility.GetString("btn_ok"), delegate(bool sure)
			{
				if (unityAction != null)
				{
					unityAction(sure);
				}
			}, close: true);
		}

		public static void ShowRequestPermission(UnityAction<bool> unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("RequestPermissionTitle"), localizationUtility.GetString("RequestPermissionMessage"), localizationUtility.GetString("btn_Open"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close(null, delegate
				{
					if (unityAction != null)
					{
						unityAction(sure);
					}
				});
			});
		}

		public static void ShowOpenSettings(UnityAction<bool> unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("RequestPermissionOpenSettingsTitle"), localizationUtility.GetString("RequestPermissionOpenSettingsMessage"), localizationUtility.GetString("btn_go"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close(null, delegate
				{
					if (unityAction != null)
					{
						unityAction(sure);
					}
				});
			}, close: true);
		}

		public static void ShowNotReachable(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Not_Reachable_Title"), localizationUtility.GetString("Not_Reachable_description"), localizationUtility.GetString("RETRY"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowDeviceTimeError(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("DeviceTimeError_Title"), localizationUtility.GetString("DeviceTimeError_Desc"), localizationUtility.GetString("RETRY"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowLoadingFeatures(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("loading_time_too_long_Title"), localizationUtility.GetString("loading_time_too_long_Desc"), localizationUtility.GetString("Logout_Facebook_restart_btn"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowInviteFriends()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Invite_Friends_Title"), localizationUtility.GetString("Invite_Friends_description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowAskForHelp()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Ask_for_help_Title"), localizationUtility.GetString("Ask_for_help_description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowFullFriends()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("FullFriends_Title"), localizationUtility.GetString("FullFriends_description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowNoSelectFriends()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("NoSelectFriends_Title"), localizationUtility.GetString("NoSelectFriends_description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowFacebookFaild()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Error_Title"), localizationUtility.GetString("Facebook_Login_Failed_description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowNewVersion(bool force, UnityAction<bool> unityAction)
		{
            Debug.LogError("@LOG ShowNewVersion");
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon")
                .OnStart(localizationUtility.GetString("New_Version_Title"), 
                localizationUtility.GetString("New_Version_description"), 
                localizationUtility.GetString("btn_join_market"), unityAction, force);
		}

		public static void ShowQuitPlayScene()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Quit_Title"), localizationUtility.GetString("Quit_description"), localizationUtility.GetString("btn_yes"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (sure && PlayDesk.Get() != null)
				{
					PlayDesk.Get().GiveUp();
				}
			}, close: true);
		}

		public static void ShowNeedUpdateVersion()
		{
            Debug.LogError("@LOG ShowNeedUpdateVersion");
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon")
                .OnStart(localizationUtility.GetString("Version_TooLow_Title"), 
                localizationUtility.GetString("Version_TooLow_description"), 
                localizationUtility.GetString("btn_join_market"), delegate(bool sure)
			{
				if (sure)
				{
                    PlatformUtility.OnMarketJoin();
                }
				else
				{
					SingletonClass<MySceneManager>.Get().Close();
				}
			}, close: true);
		}

		public static void ShowDataDownloading(string msg = "")
		{
			Debug.Log("@LOG ShowDataDownloading... msg:" + msg);
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("data_downloading_title"), localizationUtility.GetString("data_downloading_desc"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowDownloading()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("Download Chapter Tips Title"), localizationUtility.GetString("Download Chapter Tips Description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowKeyInvalid(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("KeyInvalid_Title"), localizationUtility.GetString("KeyInvalid_Description"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (unityAction != null)
				{
					unityAction();
				}
			});
		}

		public static void ShowVideoWaiting(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("watch_ad_wating_title"), localizationUtility.GetString("watch_ad_wating_desc"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close(null, unityAction);
			});
		}

		public static void ShowComingSoon()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("ComingSoon_title"), localizationUtility.GetString("ComingSoon_desc"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowBonusErrorCode()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("TripeaksBonus_ErrorCode_Title"), localizationUtility.GetString("TripeaksBonus_ErrorCode_Des"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close();
			});
		}

		public static void ShowTitleDescription(string title, string description, UnityAction closed = null, UnityAction closing = null)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(title, description, localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close(null, closed);
				if (closing != null)
				{
					closing();
				}
			});
		}

		public static void ShowUnlockWorldScene(int world)
		{
			WorldConfig worldConfig = UniverseConfig.Get().GetWorldConfig(world - 1);
			if (worldConfig != null)
			{
				WorldConfig worldConfig2 = UniverseConfig.Get().GetWorldConfig(world);
				if (worldConfig2 != null)
				{
					string id = $"yy_world_unlock_{world + 1}";
					PurchasingEevet PurchasingSuccess = null;
					PurchasingSuccess = delegate(string transactionID, PurchasingPackage package)
					{
						if (package.commoditys.Count((PurchasingCommodity c) => c.boosterType == BoosterType.World) > 0)
						{
							SingletonClass<MySceneManager>.Get().Close();
							SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingSuccess);
						}
					};
					SingletonBehaviour<UnityPurchasingHelper>.Get().Insert(PurchasingSuccess);
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
					SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/WorldLockedTipPopup").OnStart(string.Format(localizationUtility.GetString("UnLock_World_Tips_Title"), worldConfig2.name).ToUpper(), string.Format(localizationUtility.GetString("UnLock_World_Tips_Description"), worldConfig.name, worldConfig2.name), UnityPurchasingConfig.Get().GetLocalizedPriceString(id), delegate(bool success)
					{
						if (success)
						{
							SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(new PurchasingPackage
							{
								id = id,
								Type = "World",
								commoditys = new PurchasingCommodity[2]
								{
									new PurchasingCommodity
									{
										boosterType = BoosterType.World,
										count = world
									},
									new PurchasingCommodity
									{
										boosterType = BoosterType.Coins,
										count = 15000
									}
								}
							});
						}
						else
						{
							SingletonClass<MySceneManager>.Get().Close();
							SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingSuccess);
							JoinPlayHelper.JoinPlayLevel(PlayData.Get().GetPlayScheduleData());
						}
					}, close: true);
				}
			}
		}
	}
}
