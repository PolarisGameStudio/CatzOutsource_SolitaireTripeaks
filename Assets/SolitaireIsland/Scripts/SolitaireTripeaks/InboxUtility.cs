using Nightingale.Socials;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class InboxUtility : SingletonClass<InboxUtility>
	{
		private readonly List<UnityAction<List<FacebookRequestData>>> unityActions = new List<UnityAction<List<FacebookRequestData>>>();

		public InboxNumberEvent InboxNumberChanged = new InboxNumberEvent();

		private List<FacebookRequestData> RequestDatas;

		public InboxUtility()
		{
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(delegate(bool login)
			{
				if (login)
				{
					AddListener(null);
				}
				else
				{
					foreach (UnityAction<List<FacebookRequestData>> unityAction in unityActions)
					{
						unityAction(new List<FacebookRequestData>());
					}
				}
			});
			Refresh();
		}

		public void LocalRefresh(List<NewsConfig> news)
		{
			UpdateNumber();
		}

		public void Refresh()
		{
			UpdateNumber();
			TaskHelper.Get("Facebook").AppendTask(new NormalTask(string.Empty, SingletonBehaviour<FacebookMananger>.Get().GetAppRequests)).RemoveAllListeners()
				.AddListener(delegate(object data, float p)
				{
					if (data != null)
					{
						UpdateInboxDatas(data as List<FacebookRequestData>);
					}
				});
			SingletonBehaviour<MessageUtility>.Get().ReceiveMessageChanged.AddListener(UpdateNumber);
			SingletonBehaviour<MessageUtility>.Get().ListReceiveMessage();
		}

		public void AddListener(UnityAction<List<FacebookRequestData>> unityAction)
		{
			if (unityAction != null)
			{
				unityActions.Add(unityAction);
			}
			Refresh();
		}

		public void RemoveListener(UnityAction<List<FacebookRequestData>> unityAction)
		{
			if (unityActions.Contains(unityAction))
			{
				unityActions.Remove(unityAction);
			}
		}

		public void ClearAppRequest(FacebookRequestData data)
		{
			if (RequestDatas != null && RequestDatas.Contains(data))
			{
				RequestDatas.Remove(data);
				SingletonBehaviour<FacebookMananger>.Get().ClearAppRequest(data.id);
				UpdateNumber();
			}
		}

		public void CollectDailyBonus()
		{
			AuxiliaryData.Get().DailyBonusRewards = 0;
			AuxiliaryData.Get().DailyBonusCount++;
			UpdateNumber();
		}

		public List<NewsConfig> GetNewsConfig()
		{
			List<NewsConfig> list = new List<NewsConfig>();
			list.AddRange(SingletonBehaviour<ClubSystemHelper>.Get().GetNewsConfigs());
			list.AddRange(NewsConfig.CreateDailyBonusConfig());
			return (from e in list
				orderby e.Order
				orderby SingletonData<DeviceFileData>.Get().Contains(e.identifier)
				select e).ToList();
		}

		public void UpdateNumber()
		{
			int num = 0;
			if (RequestDatas != null)
			{
				num += ((RequestDatas.Count((FacebookRequestData e) => "Ask".Equals(e.data)) > 0) ? 1 : 0);
				num += ((RequestDatas.Count((FacebookRequestData e) => "Invite".Equals(e.data)) > 0) ? 1 : 0);
				num += ((RequestDatas.Count((FacebookRequestData e) => "Asked".Equals(e.data)) > 0) ? 1 : 0);
				num += RequestDatas.Count((FacebookRequestData e) => "Invited".Equals(e.data));
			}
			num += GetNewsConfig().Count((NewsConfig e) => !e.HidingInNumber && !SingletonData<DeviceFileData>.Get().Contains(e.identifier));
			num += SystemMessageData.GetReceive().GetUnReadMessages();
			InboxNumberChanged.Invoke(num);
		}

		private void UpdateInboxDatas(List<FacebookRequestData> datas)
		{
			try
			{
				List<FacebookRequestData> list = new List<FacebookRequestData>();
				foreach (FacebookRequestData item in datas)
				{
					if ("Invited".Equals(item.data))
					{
						if (list.Find((FacebookRequestData e) => e.data.Equals(item.data) && e.fromUser.id.Equals(item.fromUser.id)) != null || AuxiliaryData.Get().InvitableFriended.Contains(item.fromUser.id))
						{
							SingletonBehaviour<FacebookMananger>.Get().ClearAppRequest(item.id);
							continue;
						}
					}
					else if (list.Find((FacebookRequestData e) => e.data.Equals(item.data) && e.fromUser.id.Equals(item.fromUser.id)) != null)
					{
						SingletonBehaviour<FacebookMananger>.Get().ClearAppRequest(item.id);
						continue;
					}
					list.Add(item);
				}
				RequestDatas = list;
				foreach (UnityAction<List<FacebookRequestData>> unityAction in unityActions)
				{
					unityAction(list);
				}
				UpdateNumber();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}
	}
}
