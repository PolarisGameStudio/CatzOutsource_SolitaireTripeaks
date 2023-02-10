using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ChatScene : BaseScene
	{
		public LoopScrollRect loopScrollRect;

		public InputField ChatInputField;

		public Button CloseButton;

		public Button SendButton;

		public Text ClubNameLabel;

		public Image ClubIcon;

		private bool initd;

		private void Start()
		{
			base.IsStay = true;
			loopScrollRect.OnFullRefresh.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().GetHistoryClubMessage();
			});
			loopScrollRect.OnFullLoad.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().GetLatestClubMessage();
			});
			UpdateChatMessage(newMessage: true, SingletonBehaviour<ClubSystemHelper>.Get().GetMessages());
			SingletonBehaviour<ClubSystemHelper>.Get().AddChatMessageListener(UpdateChatMessage);
			SingletonBehaviour<ClubSystemHelper>.Get().GetLatestClubMessage();
			SendButton.onClick.AddListener(delegate
			{
				string text = ChatInputField.text;
				if (!string.IsNullOrEmpty(text))
				{
					string clubIdentifier2 = SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier();
					if (!string.IsNullOrEmpty(clubIdentifier2))
					{
						SingletonBehaviour<ClubSystemHelper>.Get().SendChatMessage(text, clubIdentifier2);
						ChatInputField.text = string.Empty;
					}
				}
			});
			ChatInputField.onEndEdit.AddListener(delegate(string content)
			{
				if (!string.IsNullOrEmpty(content))
				{
					string clubIdentifier = SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier();
					if (!string.IsNullOrEmpty(clubIdentifier))
					{
						SingletonBehaviour<ClubSystemHelper>.Get().SendChatMessage(content, clubIdentifier);
						ChatInputField.text = string.Empty;
					}
				}
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
			});
			UpdateMyClubResponse(SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse);
			SingletonBehaviour<ClubSystemHelper>.Get().AddMyClubResponseListener(UpdateMyClubResponse);
			InvokeRepeating("RepeatingStart", 15f, 15f);
		}

		private void RepeatingStart()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().GetLatestClubMessage();
		}

		protected override void OnDestroy()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveChatMessageListener(UpdateChatMessage);
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(UpdateMyClubResponse);
		}

		private void UpdateMyClubResponse(MyClubResponse response)
		{
			if (response != null && response.Club != null)
			{
				ClubNameLabel.text = response.Club.ClubName;
				ClubIcon.sprite = SingletonBehaviour<ClubSystemHelper>.Get().GetClubAvatar(response.Club);
			}
		}

		private void UpdateChatMessage(bool newMessage, List<Message> chats)
		{
			if (chats.Count != 0)
			{
				PlayerPrefs.SetString("_LastChatTicks", chats.Max((Message e) => e.Ticks).ToString());
				PlayerPrefs.Save();
				loopScrollRect.objectsToFill = chats.ToArray();
				loopScrollRect.totalCount = chats.Count;
				loopScrollRect.RefreshCells();
				if (newMessage)
				{
					loopScrollRect.RefillCellsFromEnd();
					DelayDo(new WaitForSeconds(0.2f), delegate
					{
						loopScrollRect.RefillCellsFromEnd();
					});
				}
			}
		}
	}
}
