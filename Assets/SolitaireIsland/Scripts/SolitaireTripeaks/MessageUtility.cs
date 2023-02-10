using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using TriPeaks.ProtoData;
using TriPeaks.ProtoData.Message;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class MessageUtility : SingletonBehaviour<MessageUtility>
	{
		private static readonly object RequestLock = new object();

		private static int _requestId;

		private string _messageApi;

		public UnityEvent ReceiveMessageChanged = new UnityEvent();

		public UnityEvent SendMessageChanged = new UnityEvent();

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

		public void OnAppStart()
		{
			_messageApi = NightingaleConfig.Get().MessageApi;
		}

		public void SendMessage(string receiverId, string receiverName, string tag, string content)
		{
			SendRequest sendRequest = new SendRequest();
			sendRequest.RequestId = RequestId;
			sendRequest.Cmd = 501;
			sendRequest.Platform = Application.platform.ToString();
			sendRequest.AppVersion = Application.version;
			sendRequest.Channel = "Official";
			sendRequest.Group = "Normal";
			sendRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			sendRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			sendRequest.Content = content;
			sendRequest.ReceiverId = receiverId;
			sendRequest.ReceiverName = receiverName;
			sendRequest.Tag = tag;
			SendRequest obj = sendRequest;
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("args", WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj)));
			UnityWebRequest unityWebRequest = UnityWebRequest.Post($"{_messageApi}/container/message", wWWForm);
			UnityEngine.Debug.Log("-------------------发送消息：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_report.json");
					if (baseResponse != null && baseResponse.ErrorCode == 1)
					{
						string extraState = baseResponse.ExtraState;
						UnityEngine.Debug.Log($"发送消息成功，MessageId: {extraState}");
						SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("title_report_completed"), localizationUtility.GetString("desc_report_completed"), localizationUtility.GetString("btn_ok"), delegate
						{
							SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
						});
					}
					else
					{
						UnityEngine.Debug.LogWarning($"发送消息失败，代码：{baseResponse.ErrorCode}");
						SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("title_report_faild"), localizationUtility.GetString("desc_report_faild"), localizationUtility.GetString("btn_ok"), delegate
						{
							SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
						});
					}
				}
			}));
		}

		public void ReadMessage(string partitionKey, string messageId)
		{
			ReadRequest readRequest = new ReadRequest();
			readRequest.RequestId = RequestId;
			readRequest.Cmd = 504;
			readRequest.Platform = Application.platform.ToString();
			readRequest.AppVersion = Application.version;
			readRequest.Channel = "Official";
			readRequest.Group = "Normal";
			readRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			readRequest.PartitionKey = partitionKey;
			readRequest.MessageId = messageId;
			ReadRequest obj = readRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_messageApi}/container/message?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------阅读消息：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							string extraState = baseResponse.ExtraState;
							UnityEngine.Debug.Log($"阅读消息成功，MessageId: {extraState}");
						}
						else
						{
							UnityEngine.Debug.LogWarning($"阅读消息失败，代码：{baseResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void ListSendMessage()
		{
#if ENABLE_AZURE
			ListMessageRequest listMessageRequest = new ListMessageRequest();
			listMessageRequest.RequestId = RequestId;
			listMessageRequest.Cmd = 502;
			listMessageRequest.Platform = Application.platform.ToString();
			listMessageRequest.AppVersion = Application.version;
			listMessageRequest.Channel = "Official";
			listMessageRequest.Group = "Normal";
			listMessageRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			listMessageRequest.LastTicks = SystemMessageData.GetSend().lastTicks.ToString();
			ListMessageRequest obj = listMessageRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_messageApi}/container/message?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------获取自己发送的消息列表：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					ListMessageResponse listMessageResponse = ProtoDataUtility.Deserialize<ListMessageResponse>(download.data);
					if (listMessageResponse != null)
					{
						if (listMessageResponse.ErrorCode == 1)
						{
							SystemMessageData.GetSend().SaveMessages(listMessageResponse.Messages.ToList(), listMessageResponse.LastTicks);
							SendMessageChanged.Invoke();
							UnityEngine.Debug.Log($"获取自己发送的消息列表成功，MessageCount: {listMessageResponse.Messages.Count}");
						}
						else
						{
							UnityEngine.Debug.LogWarning($"获取自己发送的消息列表失败，代码：{listMessageResponse.ErrorCode}");
						}
					}
				}
			}));
#endif
        }

        public void ListReceiveMessage()
		{
#if ENABLE_AZURE
            ListMessageRequest listMessageRequest = new ListMessageRequest();
			listMessageRequest.RequestId = RequestId;
			listMessageRequest.Cmd = 503;
			listMessageRequest.Platform = Application.platform.ToString();
			listMessageRequest.AppVersion = Application.version;
			listMessageRequest.Channel = "Official";
			listMessageRequest.Group = "Normal";
			listMessageRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			listMessageRequest.LastTicks = SystemMessageData.GetReceive().lastTicks.ToString();
			ListMessageRequest obj = listMessageRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_messageApi}/container/message?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------获取别人发给我的消息列表：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					ListMessageResponse listMessageResponse = ProtoDataUtility.Deserialize<ListMessageResponse>(download.data);
					if (listMessageResponse != null)
					{
						if (listMessageResponse.ErrorCode == 1)
						{
							SystemMessageData.GetReceive().SaveMessages(listMessageResponse.Messages.ToList(), listMessageResponse.LastTicks);
							ReceiveMessageChanged.Invoke();
							UnityEngine.Debug.Log($"获取别人发给我的消息列表成功，MessageCount: {listMessageResponse.Messages.Count}");
						}
						else
						{
							UnityEngine.Debug.LogWarning($"获取别人发给我的消息列表失败，代码：{listMessageResponse.ErrorCode}");
						}
					}
				}
			}));
#endif
		}
	}
}
