using Nightingale.Localization;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Club;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class InboxScene : SoundScene
	{
		public Transform RewardsTransform;

		public GameObject LoadingGameObject;

		private void Start()
		{
			LoadingGameObject.SetActive(value: true);
			base.IsStay = true;
			SingletonClass<InboxUtility>.Get().AddListener(DownloadRequestDataCompleted);
			UpdateMessage();
			SingletonBehaviour<MessageUtility>.Get().ReceiveMessageChanged.AddListener(UpdateMessage);
			UpdateClub(null);
			if (SingletonBehaviour<ClubSystemHelper>.Get().IsActive())
			{
				SingletonBehaviour<ClubSystemHelper>.Get().AddMyClubResponseListener(UpdateClub);
				SingletonBehaviour<ClubSystemHelper>.Get().MyClub();
			}
		}

		private void UpdateMessage()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_inbox.json");
			InboxNewsUI[] componentsInChildren = RewardsTransform.GetComponentsInChildren<InboxNewsUI>();
			foreach (InboxNewsUI inboxNewsUI in componentsInChildren)
			{
				if (inboxNewsUI.gameObject.name == "NTG-Message")
				{
					UnityEngine.Object.Destroy(inboxNewsUI.gameObject);
				}
			}
			foreach (MessageData message in SystemMessageData.GetReceive().GetMessages())
			{
				NewsConfig newsConfig = message.ToNewsConfig();
				if (newsConfig != null)
				{
					GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(InboxScene).Name, "UI/Inboxs/InboxNewsUI"));
					gameObject.transform.gameObject.name = "NTG-Message";
					gameObject.transform.SetParent(RewardsTransform, worldPositionStays: false);
					gameObject.GetComponent<InboxNewsUI>().SetNewsConfig(newsConfig);
					if (message.Read)
					{
						gameObject.transform.SetAsLastSibling();
					}
					else
					{
						gameObject.transform.SetAsFirstSibling();
					}
				}
			}
		}

		private void UpdateClub(MyClubResponse response)
		{
			InboxNewsUI[] componentsInChildren = RewardsTransform.GetComponentsInChildren<InboxNewsUI>();
			foreach (InboxNewsUI inboxNewsUI in componentsInChildren)
			{
				if (inboxNewsUI.gameObject.name == "Normal")
				{
					UnityEngine.Object.Destroy(inboxNewsUI.gameObject);
				}
			}
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(InboxScene).Name, "UI/Inboxs/InboxNewsUI");
			foreach (NewsConfig item in SingletonClass<InboxUtility>.Get().GetNewsConfig())
			{
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.gameObject.name = "Normal";
				gameObject.transform.SetParent(RewardsTransform, worldPositionStays: false);
				gameObject.GetComponent<InboxNewsUI>().SetNewsConfig(item);
			}
		}

		private void DownloadRequestDataCompleted(List<FacebookRequestData> requests)
		{
			LoadingGameObject.SetActive(value: false);
			GameObject gameObject = null;
			FacebookRequestData[] array = null;
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(InboxScene).Name, "UI/Inboxs/InboxFbFriendMessageUI");
			array = (from e in requests
				where "Invite".Equals(e.data)
				select e).ToArray();
			if (array.Count() > 0)
			{
				gameObject = Object.Instantiate(asset, RewardsTransform);
				gameObject.GetComponent<InboxMessageUI>().SetInvites(array);
				gameObject.transform.SetAsFirstSibling();
			}
			array = (from e in requests
				where "Ask".Equals(e.data)
				select e).ToArray();
			if (array.Count() > 0)
			{
				gameObject = Object.Instantiate(asset, RewardsTransform);
				gameObject.GetComponent<InboxMessageUI>().SetAsk(array);
				gameObject.transform.SetAsFirstSibling();
			}
			array = (from e in requests
				where "Asked".Equals(e.data)
				select e).ToArray();
			if (array.Count() > 0)
			{
				gameObject = Object.Instantiate(asset, RewardsTransform);
				gameObject.GetComponent<InboxMessageUI>().SetAsked(array);
				gameObject.transform.SetAsFirstSibling();
			}
			array = (from e in requests
				where "Invited".Equals(e.data)
				select e).ToArray();
			FacebookRequestData[] array2 = array;
			foreach (FacebookRequestData invited in array2)
			{
				gameObject = Object.Instantiate(asset, RewardsTransform);
				gameObject.GetComponent<InboxMessageUI>().SetInvited(invited);
				gameObject.transform.SetAsFirstSibling();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonClass<InboxUtility>.Get().RemoveListener(DownloadRequestDataCompleted);
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(UpdateClub);
			SingletonData<DeviceFileData>.Get().Remove();
			SingletonBehaviour<SynchronizeUtility>.Get().UploadGameData();
		}
	}
}
