using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.U2D;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class MessageData
	{
		public string PartitionKey;

		public string RowKey;

		public string MessageId;

		public string SenderId;

		public string SenderName;

		public long SendTime;

		public string ReceiverId;

		public string ReceiverName;

		public string Content;

		public string Tag;

		public bool Read;

		public long ReadTime;

		public long ExpiredTime;

		public NewsConfig ToNewsConfig()
		{
			try
			{
				MessageContent messageContent = JsonUtility.FromJson<MessageContent>(Content);
				bool isCollect = !string.IsNullOrEmpty(messageContent.code);
				if (Read && isCollect)
				{
					return null;
				}
				if (Read && ExpiredTime <= 0)
				{
					return null;
				}
				LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_report.json");
				NewsConfig newsConfig;
				if ("Notice".Equals(Tag))
				{
					newsConfig = new NewsConfig();
					newsConfig.title = messageContent.title.Trim();
					newsConfig.description = messageContent.description.Trim();
					newsConfig.icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_bella");
					newsConfig.buttons = new List<ButtonLabel>
					{
						new ButtonLabel(localizationUtility.GetString("btn_details"))
					};
					newsConfig.RunAction = delegate(InboxNewsUI ui, int dex)
					{
						SingletonClass<MySceneManager>.Get().Popup<NoticeDetailsPopup>("Scenes/Pops/NoticeDetailsPopup").OnStart(messageContent, this, delegate
						{
							Read = true;
							SingletonClass<InboxUtility>.Get().UpdateNumber();
							if (isCollect)
							{
								ui.DestroyUI();
							}
						});
					};
					return newsConfig;
				}
				if (string.IsNullOrEmpty(messageContent.details))
				{
					messageContent.details = messageContent.description;
					if (messageContent.description.Length >= 30)
					{
						messageContent.description = messageContent.description.Substring(0, 30) + "...";
					}
				}
				newsConfig = new NewsConfig();
				newsConfig.title = messageContent.title.Trim();
				newsConfig.description = messageContent.description.Trim();
				newsConfig.icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_bella");
				newsConfig.buttons = new List<ButtonLabel>
				{
					new ButtonLabel(localizationUtility.GetString("btn_details"))
				};
				newsConfig.RunAction = delegate(InboxNewsUI ui, int dex)
				{
					SingletonClass<MySceneManager>.Get().Popup<NoticeDetailsPopup>("Scenes/Pops/NoticeDetailsPopup").OnStart(messageContent, this, delegate
					{
						ui.DestroyUI();
						Read = true;
						SingletonBehaviour<MessageUtility>.Get().ReadMessage(PartitionKey, RowKey);
						SingletonClass<InboxUtility>.Get().UpdateNumber();
					});
				};
				return newsConfig;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public bool IsUnread()
		{
			if (Read)
			{
				return false;
			}
			try
			{
				return JsonUtility.FromJson<MessageContent>(Content) != null;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
