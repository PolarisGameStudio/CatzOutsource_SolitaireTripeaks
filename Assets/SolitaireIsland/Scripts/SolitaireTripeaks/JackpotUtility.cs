using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using TriPeaks.ProtoData.Jackpot;
using UnityEngine;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class JackpotUtility : SingletonBehaviour<JackpotUtility>
	{
		private static readonly object RequestLock = new object();

		private static int _requestId;

		private bool downing;

		public JackpotUserEvent JackpotUserChanged = new JackpotUserEvent();

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

		public void DownloadJackpot()
		{
			if (!downing)
			{
				downing = true;
				JackpotRequest jackpotRequest = new JackpotRequest();
				jackpotRequest.RequestId = RequestId;
				jackpotRequest.Cmd = 602;
				jackpotRequest.Platform = Application.platform.ToString();
				jackpotRequest.AppVersion = Application.version;
				jackpotRequest.Channel = "Official";
				jackpotRequest.Group = "Normal";
				jackpotRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				JackpotRequest obj = jackpotRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{NightingaleConfig.Get().LeaderBoardApi}/container/jackpot?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取Jackpot：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					downing = false;
					if (download.isDone)
					{
						JackpotResponse jackpotResponse = ProtoDataUtility.Deserialize<JackpotResponse>(download.data);
						if (jackpotResponse != null)
						{
							if (jackpotResponse.ErrorCode == 1)
							{
								JackpotUser arg = new JackpotUser
								{
									JackpotTime = new DateTime(jackpotResponse.JackpotTime, DateTimeKind.Utc),
									AvaterId = jackpotResponse.AvatarId,
									NickName = jackpotResponse.Nickname,
									SocailId = jackpotResponse.SocialId,
									SocialPlatform = jackpotResponse.SocialPlatform,
									JackpotId = jackpotResponse.JackpotId
								};
								UnityEngine.Debug.Log("获取Jackpot成功");
								JackpotUserChanged.Invoke(arg);
							}
							else
							{
								UnityEngine.Debug.Log($"获取Jackpot失败, 代码：{jackpotResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void UploadJackpot()
		{
			JackpotRequest jackpotRequest = new JackpotRequest();
			jackpotRequest.RequestId = RequestId;
			jackpotRequest.Cmd = 601;
			jackpotRequest.Platform = Application.platform.ToString();
			jackpotRequest.AppVersion = Application.version;
			jackpotRequest.Channel = "Official";
			jackpotRequest.Group = "Normal";
			jackpotRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			jackpotRequest.DeviceId = SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier;
			jackpotRequest.AvatarId = AuxiliaryData.Get().AvaterFileName;
			jackpotRequest.Nickname = AuxiliaryData.Get().GetNickName();
			jackpotRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			jackpotRequest.SocialPlatform = 1;
			JackpotRequest obj = jackpotRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{NightingaleConfig.Get().LeaderBoardApi}/container/jackpot?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------上传Jackpot：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				downing = false;
				if (download.isDone)
				{
					JackpotResponse jackpotResponse = ProtoDataUtility.Deserialize<JackpotResponse>(download.data);
					if (jackpotResponse != null)
					{
						if (jackpotResponse.ErrorCode == 1)
						{
							JackpotUser arg = new JackpotUser
							{
								JackpotTime = new DateTime(jackpotResponse.JackpotTime, DateTimeKind.Utc),
								AvaterId = jackpotResponse.AvatarId,
								NickName = jackpotResponse.Nickname,
								SocailId = jackpotResponse.SocialId,
								SocialPlatform = jackpotResponse.SocialPlatform,
								JackpotId = jackpotResponse.JackpotId
							};
							UnityEngine.Debug.Log("上传Jackpot成功");
							JackpotUserChanged.Invoke(arg);
						}
						else
						{
							UnityEngine.Debug.Log($"上传Jackpot失败, 代码：{jackpotResponse.ErrorCode}");
						}
					}
				}
			}));
		}
	}
}
