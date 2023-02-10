using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using TriPeaks.ProtoData.Facebook;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class SynchronizeUtility : SingletonBehaviour<SynchronizeUtility>
	{
		private static readonly object RequestLock = new object();

		private static int _requestId;

		private string _facebookApi;

		private SynchronizeDownloadState synchronizeDownloadState;

		private float uploading;

		private static int RequestId
		{
			get
			{
				lock (RequestLock)
				{
					return ++_requestId;
				}
			}
		}

		private void Awake()
		{
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(delegate(bool login)
			{
				if (login)
				{
					SynchronizeGameData();
					UnityEngine.Debug.LogWarningFormat("Current Facebook Account：" + GetFacebookId());
				}
			});
			SingletonBehaviour<FacebookMananger>.Get().LoginFaild.AddListener(delegate
			{
				TipPopupNoIconScene.ShowFacebookFaild();
			});
			_facebookApi = NightingaleConfig.Get().FacebookApi;
			SynchronizeGameData();
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				SynchronizeGameData();
			}
		}

		private string GetFacebookId()
		{
			return SingletonBehaviour<FacebookMananger>.Get().UserId;
		}

		public void Init()
		{
		}

		public void SynchronizeGameData(UnityAction unityAction = null)
		{
			SynchronizeGameData(GetFacebookId(), unityAction);
		}

		public void SynchronizeGameData(string id, UnityAction unityAction = null)
		{
			try
			{
				if (!string.IsNullOrEmpty(id) && synchronizeDownloadState != SynchronizeDownloadState.Download && synchronizeDownloadState != SynchronizeDownloadState.Choose)
				{
					DownloadRequest downloadRequest = new DownloadRequest();
					downloadRequest.RequestId = RequestId;
					downloadRequest.Cmd = 302;
					downloadRequest.AppVersion = Application.version;
					downloadRequest.FacebookId = id;
					downloadRequest.ExcludeDeviceId = ((!SingletonData<SynchronizeGroupData>.Get().GetLocalDeviceId(id)) ? string.Empty : SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier);
					DownloadRequest obj = downloadRequest;
					UnityWebRequest uwr = UnityWebRequest.Post($"{_facebookApi}/container/facebook", new Dictionary<string, string>
					{
						{
							"args",
							ProtoDataUtility.SerializeToBase64(obj)
						}
					});
					UnityEngine.Debug.LogFormat("------------------------ Synchronize Game Data {0}：{1}", uwr.url, ProtoDataUtility.SerializeToBase64(obj));
					synchronizeDownloadState = SynchronizeDownloadState.Download;
					DownloadResponse response;
					StartCoroutine(StartUnityWeb(uwr, delegate(DownloadHandler download)
					{
						try
						{
							UnityEngine.Debug.Log("---------------------DO Synchronize Game Data--------------------------");
							if (!download.isDone)
							{
								UnityEngine.Debug.LogFormat("Synchronize Game Data Error  {0}, Error CODE {1}", uwr.error, uwr.responseCode);
								synchronizeDownloadState = SynchronizeDownloadState.Error;
								SingletonBehaviour<StepUtility>.Get().Append("SynchronizeDownload", 10f, delegate
								{
									SynchronizeGameData(id, unityAction);
								});
								goto IL_03de;
							}
							response = ProtoDataUtility.Deserialize<DownloadResponse>(download.data);
							SolitaireTripeaksData solitaireTripeaksData;
							if (response != null)
							{
								if (response.ErrorCode != 1)
								{
									synchronizeDownloadState = SynchronizeDownloadState.Error;
									SingletonBehaviour<StepUtility>.Get().Append("SynchronizeDownload", 10f, delegate
									{
										SynchronizeGameData(id, unityAction);
									});
									UnityEngine.Debug.LogFormat("Synchronize Game Data Error {0}", response.ErrorCode);
									goto IL_03de;
								}
								UnityEngine.Debug.Log("Synchronize Game Data Success.");
								if (!response.FacebookId.Equals(id))
								{
									synchronizeDownloadState = SynchronizeDownloadState.None;
									goto IL_03de;
								}
								if (response.FirstLogin)
								{
									UnityEngine.Debug.Log("Synchronize Game Data : First Login.");
									AchievementData.Get().DoAchievement(AchievementType.LoginFacebook);
									synchronizeDownloadState = SynchronizeDownloadState.Completed;
									AuxiliaryData.Get().RewardsFacebook = true;
									UploadGameData();
									SingletonBehaviour<TripeaksPlayerHelper>.Get().UploadTripeaksPlayer();
									StatisticsData.Get().LoginTicks = DateTime.UtcNow.Ticks;
									PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
									{
										new PurchasingCommodity
										{
											count = 3,
											boosterType = BoosterType.Wild
										}
									});
									SessionData.Get().PutCommodity(BoosterType.Wild, CommoditySource.Free, 3L);
									goto IL_02e8;
								}
								if (response.Data == null)
								{
									synchronizeDownloadState = SynchronizeDownloadState.Completed;
									UnityEngine.Debug.Log("Synchronize Game Data : No Data in Server.");
									goto IL_02e8;
								}
								GameData data = response.Data;
								if (SingletonData<SynchronizeGroupData>.Get().HasTicks(response.FacebookId, response.Data.Ticks))
								{
									UnityEngine.Debug.Log("Synchronize Game Data Processed.");
									synchronizeDownloadState = SynchronizeDownloadState.Completed;
								}
								else
								{
									solitaireTripeaksData = null;
									if ("old".Equals(data.DeviceId, StringComparison.OrdinalIgnoreCase))
									{
										UnityEngine.Debug.Log("Synchronize Game Data : Read From Old Version.");
										SolitaireTripeaksData data2 = SolitaireTripeaksData.GetData(data.Data);
										if (data2.Play.GetMax() > PlayData.Get().GetMax())
										{
											solitaireTripeaksData = data2;
											UnityEngine.Debug.Log("Synchronize Game Data : Want this data in client.");
										}
										else
										{
											UnityEngine.Debug.Log("Synchronize Game Data : Don't want this data in client.");
										}
										goto IL_027e;
									}
									UnityEngine.Debug.Log("Synchronize Game Data : DecompressString ing.");
									solitaireTripeaksData = SolitaireTripeaksData.GetData(data.Data);
									if (solitaireTripeaksData != null)
									{
										goto IL_027e;
									}
									synchronizeDownloadState = SynchronizeDownloadState.Error;
									SingletonBehaviour<StepUtility>.Get().Append("SynchronizeDownload", 10f, delegate
									{
										SynchronizeGameData(id, unityAction);
									});
								}
							}
							goto end_IL_0000;
							IL_027e:
							if (solitaireTripeaksData != null)
							{
								synchronizeDownloadState = SynchronizeDownloadState.Choose;
								UnityEngine.Debug.Log("Synchronize Game Data User Choose Data.");
								SingletonClass<MySceneManager>.Get().Popup<SynchronizeScene>("Scenes/SynchronizeScene", new JoinEffect()).OnStart(solitaireTripeaksData, delegate
								{
									synchronizeDownloadState = SynchronizeDownloadState.Completed;
									SingletonData<SynchronizeGroupData>.Get().PutData(response.FacebookId, response.Data.Ticks);
									SingletonData<SynchronizeGroupData>.Get().FlushData();
								});
							}
							else
							{
								UnityEngine.Debug.Log("Synchronize No Game Data.");
								synchronizeDownloadState = SynchronizeDownloadState.Completed;
							}
							goto IL_02e8;
							IL_02e8:
							if (unityAction != null)
							{
								unityAction();
							}
							goto IL_03de;
							IL_03de:
							UnityEngine.Debug.Log("---------------------DO END Synchronize Game Data--------------------------");
							end_IL_0000:;
						}
						catch (Exception ex2)
						{
							UnityEngine.Debug.Log(ex2.Message);
						}
					}));
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void UploadGameData()
		{
			string facebookId = GetFacebookId();
			if (!string.IsNullOrEmpty(facebookId))
			{
				if (synchronizeDownloadState == SynchronizeDownloadState.Completed)
				{
					UploadGameData(facebookId);
				}
			}
			else
			{
				UploadGameData(string.Empty);
			}
		}

		public void UploadGameData(string id, UnityAction<string> unityAction = null)
		{
			try
			{
                UnityEngine.Debug.Log("--------------------- Upload Game Data");
                AuxiliaryData.Get().MaxLevel = PlayData.Get().GetMax();
				AuxiliaryData.Get().MaxMasterLevel = PlayData.Get().GetMaxMasterLevels();
				UploadRequest request = new UploadRequest
				{
					RequestId = RequestId,
					Cmd = 301,
					AppVersion = Application.version,
					FacebookId = id,
					DeviceId = SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier,
					Data = SolitaireTripeaksData.Get().ToString(compress: true),
					Level = PlayData.Get().GetMax(),
					GameTime = SaleData.Get().GetOnlineSecond(),
					PlayerId = SolitaireTripeaksData.Get().GetPlayerId(),
					Name = AuxiliaryData.Get().GetNickName(),
					AvatarId = AuxiliaryData.Get().AvaterFileName,
					Coins = PackData.Get().GetCommodity(BoosterType.Coins).GetTotal()
				};
#if ENABLE_UPLOAD_DATA
				UnityWebRequest uwr = UnityWebRequest.Post($"{_facebookApi}/container/facebook", new Dictionary<string, string>
				{
					{
						"args",
						ProtoDataUtility.SerializeToBase64(request)
					}
				});
				UnityEngine.Debug.LogFormat("--------------------- Upload Game Data{0}：{1}", uwr.url, ProtoDataUtility.SerializeToBase64(request));
                StartCoroutine(StartUnityWeb(uwr, delegate(DownloadHandler download)
				{
					try
					{
						UnityEngine.Debug.Log("---------------------DO Upload Game Data--------------------------");
						if (!download.isDone)
						{
							SingletonBehaviour<StepUtility>.Get().Append("SynchronizeUpload", 10f, UploadGameData);
							UnityEngine.Debug.LogFormat("Upload Game Data Error: {0}, Error Code: {1}", uwr.error, uwr.responseCode);
							goto IL_0139;
						}
						UploadResponse uploadResponse = ProtoDataUtility.Deserialize<UploadResponse>(download.data);
						if (uploadResponse != null)
						{
							if (uploadResponse.ErrorCode != 1)
							{
								SingletonBehaviour<StepUtility>.Get().Append("SynchronizeUpload", 10f, UploadGameData);
								UnityEngine.Debug.LogFormat("Upload Game Data Error: {0}", uploadResponse.ErrorCode);
								goto IL_0139;
							}
							UnityEngine.Debug.Log("Upload Game Data Completed.");
							SingletonData<SynchronizeGroupData>.Get().PutUpload(request.FacebookId);
							SingletonData<SynchronizeGroupData>.Get().FlushData();
							if (unityAction != null)
							{
								unityAction(id);
							}
						}
						goto end_IL_0000;
						IL_0139:
						UnityEngine.Debug.Log("---------------------DO END Upload Game Data--------------------------");
						end_IL_0000:;
					}
					catch (Exception ex2)
					{
						UnityEngine.Debug.Log(ex2.Message);
					}
				}));
#endif
            }
            catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}
	}
}
