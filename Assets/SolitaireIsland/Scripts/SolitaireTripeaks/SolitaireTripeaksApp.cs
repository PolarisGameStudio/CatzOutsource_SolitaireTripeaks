using Nightingale;
using Nightingale.Localization;
using Nightingale.Notifications;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SolitaireTripeaksApp : App
	{
		protected override void OnAppStart()
		{
			Input.multiTouchEnabled = false;
			Application.targetFrameRate = 60;
			AudioUtility.GetMusic().SetVolume(SingletonData<SettingData>.Get().musicVolume);
			AudioUtility.GetSound().SetVolume(SingletonData<SettingData>.Get().soundVolume);
			NtgNativeAgent.Init();
			SingletonBehaviour<FacebookMananger>.Get().Initial(delegate
			{
				DelayDo(delegate
				{
					SingletonClass<MySceneManager>.Get().Navigation<LoadingScene>("Scenes/LoadingScene");
					SingletonBehaviour<LeaderBoardUtility>.Get().OnAppStart();
					SingletonBehaviour<ClubSystemHelper>.Get().OnAppStart();
					SingletonBehaviour<MessageUtility>.Get().OnAppStart();
				});
			});
		}

		protected override void OnAppActive()
		{
#if ENABLE_LOCAL_NOTIFICATION
            Nightingale.Notifications.LocalNotification.CleanNotification();
#endif
			Screen.sleepTimeout = -1;
		}

		protected override void OnAppTombstone()
		{
			SingletonData<DeviceFileData>.Get().FlushData();
			SolitaireTripeaksData.Get().FlushData();
			SingletonData<DevicePlayLevels>.Get().FlushData();
			SingletonClass<QuestHelper>.Get().OpenNotification();
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Notification.json");
			for (int i = 1; i <= 30; i++)
			{
				DateTime dateTime = DateTime.Now.AddDays(i);
#if ENABLE_LOCAL_NOTIFICATION
                Nightingale.Notifications.LocalNotification.NotificationMessage(timeSpan: new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 30, 0).Subtract(DateTime.Now),
                    title: localizationUtility.GetString("Default_Notification_Title"), 
                    message: localizationUtility.GetString("Default_Notification_Message"));
#endif
			}
			AuxiliaryData.Get().NotificationOfflineCoins();
			SingletonBehaviour<SpecialActivityUtility>.Get().Notification();
			Screen.sleepTimeout = -2;
		}

		public override void OnLeaveLongTime(double hours)
		{
			base.OnLeaveLongTime(hours);
			if (hours > 2.0 || hours < 0.0)
			{
				UnityEngine.Debug.Log("////////////////////////强制重启游戏");
				SingletonClass<MySceneManager>.Get().Navigation<LoadingScene>("Scenes/LoadingScene");
				SingletonBehaviour<LeaderBoardUtility>.Get().OnAppStart();
				SingletonBehaviour<ClubSystemHelper>.Get().OnAppStart();
				SingletonBehaviour<MessageUtility>.Get().OnAppStart();
				TaskHelper.GetDownload().tasks.ForEach(delegate(NormalTask e)
				{
					(e as RemoteAssetTask)?.Rest();
				});
			}
		}

		private void Update()
		{
			SolitaireTripeaksData.Get().OnUpdate();
		}
	}
}
