using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ChatButtonUI : MonoBehaviour
	{
		public Button ChatButton;

		private void Start()
		{
			ChatButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ChatScene>("Scenes/ChatScene", new JoinEffect());
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(ChatButton.gameObject, number: false);
			});
			UpdateMyClubResponse(SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse);
			SingletonBehaviour<ClubSystemHelper>.Get().AddMyClubResponseListener(UpdateMyClubResponse);
			SingletonBehaviour<ClubSystemHelper>.Get().AddChatMessageListener(UpdateMessages);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(UpdateMyClubResponse);
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveChatMessageListener(UpdateMessages);
		}

		private void UpdateMyClubResponse(MyClubResponse response)
		{
			ChatButton.gameObject.SetActive(response != null && response.Club != null);
			if (ChatButton.gameObject.activeSelf)
			{
				SingletonBehaviour<ClubSystemHelper>.Get().GetLatestClubMessage();
			}
		}

		private void UpdateMessages(bool newMessage, List<Message> messages)
		{
			messages = messages.ToList();
			if (PlayerPrefs.HasKey("_LastChatTicks"))
			{
				long ticks = long.Parse(PlayerPrefs.GetString("_LastChatTicks"));
				messages.RemoveAll((Message e) => SingletonBehaviour<ClubSystemHelper>.Get().GetPlayerId().Equals(e.Author.PlayerId));
				messages.RemoveAll((Message e) => e.Ticks <= ticks);
			}
			SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(ChatButton.gameObject, messages.Count > 0);
		}
	}
}
