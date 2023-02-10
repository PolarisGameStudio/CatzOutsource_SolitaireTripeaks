using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.SensitiveWords;
using Nightingale.Socials;
using Nightingale.Toasts;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData;
using TriPeaks.ProtoData.Club;
using TriPeaks.ProtoData.Leaderboard;
using TriPeaks.ProtoData.Shared;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class ClubSystemHelper : SingletonBehaviour<ClubSystemHelper>
	{
		private ClubEvent _ClubEvent = new ClubEvent();

		private MyClubResponseEvent _MyClubResponseEvent = new MyClubResponseEvent();

		private ClubGroupEvent _PageClubGroupEvent = new ClubGroupEvent();

		private ClubGroupEvent _SearchClubGroupEvent = new ClubGroupEvent();

		private MessageGroupEvent _MessageGroupEventEvent = new MessageGroupEvent();

		private readonly Dictionary<string, List<Message>> MessagesGroup = new Dictionary<string, List<Message>>();

		private List<string> runnings = new List<string>();

		private static readonly object RequestLock = new object();

		private static int _requestId;

		private string _clubApi;

		public const string invitationTag = "Club_Invitation";

		public const string normalTag = "Club_Normal";

		public const string giftTag = "Club_Gift";

		public const string thanksTag = "Club_Thanks";

		private Dictionary<string, ClubLeaderboardListResponse> _Responses = new Dictionary<string, ClubLeaderboardListResponse>();

		public SegmentType Segment = SegmentType.Bronze;

		public List<RewardItem> Rewards = new List<RewardItem>();

		public ClubLeaderboardEvent RankEvent = new ClubLeaderboardEvent();

		private bool _connect;

		private double StateCountdown;

		private RankType _lastRankType;

		private RankType _currentRankType;

		public RankTypeEvent RankChanged = new RankTypeEvent();

		public MyClubResponse _MyClubResponse
		{
			get;
			private set;
		}

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
			_clubApi = NightingaleConfig.Get().ClubApi;
			MyClub();
			GetLeaderboardState();
			GetLeaderboardReward();
			foreach (GiftData giftData in SingletonData<ClubPlayerData>.Get().GetGiftDatas())
			{
				SendGift(giftData);
			}
		}

		public bool ContainsTaskId(string taskId)
		{
			return runnings.Contains(taskId);
		}

		public void AppendTaskId(string taskId)
		{
			if (!ContainsTaskId(taskId))
			{
				runnings.Add(taskId);
			}
		}

		public void RemoveTaskId(string taskId)
		{
			if (ContainsTaskId(taskId))
			{
				runnings.Remove(taskId);
			}
		}

		public bool IsActive()
		{
			return PlayData.Get().HasThanLevelData(0, 0, 20);
		}

		public string GetClubIdentifier()
		{
			if (_MyClubResponse == null)
			{
				return string.Empty;
			}
			if (_MyClubResponse.Club == null)
			{
				return string.Empty;
			}
			return _MyClubResponse.Club.ClubId;
		}

		public void AddChatMessageListener(UnityAction<bool, List<Message>> unityAction)
		{
			if (unityAction != null)
			{
				_MessageGroupEventEvent.AddListener(unityAction);
			}
		}

		public void RemoveChatMessageListener(UnityAction<bool, List<Message>> unityAction)
		{
			if (unityAction != null)
			{
				_MessageGroupEventEvent.RemoveListener(unityAction);
			}
		}

		public void AddPageClubListener(UnityAction<int, List<Club>> unityAction)
		{
			if (unityAction != null)
			{
				_PageClubGroupEvent.AddListener(unityAction);
			}
		}

		public void RemovePageClubListener(UnityAction<int, List<Club>> unityAction)
		{
			if (unityAction != null)
			{
				_PageClubGroupEvent.RemoveListener(unityAction);
			}
		}

		public void AddSearchClubListener(UnityAction<int, List<Club>> unityAction)
		{
			if (unityAction != null)
			{
				_SearchClubGroupEvent.AddListener(unityAction);
			}
		}

		public void RemoveSearchClubListener(UnityAction<int, List<Club>> unityAction)
		{
			if (unityAction != null)
			{
				_SearchClubGroupEvent.RemoveListener(unityAction);
			}
		}

		public void AddClubListener(UnityAction<string, Club> unityAction)
		{
			if (unityAction != null)
			{
				_ClubEvent.AddListener(unityAction);
			}
		}

		public void RemoveClubListener(UnityAction<string, Club> unityAction)
		{
			if (unityAction != null)
			{
				_ClubEvent.RemoveListener(unityAction);
			}
		}

		public void AddMyClubResponseListener(UnityAction<MyClubResponse> unityAction)
		{
			if (unityAction != null)
			{
				_MyClubResponseEvent.AddListener(unityAction);
			}
		}

		public void RemoveMyClubResponseListener(UnityAction<MyClubResponse> unityAction)
		{
			if (unityAction != null)
			{
				_MyClubResponseEvent.RemoveListener(unityAction);
			}
		}

		public string GetTimeAgo(long ticks)
		{
			return GetTimeAgo(DateTime.UtcNow.Subtract(new DateTime(ticks)));
		}

		public string GetTimeAgo(TimeSpan sub)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			if (sub.TotalDays >= 1.0)
			{
				return string.Format(localizationUtility.GetString("days_ago"), (int)sub.TotalDays);
			}
			if (sub.TotalHours >= 1.0)
			{
				return string.Format(localizationUtility.GetString("hours_ago"), (int)sub.TotalHours);
			}
			if (sub.TotalMinutes >= 1.0)
			{
				return string.Format(localizationUtility.GetString("minutes_ago"), (int)sub.TotalMinutes);
			}
			return localizationUtility.GetString("just_a_moment_ago");
		}

		public string GetOfflineTimeAgo(long seconds)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TimeSpan timeSpan = TimeSpan.FromTicks(seconds);
			if (timeSpan.TotalDays >= 1.0)
			{
				return string.Format(localizationUtility.GetString("days_ago"), (int)timeSpan.TotalDays);
			}
			if (timeSpan.TotalHours >= 1.0)
			{
				return string.Format(localizationUtility.GetString("hours_ago"), (int)timeSpan.TotalHours);
			}
			if (timeSpan.TotalMinutes >= 1.0)
			{
				return string.Format(localizationUtility.GetString("minutes_ago"), (int)timeSpan.TotalMinutes);
			}
			return localizationUtility.GetString("active");
		}

		public string GetRole(int role)
		{
			switch (role)
			{
			case 1:
				return LocalizationUtility.Get("Localization_Club.json").GetString("Leader");
			case 2:
				return LocalizationUtility.Get("Localization_Club.json").GetString("Co-Leader");
			default:
				return LocalizationUtility.Get("Localization_Club.json").GetString("Member");
			}
		}

		public ClubRoles GetClubRoles(int role)
		{
			switch (role)
			{
			case 1:
				return ClubRoles.Chairman;
			case 2:
				return ClubRoles.ViceChairman;
			default:
				return ClubRoles.Member;
			}
		}

		public string GetMembership(bool isPrivate)
		{
			if (isPrivate)
			{
				return LocalizationUtility.Get("Localization_Club.json").GetString("club_ship_private");
			}
			return LocalizationUtility.Get("Localization_Club.json").GetString("club_ship_public");
		}

		public ClubRoles GetRoleInClub(Club club)
		{
			if (club == null)
			{
				return ClubRoles.None;
			}
			if (club.Members == null)
			{
				return ClubRoles.None;
			}
			if (club.Members.ToList().Find((Member m) => m.PlayerId == GetPlayerId() && m.Role == 1) != null)
			{
				return ClubRoles.Chairman;
			}
			if (club.Members.ToList().Find((Member m) => m.PlayerId == GetPlayerId() && m.Role == 3) != null)
			{
				return ClubRoles.Member;
			}
			if (club.Members.ToList().Find((Member m) => m.PlayerId == GetPlayerId() && m.Role == 2) != null)
			{
				return ClubRoles.ViceChairman;
			}
			return ClubRoles.None;
		}

		public ClubRoles GetRoleInClub()
		{
			if (_MyClubResponse == null)
			{
				return ClubRoles.None;
			}
			if (_MyClubResponse.Club == null)
			{
				return ClubRoles.None;
			}
			return GetRoleInClub(_MyClubResponse.Club);
		}

		public int GetViceChairmanNumbers()
		{
			if (_MyClubResponse == null)
			{
				return 0;
			}
			if (_MyClubResponse.Club == null)
			{
				return 0;
			}
			return _MyClubResponse.Club.Members.ToList().Count((Member f) => f.Role == 2);
		}

		public bool IsContains(Member member)
		{
			if (_MyClubResponse == null)
			{
				return false;
			}
			if (_MyClubResponse.Club == null)
			{
				return false;
			}
			if (_MyClubResponse.Club.Members == null)
			{
				return false;
			}
			return _MyClubResponse.Club.Members.Contains(member);
		}

		public bool IsInviteMemberEnable(Member member)
		{
			if (_MyClubResponse == null)
			{
				return false;
			}
			if (_MyClubResponse.Club == null)
			{
				return false;
			}
			if (_MyClubResponse.Club.Members == null)
			{
				return false;
			}
			if (_MyClubResponse.Club.MemberCount >= _MyClubResponse.Club.MemberLimit)
			{
				return _MyClubResponse.Club.Private;
			}
			return _MyClubResponse.Club.Members.ToList().Find((Member e) => e.PlayerId == member.PlayerId) == null;
		}

		public List<NewsConfig> GetNewsConfigs()
		{
			List<NewsConfig> list = new List<NewsConfig>();
			MyClubResponse response = _MyClubResponse;
			if (response == null)
			{
				return new List<NewsConfig>();
			}
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			if (IsActive())
			{
				string identifier = GetClubIdentifier();
				if (response.Invitations != null)
				{
					foreach (ClubInvitation invitation in response.Invitations)
					{
						foreach (Invitation inviter in invitation.Invitations)
						{
							list.Add(new NewsConfig
							{
								icon = GetClubAvatar(invitation.ClubIcon),
								description = string.Format(LocalizationUtility.Get("Localization_Club.json").GetString("player1_invitation_to_club"), inviter.Inviter.PlayerName, invitation.ClubName),
								title = localizationUtility.GetString("invitation_to_club_title"),
								Tag = "Club_Invitation",
								buttons = new List<ButtonLabel>
								{
									new ButtonLabel(localizationUtility.GetString("Reject"), "ui/Inboxs/inbox_redButton"),
									new ButtonLabel(localizationUtility.GetString("Accept")),
									new ButtonLabel("i", "ui/Inboxs/inbox_infoButton")
								},
								RunAction = delegate(InboxNewsUI ui, int ndx)
								{
									switch (ndx)
									{
									case 0:
										ui.DestroyUI();
										invitation.Invitations.Remove(inviter);
										SingletonClass<InboxUtility>.Get().UpdateNumber();
										Reject(inviter.Inviter.PlayerId, invitation.ClubId);
										break;
									case 1:
										ui.DestroyUI();
										invitation.Invitations.Remove(inviter);
										SingletonClass<InboxUtility>.Get().UpdateNumber();
										Accept(inviter.Inviter.PlayerId, invitation.ClubId, invitation.ClubName);
										UnityEngine.Object.FindObjectsOfType<InboxNewsUI>().ForEach(delegate(InboxNewsUI inboxNewsUI)
										{
											if ("Club_Invitation".Equals(inboxNewsUI.Config.Tag))
											{
												inboxNewsUI.DestroyUI();
											}
										});
										break;
									default:
										UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene e)
										{
											e.IsStay = false;
										});
										SingletonClass<MySceneManager>.Get().Popup<ClubScene>("Scenes/ClubScene").OnStart(new Club
										{
											ClubId = invitation.ClubId,
											ClubIcon = invitation.ClubIcon,
											ClubName = invitation.ClubName
										});
										break;
									}
								}
							});
						}
					}
				}
				if (response.ClubOperationsToMe != null && response.ClubOperationsToMe.Count > 0)
				{
					foreach (TriPeaks.ProtoData.Club.ClubOperation clubOperation in response.ClubOperationsToMe)
					{
						switch (clubOperation.Operation)
						{
						case 2:
							list.Add(new NewsConfig
							{
								title = localizationUtility.GetString("removed_from_club_title"),
								description = string.Format(localizationUtility.GetString("removed_from_club"), clubOperation.ClubName),
								Tag = "Club_Normal",
								icon = GetClubAvatar(clubOperation.ClubIcon),
								buttons = new List<ButtonLabel>
								{
									new ButtonLabel(localizationUtility.GetString("btn_ok"))
								},
								RunAction = delegate(InboxNewsUI ui, int dex)
								{
									ReadOperation(clubOperation.ClubId, clubOperation.OperationId, isOperationToMe: true);
									response.ClubOperationsToMe.Remove(clubOperation);
									SingletonClass<InboxUtility>.Get().UpdateNumber();
									ui.DestroyUI();
								}
							});
							break;
						case 4:
							ClubSystemData.Get().RemoveRequest(clubOperation.ClubId);
							list.Add(new NewsConfig
							{
								title = localizationUtility.GetString("disapprove_join_club_title"),
								description = string.Format(localizationUtility.GetString("disapprove_join_club_des"), clubOperation.ClubName),
								icon = GetClubAvatar(clubOperation.ClubIcon),
								Tag = "Club_Normal",
								buttons = new List<ButtonLabel>
								{
									new ButtonLabel(localizationUtility.GetString("btn_ok"))
								},
								RunAction = delegate(InboxNewsUI ui, int dex)
								{
									ReadOperation(clubOperation.ClubId, clubOperation.OperationId, isOperationToMe: true);
									response.ClubOperationsToMe.Remove(clubOperation);
									SingletonClass<InboxUtility>.Get().UpdateNumber();
									ui.DestroyUI();
								}
							});
							break;
						}
					}
				}
				if (response.Club == null)
				{
					foreach (RequestClubData item in ClubSystemData.Get().GetRequestClub())
					{
						RequestClubData request = item;
						list.Add(new NewsConfig
						{
							description = string.Format(localizationUtility.GetString("club_Requesting"), request.clubName),
							icon = GetClubAvatar(request.clubIcon),
							Tag = "Club_Normal",
							buttons = new List<ButtonLabel>
							{
								new ButtonLabel(localizationUtility.GetString("btn_ok"))
							},
							RunAction = delegate(InboxNewsUI ui, int dex)
							{
								ClubSystemData.Get().RemoveRequest(request);
								ui.DestroyUI();
								SingletonClass<InboxUtility>.Get().UpdateNumber();
							}
						});
					}
				}
				if (response.TemporaryMembers != null)
				{
					ProtoList<Member> temporaryMembers = response.TemporaryMembers;
					foreach (Member temporaryMember in response.TemporaryMembers)
					{
						list.Add(new NewsConfig
						{
							description = string.Format(localizationUtility.GetString("player_request_to_club"), temporaryMember.PlayerName),
							icon = GetClubAvatar(response.Club),
							Tag = "Club_Normal",
							buttons = new List<ButtonLabel>
							{
								new ButtonLabel(localizationUtility.GetString("Reject"), "ui/Inboxs/inbox_redButton"),
								new ButtonLabel(localizationUtility.GetString("Accept")),
								new ButtonLabel("i", "ui/Inboxs/inbox_infoButton")
							},
							RunAction = delegate(InboxNewsUI ui, int dex)
							{
								if (dex == 2)
								{
									SingletonClass<MySceneManager>.Get().Popup<ClubPlayerScene>("Scenes/ClubPlayerScene", new ScaleEffect()).OnStart(temporaryMember);
								}
								else
								{
									Approve(temporaryMember.PlayerId, dex == 1, temporaryMember.PlayerName);
									response.TemporaryMembers.Remove(temporaryMember);
									ui.DestroyUI();
									SingletonClass<InboxUtility>.Get().UpdateNumber();
								}
							}
						});
					}
				}
				ClubSystemData.Get().ClearGifts((response.Gifts != null) ? (from g in response.Gifts.ToList()
					select g.Guid).ToList() : null);
				if (response.Gifts != null)
				{
					foreach (Gift gift in response.Gifts.ToList())
					{
						if (ClubSystemData.Get().ContainsGift(gift.Guid))
						{
							UnityEngine.Debug.Log("虽然有礼物但是我认为该礼物我已经处理过了。");
							AcceptGift(gift.Guid, identifier);
							response.Gifts.Remove(gift);
						}
						else
						{
							bool isMe = gift.Giver.PlayerId.Equals(GetPlayerId());
							list.Add(new NewsConfig
							{
								icon = GetClubAvatar(response.Club),
								description = string.Format(localizationUtility.GetString("club_gift_to_you"), gift.Giver.PlayerName, ClubStoreConfig.Get().GetTitleByGiftId(gift.GiftId)),
								title = localizationUtility.GetString("club_gift_to_you_title"),
								Tag = "Club_Gift",
								buttons = new List<ButtonLabel>
								{
									new ButtonLabel(localizationUtility.GetString("Accept"))
								},
								RunAction = delegate(InboxNewsUI ui, int ndx)
								{
									response.Gifts.Remove(gift);
									ClubSystemData.Get().AppendGift(gift.Guid);
									AcceptGift(gift.Guid, identifier);
									PurchasingCommodity[] commoditys = GetCommoditys(gift.GiftId);
									PurchasingCommodity[] array2 = commoditys;
									foreach (PurchasingCommodity purchasingCommodity2 in array2)
									{
										SessionData.Get().PutCommodity(purchasingCommodity2.boosterType, CommoditySource.Free, purchasingCommodity2.count, changed: false);
									}
									if (isMe)
									{
										PurchasSuccessPopup.ShowPurchasSuccessPopup(commoditys);
									}
									else
									{
										SingletonClass<MySceneManager>.Get().Popup<PurchasSuccessPopup>("Scenes/ClubGiftScene").OnStart(commoditys, delegate
										{
											ThanksForGift(gift.Guid, gift.GiftId, identifier, gift.Giver.PlayerId);
										});
									}
									ui.DestroyUI();
									SingletonClass<InboxUtility>.Get().UpdateNumber();
								}
							});
						}
					}
				}
				ClubSystemData.Get().ClearThanks((response.Thanks != null) ? (from g in response.Thanks.ToList()
					select g.Guid).ToList() : null);
				if (response.Thanks != null)
				{
					foreach (Thanks thank in response.Thanks.ToList())
					{
						if (ClubSystemData.Get().ContainsThanks(thank.Guid))
						{
							ReadThanks(thank.Guid, identifier);
							response.Thanks.Remove(thank);
						}
						else
						{
							list.Add(new NewsConfig
							{
								icon = GetClubAvatar(response.Club),
								description = string.Format(localizationUtility.GetString("club_thanks_from_player"), thank.Sender.PlayerName),
								title = localizationUtility.GetString("club_thanks_title"),
								Tag = "Club_Thanks",
								buttons = new List<ButtonLabel>
								{
									new ButtonLabel(localizationUtility.GetString("Accept"))
								},
								RunAction = delegate(InboxNewsUI ui, int ndx)
								{
									response.Thanks.Remove(thank);
									ClubSystemData.Get().AppendThanks(thank.Guid);
									ReadThanks(identifier, thank.Guid);
									PurchasingCommodity[] commoditysByGiftId = ClubStoreConfig.Get().GetCommoditysByGiftId(thank.GiftId);
									PurchasingCommodity[] array = commoditysByGiftId;
									foreach (PurchasingCommodity purchasingCommodity in array)
									{
										SessionData.Get().PutCommodity(purchasingCommodity.boosterType, CommoditySource.Free, purchasingCommodity.count, changed: false);
									}
									PurchasSuccessPopup.ShowPurchasSuccessPopup(commoditysByGiftId);
									ui.DestroyUI();
									SingletonClass<InboxUtility>.Get().UpdateNumber();
								}
							});
						}
					}
				}
			}
			localizationUtility = LocalizationUtility.Get("Localization_inbox.json");
			if (response.Club == null)
			{
				if (IsActive() && AuxiliaryData.Get().IsDailyActive("InboxJoinClub"))
				{
					list.Add(new NewsConfig
					{
						icon = GetClubAvatar(response.Club),
						prefabs = new List<PrefabLabel>
						{
							new PrefabLabel(new Vector3(462f, 46f), "UI/Inboxs/Treasure")
						},
						title = localizationUtility.GetString("Inbox_join_club_title"),
						description = localizationUtility.GetString("Inbox_join_club_description"),
						buttons = new List<ButtonLabel>
						{
							new ButtonLabel(localizationUtility.GetString("btn_NextTime"), "ui/Inboxs/inbox_redButton"),
							new ButtonLabel(localizationUtility.GetString("btn_Join"))
						},
						RunAction = delegate(InboxNewsUI ui, int dex)
						{
							ui.DestroyUI();
							if (dex == 0)
							{
								AuxiliaryData.Get().PutDailyCompleted("InboxJoinClub");
							}
							else
							{
								ShowClubScene();
							}
							SingletonClass<InboxUtility>.Get().UpdateNumber();
						}
					});
				}
			}
			else if (SingletonBehaviour<LeaderBoardUtility>.Get().IsUploadEnable && SystemTime.IsConnect)
			{
				if (AuxiliaryData.Get().IsDailyActive("InboxClubDailyBonus"))
				{
					list.Add(new NewsConfig
					{
						icon = GetClubAvatar(response.Club),
						prefabs = new List<PrefabLabel>
						{
							new PrefabLabel(new Vector3(462f, 46f), "UI/Inboxs/Treasure")
						},
						title = localizationUtility.GetString("Inbox_club_daily_bonus_title"),
						description = localizationUtility.GetString("Inbox_club_daily_bonus_description"),
						buttons = new List<ButtonLabel>
						{
							new ButtonLabel(localizationUtility.GetString("btn_collect"))
						},
						RunAction = delegate(InboxNewsUI ui, int dex)
						{
							AuxiliaryData.Get().PutDailyCompleted("InboxClubDailyBonus");
							ui.Config.buttons = null;
							ui.Config.prefabs = new List<PrefabLabel>
							{
								new PrefabLabel(new Vector3(446.3f, 0f), "UI/Inboxs/Collected")
							};
							ui.UpdateUI();
							List<PurchasingCommodity> clubBonus = ClubBonusConfig.Get().GetClubBonus(response.Club.Level);
							foreach (PurchasingCommodity item2 in clubBonus)
							{
								SessionData.Get().PutCommodity(item2.boosterType, CommoditySource.Free, item2.count, changed: false);
							}
							PurchasSuccessPopup.ShowPurchasSuccessPopup(clubBonus.ToArray());
							SingletonClass<InboxUtility>.Get().UpdateNumber();
						}
					});
				}
				else
				{
					list.Add(new NewsConfig
					{
						icon = GetClubAvatar(response.Club),
						prefabs = new List<PrefabLabel>
						{
							new PrefabLabel(new Vector3(446.3f, 0f), "UI/Inboxs/Collected")
						},
						title = localizationUtility.GetString("Inbox_club_daily_bonus_title"),
						description = localizationUtility.GetString("Inbox_club_daily_bonus_description"),
						HidingInNumber = true
					});
				}
			}
			return list;
		}

		public PurchasingCommodity[] GetCommoditys(string content)
		{
			List<PurchasingCommodity> list = new List<PurchasingCommodity>();
			string[] array = content.Split('|');
			foreach (string text in array)
			{
				try
				{
					string[] array2 = text.Split('_');
					list.Add(new PurchasingCommodity
					{
						boosterType = EnumUtility.GetEnumType(array2[0], BoosterType.RandomBooster),
						count = int.Parse(array2[1])
					});
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log(ex.Message);
				}
			}
			return list.ToArray();
		}

		public string GetGiftId(List<PurchasingCommodity> commoditys)
		{
			return string.Join("|", (from c in commoditys
				select $"{c.boosterType.ToString()}_{c.count}").ToArray());
		}

		public void Contribute(int score)
		{
			string clubIdentifier = GetClubIdentifier();
			if (!string.IsNullOrEmpty(clubIdentifier))
			{
				ClubSystemData.Get().AppendScore(clubIdentifier, score);
				Contribute(clubIdentifier, ClubSystemData.Get().Score, ClubSystemData.Get().PlayNumber);
			}
		}

		public void GetLatestClubMessage()
		{
			List<Message> messages = GetMessages();
			if (messages.Count == 0)
			{
				GetLatestClubMessage(GetClubIdentifier(), 0L);
				return;
			}
			long latestTicks = 0L;
			try
			{
				List<Message> list = (from e in messages
					where e.Author != null
					where !string.IsNullOrEmpty(e.Template) || (!GetPlayerId().Equals(e.Author.PlayerId) && string.IsNullOrEmpty(e.Template))
					select e).ToList();
				if (list.Count > 0)
				{
					latestTicks = list.Max((Message e) => e.Ticks);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			GetLatestClubMessage(GetClubIdentifier(), latestTicks);
		}

		public void GetHistoryClubMessage()
		{
			List<Message> messages = GetMessages();
			if (messages.Count == 0)
			{
				GetLatestClubMessage(GetClubIdentifier(), 0L);
			}
			else
			{
				GetHistoryClubMessage(GetClubIdentifier(), messages.Min((Message e) => e.Ticks));
			}
		}

		public List<Message> GetMessages()
		{
			string clubIdentifier = GetClubIdentifier();
			if (MessagesGroup.ContainsKey(clubIdentifier))
			{
				return (from e in MessagesGroup[clubIdentifier]
					orderby e.Ticks
					select e).ToList();
			}
			return new List<Message>();
		}

		public void AppendMessages(List<Message> messages, bool newMessage)
		{
			string identifier = GetClubIdentifier();
			if (!MessagesGroup.ContainsKey(identifier))
			{
				MessagesGroup.Add(identifier, messages);
			}
			else
			{
				messages.RemoveAll((Message e) => MessagesGroup[identifier].Find((Message m) => !string.IsNullOrEmpty(m.MessageId) && e.MessageId == m.MessageId) != null);
				messages.RemoveAll((Message e) => MessagesGroup[identifier].Find((Message m) => m.Ticks == e.Ticks) != null);
				if (messages.Count == 0)
				{
					return;
				}
				MessagesGroup[identifier].AddRange(messages);
			}
			if (MessagesGroup.ContainsKey(identifier))
			{
				MessagesGroup[identifier].ForEach(delegate(Message e)
				{
					Message message = MessagesGroup[identifier].Find((Message p) => e.Author.PlayerId == p.Author.PlayerId);
					if (message.Author.PlayerName != e.Author.PlayerName || message.Author.Avatar != e.Author.Avatar)
					{
						e.Author = message.Author;
					}
				});
			}
			_MessageGroupEventEvent.Invoke(newMessage, GetMessages());
		}

		public Sprite GetClubAvatar(Club club)
		{
			if (club == null)
			{
				return SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("ClubAvatar/000");
			}
			Sprite asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>($"ClubAvatar/{club.ClubIcon}");
			if (asset == null)
			{
				asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("ClubAvatar/000");
			}
			return asset;
		}

		public Sprite GetClubAvatar(string icon)
		{
			Sprite asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>($"ClubAvatar/{icon}");
			if (asset == null)
			{
				asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("ClubAvatar/000");
			}
			return asset;
		}

		public int GetMessageIndex(Message message)
		{
			return GetMessages().IndexOf(message);
		}

		public void SendGift(int count)
		{
			SendGift(new GiftData
			{
				giftId = ClubStoreConfig.Get().GetConfig(count).GetGiftId(),
				clubId = GetClubIdentifier(),
				guid = Guid.NewGuid().ToString("D")
			});
		}

		public string GetPlayerId()
		{
			return SolitaireTripeaksData.Get().GetPlayerId();
		}

		public List<PurchasingCommodity> GetCommoditys(int segment, int position, List<RewardItem> rewardTable)
		{
			return (from e in rewardTable
				where e.Stage == segment
				where e.MinPosition <= position && e.MaxPosition >= position
				select new PurchasingCommodity
				{
					count = e.Amount,
					boosterType = EnumUtility.GetEnumType(e.ItemId, BoosterType.Coins)
				}).ToList();
		}

		public int GetDailyLimit()
		{
			if (_MyClubResponse == null)
			{
				return 5;
			}
			if (_MyClubResponse.Club == null)
			{
				return 5;
			}
			return ClubBonusConfig.Get().GetDailyLimit(_MyClubResponse.Club.Level) + 5;
		}

		public float GetSkillRatioCoins(ClubSkill skill)
		{
			if (_MyClubResponse == null)
			{
				return 0f;
			}
			if (_MyClubResponse.Club == null)
			{
				return 0f;
			}
			if (AuxiliaryData.Get().GetDailyNumber($"ClubSkillCoins_{skill}") < 3)
			{
				return ClubBonusConfig.Get().GetSkillRatioCoins(skill, _MyClubResponse.Club.Level);
			}
			return 0f;
		}

		public void ClubSkillOnce(ClubSkill skill)
		{
			if (SingletonBehaviour<ClubSystemHelper>.Get().GetSkillRatioCoins(ClubSkill.Hand) > 0f)
			{
				AuxiliaryData.Get().PutDailyOnce($"ClubSkillCoins_{skill}");
			}
		}

		public int LevelCompletedCoins(int coins)
		{
			if (_MyClubResponse == null)
			{
				return 0;
			}
			if (_MyClubResponse.Club == null)
			{
				return 0;
			}
			if (AuxiliaryData.Get().GetDailyNumber("ClanLevelCompletedCoins") < 3)
			{
				float skillRatioCoins = ClubBonusConfig.Get().GetSkillRatioCoins(ClubSkill.LevelCompleted, _MyClubResponse.Club.Level);
				if (skillRatioCoins > 0f)
				{
					AuxiliaryData.Get().PutDailyOnce("ClanLevelCompletedCoins");
					return Mathf.CeilToInt((float)coins * skillRatioCoins);
				}
			}
			return 0;
		}

		public SuperTreasure HasSuperTreasure()
		{
			if (_MyClubResponse == null)
			{
				return SuperTreasure.Lock;
			}
			if (_MyClubResponse.Club == null)
			{
				return SuperTreasure.Lock;
			}
			if (SystemTime.Now.DayOfWeek == DayOfWeek.Saturday && AuxiliaryData.Get().IsDailyActive("Clan_SuperTreasure"))
			{
				if (ClubBonusConfig.Get().HasSuperTreasure(_MyClubResponse.Club.Level))
				{
					return SuperTreasure.Normal;
				}
				return SuperTreasure.Lock;
			}
			return SuperTreasure.Collected;
		}

		public TimeSpan GetSuperRemain()
		{
			int num = (int)(6 - SystemTime.Now.DayOfWeek);
			if (num <= 0)
			{
				num += 7;
			}
			return SystemTime.Now.Date.AddDays(num).Subtract(SystemTime.Now);
		}

		public int ClubLeaderboardScore()
		{
			if (_MyClubResponse == null)
			{
				return 0;
			}
			if (_MyClubResponse.Club == null)
			{
				return 0;
			}
			return _MyClubResponse.Club.Members.ToList().Find((Member e) => e.PlayerId == SolitaireTripeaksData.Get().GetPlayerId())?.LeaderboardScore ?? 0;
		}

		public void CollectSuperTreasure()
		{
			AuxiliaryData.Get().PutDailyCompleted("Clan_SuperTreasure");
			List<PurchasingCommodity> superTreasure = ClubBonusConfig.Get().GetSuperTreasure();
			for (int i = 0; i < superTreasure.Count; i++)
			{
				PurchasingCommodity commodity = superTreasure[i];
				DelayDo(new WaitForSeconds(0.5f * (float)i), delegate
				{
					if (commodity.boosterType == BoosterType.Wild || commodity.boosterType == BoosterType.Rocket || commodity.boosterType == BoosterType.UnlimitedDoubleStar || (commodity.boosterType == BoosterType.ExpiredPlay && commodity.count > 4) || (commodity.boosterType == BoosterType.RandomBooster && commodity.count > 4))
					{
						SendOpenGoldenBoxMessage(GetClubIdentifier(), $"{commodity.boosterType}_{commodity.count}");
					}
				});
			}
			foreach (PurchasingCommodity item in superTreasure)
			{
				SessionData.Get().PutCommodity(item.boosterType, CommoditySource.None, item.count, changed: false);
			}
			PurchasSuccessPopup.ShowPurchasSuccessPopup(superTreasure.ToArray());
		}

		public void SendChatMessage(string message, string clubId)
		{
			Message message2 = new Message();
			message2.Author = new Member
			{
				Avatar = AuxiliaryData.Get().AvaterFileName,
				PlayerName = AuxiliaryData.Get().GetNickName(),
				SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId,
				SocialPlatform = 1,
				PlayerId = GetPlayerId()
			};
			message2.Content = SingletonClass<SensitiveWords>.Get().ProfanityFilter(message);
			message2.Ticks = DateTime.UtcNow.Ticks;
			message2.MessageId = Guid.NewGuid().ToString();
			Message message3 = message2;
			SendMessageToClubRequest sendMessageToClubRequest = new SendMessageToClubRequest();
			sendMessageToClubRequest.RequestId = RequestId;
			sendMessageToClubRequest.Cmd = 416;
			sendMessageToClubRequest.Platform = Application.platform.ToString();
			sendMessageToClubRequest.AppVersion = Application.version;
			sendMessageToClubRequest.Channel = "Official";
			sendMessageToClubRequest.Group = "Normal";
			sendMessageToClubRequest.PlayerId = message3.Author.PlayerId;
			sendMessageToClubRequest.ClubId = clubId;
			sendMessageToClubRequest.PlayerName = message3.Author.PlayerName;
			sendMessageToClubRequest.Avatar = message3.Author.Avatar;
			sendMessageToClubRequest.SocialId = message3.Author.SocialId;
			sendMessageToClubRequest.SocialPlatform = message3.Author.SocialPlatform;
			sendMessageToClubRequest.Content = message3.Content;
			sendMessageToClubRequest.MessageId = message3.MessageId;
			SendMessageToClubRequest obj = sendMessageToClubRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------发送俱乐部消息：" + unityWebRequest.url);
			AppendMessages(new List<Message>
			{
				message3
			}, newMessage: true);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"发送俱乐部消息成功");
						}
						else
						{
							UnityEngine.Debug.Log($"发送俱乐部消息失败, 代码：{baseResponse.ErrorCode}");
							if (baseResponse.ErrorCode == 10006)
							{
								ShowNotInClub();
							}
							else
							{
								ShowOperationFaild();
							}
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void SendOpenGoldenBoxMessage(string clubId, string content)
		{
			OpenGoldenBoxRequest openGoldenBoxRequest = new OpenGoldenBoxRequest();
			openGoldenBoxRequest.RequestId = RequestId;
			openGoldenBoxRequest.Cmd = 425;
			openGoldenBoxRequest.Platform = Application.platform.ToString();
			openGoldenBoxRequest.AppVersion = Application.version;
			openGoldenBoxRequest.Channel = "Official";
			openGoldenBoxRequest.Group = "Normal";
			openGoldenBoxRequest.PlayerId = GetPlayerId();
			openGoldenBoxRequest.ClubId = clubId;
			openGoldenBoxRequest.Content = content;
			OpenGoldenBoxRequest obj = openGoldenBoxRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------上传打开俱乐部宝箱：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				BaseResponse baseResponse = null;
				if (download.isDone)
				{
					baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
				}
				if (baseResponse != null && baseResponse.ErrorCode == 1)
				{
					UnityEngine.Debug.Log("俱乐部宝箱上传成功。");
				}
				else
				{
					UnityEngine.Debug.Log("俱乐部宝箱上传失败。");
					SingletonBehaviour<StepUtility>.Get().Append("SendOpenGoldenBoxMessage", 10f, delegate
					{
						SendOpenGoldenBoxMessage(clubId, content);
					});
				}
			}));
		}

		public void GetLatestClubMessage(string clubId, long latestTicks = 0L)
		{
			if (!ContainsTaskId("LatestClubMessage"))
			{
				AppendTaskId("LatestClubMessage");
				GetLatestClubMessageRequest getLatestClubMessageRequest = new GetLatestClubMessageRequest();
				getLatestClubMessageRequest.RequestId = RequestId;
				getLatestClubMessageRequest.Cmd = 417;
				getLatestClubMessageRequest.Platform = Application.platform.ToString();
				getLatestClubMessageRequest.AppVersion = Application.version;
				getLatestClubMessageRequest.Channel = "Official";
				getLatestClubMessageRequest.Group = "Normal";
				getLatestClubMessageRequest.PlayerId = GetPlayerId();
				getLatestClubMessageRequest.ClubId = clubId;
				getLatestClubMessageRequest.LatestTicks = latestTicks;
				GetLatestClubMessageRequest obj = getLatestClubMessageRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取最新俱乐部消息：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("LatestClubMessage");
					if (download.isDone)
					{
						GetMessageResponse getMessageResponse = ProtoDataUtility.Deserialize<GetMessageResponse>(download.data);
						if (getMessageResponse != null)
						{
							if (getMessageResponse.ErrorCode == 1)
							{
								if (getMessageResponse.Messages.Count > 0)
								{
									AppendMessages(getMessageResponse.Messages.ToList(), newMessage: true);
								}
								UnityEngine.Debug.Log($"获取最新俱乐部消息成功, MessageCount={((getMessageResponse.Messages != null) ? getMessageResponse.Messages.Count : 0)}");
							}
							else
							{
								UnityEngine.Debug.Log($"获取最新俱乐部消息失败, 代码：{getMessageResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void GetHistoryClubMessage(string clubId, long earliestTicks)
		{
			if (!ContainsTaskId("HistoryClubMessage"))
			{
				AppendTaskId("HistoryClubMessage");
				GetHistoryClubMessageRequest getHistoryClubMessageRequest = new GetHistoryClubMessageRequest();
				getHistoryClubMessageRequest.RequestId = RequestId;
				getHistoryClubMessageRequest.Cmd = 418;
				getHistoryClubMessageRequest.Platform = Application.platform.ToString();
				getHistoryClubMessageRequest.AppVersion = Application.version;
				getHistoryClubMessageRequest.Channel = "Official";
				getHistoryClubMessageRequest.Group = "Normal";
				getHistoryClubMessageRequest.PlayerId = GetPlayerId();
				getHistoryClubMessageRequest.ClubId = clubId;
				getHistoryClubMessageRequest.EarliestTicks = earliestTicks;
				GetHistoryClubMessageRequest obj = getHistoryClubMessageRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取历史俱乐部消息：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("HistoryClubMessage");
					if (download.isDone)
					{
						GetMessageResponse getMessageResponse = ProtoDataUtility.Deserialize<GetMessageResponse>(download.data);
						if (getMessageResponse != null)
						{
							if (getMessageResponse.ErrorCode == 1)
							{
								UnityEngine.Debug.Log($"获取历史俱乐部消息成功, MessageCount={getMessageResponse.Messages.Count}");
								AppendMessages(getMessageResponse.Messages.ToList(), newMessage: false);
							}
							else
							{
								UnityEngine.Debug.Log($"获取历史俱乐部消息失败, 代码：{getMessageResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void SearchClub(string filter)
		{
			if (!ContainsTaskId("SearchClub"))
			{
				AppendTaskId("SearchClub");
				SearchClubRequest searchClubRequest = new SearchClubRequest();
				searchClubRequest.RequestId = RequestId;
				searchClubRequest.Cmd = 426;
				searchClubRequest.Platform = Application.platform.ToString();
				searchClubRequest.AppVersion = Application.version;
				searchClubRequest.Channel = "Official";
				searchClubRequest.Group = "Normal";
				searchClubRequest.PlayerId = GetPlayerId();
				searchClubRequest.ClubNameTerms = filter;
				SearchClubRequest obj = searchClubRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------搜索俱乐部：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("SearchClub");
					if (download.isDone)
					{
						SearchClubResponse searchClubResponse = ProtoDataUtility.Deserialize<SearchClubResponse>(download.data);
						if (searchClubResponse != null)
						{
							if (searchClubResponse.ErrorCode == 1)
							{
								_SearchClubGroupEvent.Invoke(1, searchClubResponse.Clubs.ToList());
								UnityEngine.Debug.Log($"搜索俱乐部成功, ClubCount: {searchClubResponse.Clubs.Count}");
							}
							else
							{
								UnityEngine.Debug.Log($"搜索俱乐部失败, 代码：{searchClubResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void ClubPageList(int page = 1)
		{
			string taskId = string.Format("ClubPageList", page);
			if (!ContainsTaskId(taskId))
			{
				AppendTaskId(taskId);
				RanksRequest ranksRequest = new RanksRequest();
				ranksRequest.RequestId = RequestId;
				ranksRequest.Cmd = 428;
				ranksRequest.Platform = Application.platform.ToString();
				ranksRequest.AppVersion = Application.version;
				ranksRequest.Channel = "Official";
				ranksRequest.Group = "Normal";
				ranksRequest.PlayerId = GetPlayerId();
				ranksRequest.Page = page;
				RanksRequest obj = ranksRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取俱乐部排行：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId(taskId);
					if (download.isDone)
					{
						RanksResponse ranksResponse = ProtoDataUtility.Deserialize<RanksResponse>(download.data);
						if (ranksResponse != null)
						{
							if (ranksResponse.ErrorCode == 1)
							{
								_PageClubGroupEvent.Invoke(page, ranksResponse.Clubs.ToList());
								UnityEngine.Debug.Log($"获取俱乐部排行成功, Page: {ranksResponse.Page}, PageSize: {ranksResponse.PageSize}, ClubCount: {ranksResponse.Clubs.Count}");
							}
							else
							{
								_PageClubGroupEvent.Invoke(page, new List<Club>());
								UnityEngine.Debug.Log($"获取俱乐部排行失败, 代码：{ranksResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void CreatorClub(Club clubData, UnityAction<bool> unityAction)
		{
			LoadingHelper.Get("CreatorClub").ShowLoading(delegate(LoadingHelper helper, float total)
			{
				if (total > 15f)
				{
					helper.StopLoading();
				}
			}, delegate
			{
			}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_Creator_Clubing"));
			CreateRequest createRequest = new CreateRequest();
			createRequest.RequestId = RequestId;
			createRequest.Cmd = 401;
			createRequest.Platform = Application.platform.ToString();
			createRequest.AppVersion = Application.version;
			createRequest.Channel = "Official";
			createRequest.Group = "Normal";
			createRequest.PlayerId = GetPlayerId();
			createRequest.ClubName = clubData.ClubName;
			createRequest.ClubIcon = clubData.ClubIcon;
			createRequest.ClubDescription = clubData.ClubDescription;
			createRequest.Private = clubData.Private;
			createRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			createRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			createRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			createRequest.SocialPlatform = 1;
			createRequest.MaxLevel = PlayData.Get().GetMax();
			createRequest.MaxMasterLevel = PlayData.Get().GetMaxMasterLevels();
			createRequest.StarCount = PlayData.Get().GetStars();
			CreateRequest obj = createRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------创建俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				LoadingHelper.Get("CreatorClub").StopLoading();
				if (download.isDone)
				{
					CreateResponse createResponse = ProtoDataUtility.Deserialize<CreateResponse>(download.data);
					if (createResponse != null)
					{
						if (createResponse.ErrorCode == 1)
						{
							clubData.ClubId = createResponse.ClubId;
							if (_MyClubResponse != null)
							{
								_MyClubResponse.Club = clubData;
							}
							if (unityAction != null)
							{
								unityAction(arg0: true);
							}
							UnityEngine.Debug.Log($"创建俱乐部成功, ClubId: {createResponse.ClubId}");
						}
						else
						{
							UnityEngine.Debug.Log($"创建俱乐部失败, 代码：{createResponse.ErrorCode}");
							if (unityAction != null)
							{
								unityAction(arg0: false);
							}
							if (createResponse.ErrorCode == 10003)
							{
								ShowInClub();
							}
						}
					}
				}
				else if (unityAction != null)
				{
					unityAction(arg0: false);
				}
			}));
		}

		public void ModifyClub(Club clubData, UnityAction<bool> unityAction)
		{
			LoadingHelper.Get("ModifyClub").ShowLoading(delegate(LoadingHelper helper, float total)
			{
				if (total > 15f)
				{
					helper.StopLoading();
				}
			}, delegate
			{
			}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_ModifyClub_Clubing"));
			ModifyRequest modifyRequest = new ModifyRequest();
			modifyRequest.RequestId = RequestId;
			modifyRequest.Cmd = 402;
			modifyRequest.Platform = Application.platform.ToString();
			modifyRequest.AppVersion = Application.version;
			modifyRequest.Channel = "Official";
			modifyRequest.Group = "Normal";
			modifyRequest.PlayerId = GetPlayerId();
			modifyRequest.ClubIcon = clubData.ClubIcon;
			modifyRequest.ClubDescription = clubData.ClubDescription;
			modifyRequest.Private = clubData.Private;
			ModifyRequest obj = modifyRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------修改俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				LoadingHelper.Get("ModifyClub").StopLoading();
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"修改俱乐部成功");
							if (_MyClubResponse != null)
							{
								_MyClubResponseEvent.Invoke(_MyClubResponse);
							}
							if (unityAction != null)
							{
								unityAction(arg0: true);
							}
						}
						else
						{
							UnityEngine.Debug.Log($"修改俱乐部失败, 代码：{baseResponse.ErrorCode}");
							if (unityAction != null)
							{
								unityAction(arg0: false);
							}
						}
					}
				}
				else if (unityAction != null)
				{
					unityAction(arg0: false);
				}
			}));
		}

		public void InviteJoinClub(List<TripeaksPlayerInView> users)
		{
			bool notInJoinClub = false;
			int index = 0;
			LoadingHelper.Get("InviteJoinClub").ShowLoading(delegate(LoadingHelper helper, float total)
			{
				if (total > 30f)
				{
					helper.StopLoading();
				}
				else if (index <= 0)
				{
					ShowInviteJoinClubCompleted();
					helper.StopLoading();
				}
			}, delegate(LoadingHelper helper, float total)
			{
				helper.StopLoading();
			}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_Invite_Join_Club"));
			foreach (TripeaksPlayerInView user in users)
			{
				index++;
				InviteRequest inviteRequest = new InviteRequest();
				inviteRequest.RequestId = RequestId;
				inviteRequest.Cmd = 404;
				inviteRequest.Platform = Application.platform.ToString();
				inviteRequest.AppVersion = Application.version;
				inviteRequest.Channel = "Official";
				inviteRequest.Group = "Normal";
				inviteRequest.PlayerId = GetPlayerId();
				inviteRequest.TargetId = user.player.id;
				InviteRequest obj = inviteRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------邀请玩家加入自己俱乐部：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					if (download.isDone)
					{
						BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
						if (baseResponse == null)
						{
							return;
						}
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"邀请玩家成功");
						}
						else
						{
							UnityEngine.Debug.Log($"邀请玩家失败, 代码：{baseResponse.ErrorCode}");
							if (baseResponse.ErrorCode == 10006 && !notInJoinClub)
							{
								notInJoinClub = true;
								ShowNotInClub();
								LoadingHelper.Get("InviteJoinClub").StopLoading();
							}
						}
					}
					index--;
				}));
			}
		}

		public void InviteJoinClub(Member member)
		{
			LoadingHelper.Get("InviteJoinClub").ShowLoading(delegate(LoadingHelper helper, float total)
			{
				if (total > 30f)
				{
					helper.StopLoading();
				}
				else
				{
					ShowInviteJoinClubCompleted();
					helper.StopLoading();
				}
			}, delegate(LoadingHelper helper, float total)
			{
				helper.StopLoading();
			}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_Invite_Join_Club"));
			InviteRequest inviteRequest = new InviteRequest();
			inviteRequest.RequestId = RequestId;
			inviteRequest.Cmd = 404;
			inviteRequest.Platform = Application.platform.ToString();
			inviteRequest.AppVersion = Application.version;
			inviteRequest.Channel = "Official";
			inviteRequest.Group = "Normal";
			inviteRequest.PlayerId = GetPlayerId();
			inviteRequest.TargetId = member.PlayerId;
			InviteRequest obj = inviteRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------邀请玩家加入自己俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"邀请玩家成功");
						}
						else
						{
							UnityEngine.Debug.Log($"邀请玩家失败, 代码：{baseResponse.ErrorCode}");
							if (baseResponse.ErrorCode == 10006)
							{
								ShowNotInClub();
								LoadingHelper.Get("InviteJoinClub").StopLoading();
							}
						}
					}
				}
			}));
		}

		public void RequestJoinClub(string identifier, string name, string icon)
		{
			LoadingHelper.Get("RequestJoinClub").ShowLoading(delegate(LoadingHelper helper, float total)
			{
				if (total > 30f)
				{
					helper.StopLoading();
				}
			}, delegate
			{
			}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_RequestJoinClub"));
			RequestRequest requestRequest = new RequestRequest();
			requestRequest.RequestId = RequestId;
			requestRequest.Cmd = 405;
			requestRequest.Platform = Application.platform.ToString();
			requestRequest.AppVersion = Application.version;
			requestRequest.Channel = "Official";
			requestRequest.Group = "Normal";
			requestRequest.PlayerId = GetPlayerId();
			requestRequest.ClubId = identifier;
			requestRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			requestRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			requestRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			requestRequest.SocialPlatform = 1;
			requestRequest.MaxLevel = PlayData.Get().GetMax();
			requestRequest.MaxMasterLevel = PlayData.Get().GetMaxMasterLevels();
			requestRequest.StarCount = PlayData.Get().GetStars();
			RequestRequest obj = requestRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------申请加入俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				LoadingHelper.Get("RequestJoinClub").StopLoading();
				if (download.isDone)
				{
					RequestResponse requestResponse = ProtoDataUtility.Deserialize<RequestResponse>(download.data);
					if (requestResponse != null)
					{
						if (requestResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"申请加入俱乐部成功, Role={requestResponse.Role}");
							if (requestResponse.Role == 1 || requestResponse.Role == 3 || requestResponse.Role == 2)
							{
								JoinNewClub();
							}
							else
							{
								ClubSystemData.Get().AppendRequest(identifier, name, icon);
								SingletonClass<InboxUtility>.Get().UpdateNumber();
								ShowRequestJoinClubSuccess();
							}
						}
						else
						{
							UnityEngine.Debug.Log($"申请加入俱乐部失败, 代码：{requestResponse.ErrorCode}");
							if (requestResponse.ErrorCode == 10003)
							{
								ShowInClub();
							}
							else if (requestResponse.ErrorCode == 10002)
							{
								ShowClubIsFull();
							}
							else
							{
								ShowOperationFaild();
							}
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void Approve(string targetId, bool approve, string name)
		{
			ApproveRequest approveRequest = new ApproveRequest();
			approveRequest.RequestId = RequestId;
			approveRequest.Cmd = 419;
			approveRequest.Platform = Application.platform.ToString();
			approveRequest.AppVersion = Application.version;
			approveRequest.Channel = "Official";
			approveRequest.Group = "Normal";
			approveRequest.PlayerId = GetPlayerId();
			approveRequest.TargetId = targetId;
			approveRequest.Approve = approve;
			ApproveRequest obj = approveRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.LogFormat("-------------------{0}加入俱乐部：{1}", (!approve) ? "拒绝" : "批准", unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.LogFormat("{0}加入俱乐部成功", (!approve) ? "拒绝" : "批准");
						}
						else
						{
							UnityEngine.Debug.LogFormat("{0}加入俱乐部失败, 代码：{1}", (!approve) ? "拒绝" : "批准", baseResponse.ErrorCode);
							if (baseResponse.ErrorCode == 10002)
							{
								ShowClubIsFull();
							}
							else if (baseResponse.ErrorCode == 10003)
							{
								ShowOtherInClubNow(name);
							}
							else
							{
								ShowOperationFaild();
							}
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void Contribute(string clubId, int score, int playNumber)
		{
			UnityEngine.Debug.Log($"俱乐部分数值：{score}");
			ContributeRequest contributeRequest = new ContributeRequest();
			contributeRequest.RequestId = RequestId;
			contributeRequest.Cmd = 427;
			contributeRequest.Platform = Application.platform.ToString();
			contributeRequest.AppVersion = Application.version;
			contributeRequest.Channel = "Official";
			contributeRequest.Group = "Normal";
			contributeRequest.PlayerId = GetPlayerId();
			contributeRequest.ClubId = clubId;
			contributeRequest.Score = score;
			contributeRequest.PlayCount = playNumber;
			contributeRequest.MaxLevel = PlayData.Get().GetMax();
			contributeRequest.MaxMasterLevel = PlayData.Get().GetMaxMasterLevels();
			contributeRequest.StarCount = PlayData.Get().GetStars();
			ContributeRequest obj = contributeRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------提交俱乐部贡献：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					ContributeResponse contributeResponse = ProtoDataUtility.Deserialize<ContributeResponse>(download.data);
					if (contributeResponse != null)
					{
						if (contributeResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"提交俱乐部贡献成功");
							ClubSystemData.Get().SetLeaderboardData(contributeResponse.RankId, upload: true);
							_Responses.Clear();
						}
						else
						{
							UnityEngine.Debug.Log($"提交俱乐部贡献失败, 代码：{contributeResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void ReadOperation(string clubId, string operationId, bool isOperationToMe)
		{
			ReadOperationRequest readOperationRequest = new ReadOperationRequest();
			readOperationRequest.RequestId = RequestId;
			readOperationRequest.Cmd = 421;
			readOperationRequest.Platform = Application.platform.ToString();
			readOperationRequest.AppVersion = Application.version;
			readOperationRequest.Channel = "Official";
			readOperationRequest.Group = "Normal";
			readOperationRequest.PlayerId = GetPlayerId();
			readOperationRequest.ClubId = clubId;
			readOperationRequest.OperationId = operationId;
			readOperationRequest.ToMe = isOperationToMe;
			ReadOperationRequest obj = readOperationRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------阅读俱乐部操作信息：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"阅读俱乐部操作信息成功");
						}
						else
						{
							UnityEngine.Debug.Log($"阅读俱乐部操作信息失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void ReadThanks(string clubId, string giftPackageId)
		{
			ReadThanksRequest readThanksRequest = new ReadThanksRequest();
			readThanksRequest.RequestId = RequestId;
			readThanksRequest.Cmd = 422;
			readThanksRequest.Platform = Application.platform.ToString();
			readThanksRequest.AppVersion = Application.version;
			readThanksRequest.Channel = "Official";
			readThanksRequest.Group = "Normal";
			readThanksRequest.PlayerId = GetPlayerId();
			readThanksRequest.ClubId = clubId;
			readThanksRequest.Guid = giftPackageId;
			ReadThanksRequest obj = readThanksRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------阅读感谢信息：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"阅读感谢信息成功");
						}
						else
						{
							UnityEngine.Debug.Log($"阅读感谢信息失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void Accept(string inviterId, string clubId, string clubName)
		{
			AcceptRequest acceptRequest = new AcceptRequest();
			acceptRequest.RequestId = RequestId;
			acceptRequest.Cmd = 406;
			acceptRequest.Platform = Application.platform.ToString();
			acceptRequest.AppVersion = Application.version;
			acceptRequest.Channel = "Official";
			acceptRequest.Group = "Normal";
			acceptRequest.PlayerId = GetPlayerId();
			acceptRequest.ClubId = clubId;
			acceptRequest.InviterId = inviterId;
			acceptRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			acceptRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			acceptRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			acceptRequest.SocialPlatform = 1;
			acceptRequest.MaxLevel = PlayData.Get().GetMax();
			acceptRequest.MaxMasterLevel = PlayData.Get().GetMaxMasterLevels();
			acceptRequest.StarCount = PlayData.Get().GetStars();
			AcceptRequest obj = acceptRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------同意加入俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					RequestResponse requestResponse = ProtoDataUtility.Deserialize<RequestResponse>(download.data);
					if (requestResponse != null)
					{
						if (requestResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"同意加入俱乐部成功, Role={requestResponse.Role}");
							if (requestResponse.Role == 1 || requestResponse.Role == 3 || requestResponse.Role == 2)
							{
								JoinNewClub();
							}
							else
							{
								ShowWattingLeaderAccept(clubName);
							}
						}
						else
						{
							if (requestResponse.ErrorCode == 10002)
							{
								ShowClubIsFull();
							}
							else if (requestResponse.ErrorCode == 10001)
							{
								ShowNotFoundClub();
							}
							else
							{
								ShowOperationFaild();
							}
							UnityEngine.Debug.Log($"同意加入俱乐部失败, 代码：{requestResponse.ErrorCode}");
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void Reject(string inviterId, string clubId)
		{
			AcceptRequest acceptRequest = new AcceptRequest();
			acceptRequest.RequestId = RequestId;
			acceptRequest.Cmd = 407;
			acceptRequest.Platform = Application.platform.ToString();
			acceptRequest.AppVersion = Application.version;
			acceptRequest.Channel = "Official";
			acceptRequest.Group = "Normal";
			acceptRequest.PlayerId = GetPlayerId();
			acceptRequest.ClubId = clubId;
			acceptRequest.InviterId = inviterId;
			acceptRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			acceptRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			acceptRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			acceptRequest.SocialPlatform = 1;
			AcceptRequest obj = acceptRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------拒绝加入俱乐部：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"拒绝加入俱乐部成功");
						}
						else
						{
							ShowOperationFaild();
							UnityEngine.Debug.Log($"拒绝加入俱乐部失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void RandomClub()
		{
			if (!ContainsTaskId("RandomClub"))
			{
				AppendTaskId("RandomClub");
				RecommendClubRequest recommendClubRequest = new RecommendClubRequest();
				recommendClubRequest.RequestId = RequestId;
				recommendClubRequest.Cmd = 423;
				recommendClubRequest.Platform = Application.platform.ToString();
				recommendClubRequest.AppVersion = Application.version;
				recommendClubRequest.Channel = "Official";
				recommendClubRequest.Group = "Normal";
				recommendClubRequest.PlayerId = GetPlayerId();
				RecommendClubRequest obj = recommendClubRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取推荐俱乐部：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("RandomClub");
					if (download.isDone)
					{
						RecommendClubResponse recommendClubResponse = ProtoDataUtility.Deserialize<RecommendClubResponse>(download.data);
						if (recommendClubResponse != null)
						{
							if (recommendClubResponse.ErrorCode == 1)
							{
								if (recommendClubResponse.Clubs.Count > 0)
								{
									_ClubEvent.Invoke("Random", recommendClubResponse.Clubs[0]);
								}
								UnityEngine.Debug.Log($"获取推荐俱乐部成功, ClubCount: {recommendClubResponse.Clubs.Count}");
							}
							else
							{
								UnityEngine.Debug.Log($"获取推荐俱乐部失败, 代码：{recommendClubResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void Profile(string playerName, string avatar)
		{
			string taskId = $"Profile_{playerName}_{avatar}";
			if (!ContainsTaskId(taskId))
			{
				AppendTaskId(taskId);
				ProfileRequest profileRequest = new ProfileRequest();
				profileRequest.RequestId = RequestId;
				profileRequest.Cmd = 424;
				profileRequest.Platform = Application.platform.ToString();
				profileRequest.AppVersion = Application.version;
				profileRequest.Channel = "Official";
				profileRequest.Group = "Normal";
				profileRequest.PlayerId = GetPlayerId();
				profileRequest.PlayerName = playerName;
				profileRequest.PlayerAvatar = avatar;
				ProfileRequest obj = profileRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------修改俱乐部中的个人信息：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId(taskId);
					if (download.isDone)
					{
						BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
						if (baseResponse != null)
						{
							if (baseResponse.ErrorCode == 1)
							{
								UnityEngine.Debug.Log("修改俱乐部中的个人信息成功");
							}
							else
							{
								UnityEngine.Debug.Log($"修改俱乐部中的个人信息失败, 代码：{baseResponse.ErrorCode}");
								if (baseResponse.ErrorCode != 10001 && baseResponse.ErrorCode != 10006)
								{
									SingletonBehaviour<StepUtility>.Get().Append("Profile", 10f, delegate
									{
										Profile(playerName, avatar);
									});
								}
							}
						}
					}
					else
					{
						SingletonBehaviour<StepUtility>.Get().Append("Profile", 10f, delegate
						{
							Profile(playerName, avatar);
						});
					}
				}));
			}
		}

		public void MyClub(UnityAction<bool> unityAction = null)
		{
#if ENABLE_AZURE
			if (!ContainsTaskId("MyClub"))
			{
				AppendTaskId("MyClub");
				MyClubRequest myClubRequest = new MyClubRequest();
				myClubRequest.RequestId = RequestId;
				myClubRequest.Cmd = 411;
				myClubRequest.Platform = Application.platform.ToString();
				myClubRequest.AppVersion = Application.version;
				myClubRequest.Channel = "Official";
				myClubRequest.Group = "Normal";
				myClubRequest.PlayerId = GetPlayerId();
				MyClubRequest obj = myClubRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取自己的俱乐部信息：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("MyClub");
					if (download.isDone)
					{
						MyClubResponse myClubResponse = ProtoDataUtility.Deserialize<MyClubResponse>(download.data);
						if (myClubResponse != null)
						{
							if (myClubResponse.ErrorCode == 1)
							{
								if (myClubResponse.Club == null)
								{
									ClubSystemData.Get().ClearScore();
								}
								else
								{
									Segment = EnumUtility.GetEnumType(myClubResponse.Club.Stage, SegmentType.Bronze);
								}
								if (Segment == (SegmentType)0)
								{
									Segment = SegmentType.Bronze;
								}
								UpdateClubResponse(myClubResponse);
								UnityEngine.Debug.Log(string.Format("获取自己的俱乐部信息成功，ClubId={0}, Member={1}/{2}, State={3}, OperationsToMe={4}, Invitations={5}, Gifts={6}, Thanks={7}, TemporaryMembers={8}", (myClubResponse.Club != null) ? myClubResponse.Club.ClubId : "None", (myClubResponse.Club != null) ? myClubResponse.Club.MemberCount : (-1), (myClubResponse.Club != null) ? myClubResponse.Club.MemberLimit : (-1), myClubResponse.State, (myClubResponse.ClubOperationsToMe != null) ? myClubResponse.ClubOperationsToMe.Count.ToString() : string.Empty, (myClubResponse.Invitations != null) ? myClubResponse.Invitations.Count : 0, (myClubResponse.Gifts != null) ? myClubResponse.Gifts.Count : 0, (myClubResponse.Thanks != null) ? myClubResponse.Thanks.Count : 0, (myClubResponse.TemporaryMembers != null) ? myClubResponse.TemporaryMembers.Count : 0));
								if (unityAction != null)
								{
									unityAction(arg0: true);
								}
							}
							else
							{
								UnityEngine.Debug.Log($"获取自己的俱乐部信息失败, 代码：{myClubResponse.ErrorCode}");
								if (unityAction != null)
								{
									unityAction(arg0: false);
								}
							}
						}
					}
					else if (unityAction != null)
					{
						unityAction(arg0: false);
					}
				}));
			}
#endif
		}

		private void UpdateClubResponse(MyClubResponse res)
		{
			_MyClubResponse = res;
			_MyClubResponseEvent.Invoke(_MyClubResponse);
			SingletonClass<InboxUtility>.Get().UpdateNumber();
		}

		public void GetClub(string identifier)
		{
			ClubInfoRequest clubInfoRequest = new ClubInfoRequest();
			clubInfoRequest.RequestId = RequestId;
			clubInfoRequest.Cmd = 412;
			clubInfoRequest.Platform = Application.platform.ToString();
			clubInfoRequest.AppVersion = Application.version;
			clubInfoRequest.Channel = "Official";
			clubInfoRequest.Group = "Normal";
			clubInfoRequest.PlayerId = GetPlayerId();
			clubInfoRequest.ClubId = identifier;
			ClubInfoRequest obj = clubInfoRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------获取俱乐部信息：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					ClubInfoResponse clubInfoResponse = ProtoDataUtility.Deserialize<ClubInfoResponse>(download.data);
					if (clubInfoResponse != null)
					{
						if (clubInfoResponse.ErrorCode == 1)
						{
							if (identifier.Equals(clubInfoResponse.Club.ClubId))
							{
								_ClubEvent.Invoke(identifier, clubInfoResponse.Club);
							}
							UnityEngine.Debug.Log($"获取俱乐部信息成功，ClubId={clubInfoResponse.Club.ClubId}, Member={clubInfoResponse.Club.MemberCount}/{clubInfoResponse.Club.MemberLimit}");
						}
						else
						{
							UnityEngine.Debug.Log($"获取俱乐部信息失败, 代码：{clubInfoResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void RemoveClubUser(Member member)
		{
			RemoveRequest removeRequest = new RemoveRequest();
			removeRequest.RequestId = RequestId;
			removeRequest.Cmd = 408;
			removeRequest.Platform = Application.platform.ToString();
			removeRequest.AppVersion = Application.version;
			removeRequest.Channel = "Official";
			removeRequest.Group = "Normal";
			removeRequest.PlayerId = GetPlayerId();
			removeRequest.ClubId = GetClubIdentifier();
			removeRequest.TargetId = member.PlayerId;
			RemoveRequest obj = removeRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------从俱乐部移除成员：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"从俱乐部移除成员成功");
							ShowRemoveUserClubCompleted();
							if (_MyClubResponse != null && _MyClubResponse.Club != null && _MyClubResponse.Club.Members != null && _MyClubResponse.Club.Members.Contains(member))
							{
								_MyClubResponse.Club.Members.Remove(member);
								_MyClubResponse.Club.MemberCount--;
								_MyClubResponseEvent.Invoke(_MyClubResponse);
							}
						}
						else
						{
							ShowOperationFaild();
							UnityEngine.Debug.Log($"从俱乐部移除成员失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void EditorClubUser(Member member)
		{
			AssignRequest assignRequest = new AssignRequest();
			assignRequest.RequestId = RequestId;
			assignRequest.Cmd = 403;
			assignRequest.Platform = Application.platform.ToString();
			assignRequest.AppVersion = Application.version;
			assignRequest.Channel = "Official";
			assignRequest.Group = "Normal";
			assignRequest.PlayerId = GetPlayerId();
			assignRequest.TargetId = member.PlayerId;
			assignRequest.Role = member.Role;
			AssignRequest obj = assignRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------更改成员权限：" + unityWebRequest.url);
			if (_MyClubResponse != null)
			{
				_MyClubResponseEvent.Invoke(_MyClubResponse);
			}
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							if (_MyClubResponse != null)
							{
								_MyClubResponseEvent.Invoke(_MyClubResponse);
							}
							UnityEngine.Debug.Log($"更改成员权限成功");
							ShowMemberEditorCompleted(member);
						}
						else
						{
							ShowOperationFaild();
							UnityEngine.Debug.Log($"更改成员权限失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
				else
				{
					ShowOperationFaild();
				}
			}));
		}

		public void LeaveClub()
		{
			if (_MyClubResponse != null && _MyClubResponse.Club != null)
			{
				LoadingHelper.Get("LeaveClub").ShowLoading(null, delegate(LoadingHelper helper, float total)
				{
					helper.StopLoading();
				}, LocalizationUtility.Get("Localization_Club.json").GetString("Loading_Leave_Clubing"));
				LeaveRequest leaveRequest = new LeaveRequest();
				leaveRequest.RequestId = RequestId;
				leaveRequest.Cmd = 409;
				leaveRequest.Platform = Application.platform.ToString();
				leaveRequest.AppVersion = Application.version;
				leaveRequest.Channel = "Official";
				leaveRequest.Group = "Normal";
				leaveRequest.PlayerId = GetPlayerId();
				leaveRequest.ClubId = _MyClubResponse.Club.ClubId;
				LeaveRequest obj = leaveRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------离开俱乐部：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					LoadingHelper.Get("LeaveClub").StopLoading();
					if (download.isDone)
					{
						BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
						if (baseResponse != null)
						{
							if (baseResponse.ErrorCode == 1 || baseResponse.ErrorCode == 10006)
							{
								UnityEngine.Debug.Log($"离开俱乐部成功");
								ShowQuitClub();
							}
							else
							{
								ShowOperationFaild();
								UnityEngine.Debug.Log($"离开俱乐部失败, 代码：{baseResponse.ErrorCode}");
							}
						}
					}
					else
					{
						ShowOperationFaild();
					}
				}));
			}
		}

		public void SendGift(GiftData gift)
		{
			SendGiftRequest sendGiftRequest = new SendGiftRequest();
			sendGiftRequest.RequestId = RequestId;
			sendGiftRequest.Cmd = 413;
			sendGiftRequest.Platform = Application.platform.ToString();
			sendGiftRequest.AppVersion = Application.version;
			sendGiftRequest.Channel = "Official";
			sendGiftRequest.Group = "Normal";
			sendGiftRequest.PlayerId = GetPlayerId();
			sendGiftRequest.ClubId = gift.clubId;
			sendGiftRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			sendGiftRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			sendGiftRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			sendGiftRequest.SocialPlatform = 1;
			sendGiftRequest.GiftId = gift.giftId;
			sendGiftRequest.TargetId = ((!string.IsNullOrEmpty(gift.targetId)) ? gift.targetId : null);
			sendGiftRequest.Guid = gift.guid;
			SendGiftRequest obj = sendGiftRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------赠送礼物给俱乐部成员：" + unityWebRequest.url);
			SingletonData<ClubPlayerData>.Get().AppendGift(gift);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						UnityEngine.Debug.Log($"赠送礼物给俱乐部成员, 错误代码：{baseResponse.ErrorCode}");
						switch (baseResponse.ErrorCode)
						{
						case 1:
							MyClub();
							SingletonData<ClubPlayerData>.Get().RemoveGift(gift);
							break;
						case 10006:
							SingletonData<ClubPlayerData>.Get().RemoveGift(gift);
							break;
						default:
							SingletonBehaviour<StepUtility>.Get().Append("SendGift", 10f, delegate
							{
								SendGift(gift);
							});
							break;
						}
					}
				}
			}));
		}

		public void AcceptGift(string giftPackageId, string clubId)
		{
			AcceptGiftRequest acceptGiftRequest = new AcceptGiftRequest();
			acceptGiftRequest.RequestId = RequestId;
			acceptGiftRequest.Cmd = 414;
			acceptGiftRequest.Platform = Application.platform.ToString();
			acceptGiftRequest.AppVersion = Application.version;
			acceptGiftRequest.Channel = "Official";
			acceptGiftRequest.Group = "Normal";
			acceptGiftRequest.PlayerId = GetPlayerId();
			acceptGiftRequest.ClubId = clubId;
			acceptGiftRequest.Guid = giftPackageId;
			AcceptGiftRequest obj = acceptGiftRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------接受俱乐部成员赠送的礼物：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"接受俱乐部成员赠送的礼物成功");
						}
						else
						{
							UnityEngine.Debug.Log($"接受俱乐部成员赠送的礼物失败, 代码：{baseResponse.ErrorCode}");
						}
					}
				}
			}));
		}

		public void ThanksForGift(string giftPackageId, string giftId, string clubId, string giverId)
		{
			ThanksForGiftRequest thanksForGiftRequest = new ThanksForGiftRequest();
			thanksForGiftRequest.RequestId = RequestId;
			thanksForGiftRequest.Cmd = 415;
			thanksForGiftRequest.Platform = Application.platform.ToString();
			thanksForGiftRequest.AppVersion = Application.version;
			thanksForGiftRequest.Channel = "Official";
			thanksForGiftRequest.Group = "Normal";
			thanksForGiftRequest.PlayerId = GetPlayerId();
			thanksForGiftRequest.ClubId = clubId;
			thanksForGiftRequest.PlayerName = AuxiliaryData.Get().GetNickName();
			thanksForGiftRequest.Avatar = AuxiliaryData.Get().AvaterFileName;
			thanksForGiftRequest.SocialId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			thanksForGiftRequest.SocialPlatform = 1;
			thanksForGiftRequest.GiftId = giftId;
			thanksForGiftRequest.Guid = giftPackageId;
			thanksForGiftRequest.GiverId = giverId;
			ThanksForGiftRequest obj = thanksForGiftRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------感谢俱乐部成员赠送的礼物：" + unityWebRequest.url);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					BaseResponse baseResponse = ProtoDataUtility.Deserialize<BaseResponse>(download.data);
					if (baseResponse != null)
					{
						if (baseResponse.ErrorCode == 1)
						{
							UnityEngine.Debug.Log($"感谢俱乐部成员赠送的礼物成功");
						}
						else
						{
							UnityEngine.Debug.Log($"感谢俱乐部成员赠送的礼物失败, 代码：{baseResponse.ErrorCode}");
							if (baseResponse.ErrorCode != 503)
							{
								SingletonBehaviour<StepUtility>.Get().Append("SendGift", 10f, delegate
								{
									ThanksForGift(giftPackageId, giftId, clubId, giverId);
								});
							}
						}
					}
				}
				else
				{
					SingletonBehaviour<StepUtility>.Get().Append("SendGift", 10f, delegate
					{
						ThanksForGift(giftPackageId, giftId, clubId, giverId);
					});
				}
			}));
		}

		public RankType GetRankType()
		{
			if (_connect)
			{
				return _currentRankType;
			}
			return RankType.None;
		}

		public RankType GetRankTypeOffline()
		{
			return _currentRankType;
		}

		public TimeSpan GetUploadRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public TimeSpan GetSettleRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public TimeSpan GetRewardRemainTime()
		{
			return TimeSpan.FromMilliseconds(StateCountdown);
		}

		public void GetLeaderboardState()
		{
#if ENABLE_AZURE
            LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
			leaderboardRequest.RequestId = RequestId;
			leaderboardRequest.Cmd = 205;
			leaderboardRequest.Platform = Application.platform.ToString();
			leaderboardRequest.AppVersion = Application.version;
			leaderboardRequest.Channel = "Official";
			leaderboardRequest.Group = "Normal";
			leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
			LeaderboardRequest obj = leaderboardRequest;
			UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
			UnityEngine.Debug.Log("-------------------获取俱乐部高分榜状态：" + unityWebRequest.url);
			_connect = false;
			RankChanged.Invoke(RankType.None);
			StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
			{
				if (download.isDone)
				{
					LeaderboardStateResponse leaderboardStateResponse = ProtoDataUtility.Deserialize<LeaderboardStateResponse>(download.data);
					if (leaderboardStateResponse == null)
					{
						return;
					}
					if (leaderboardStateResponse.ErrorCode == 1)
					{
						UnityEngine.Debug.Log($"获取俱乐部高分榜状态成功，State: {leaderboardStateResponse.State}，StateCountdown: {leaderboardStateResponse.StateCountdown}, Ticks: {leaderboardStateResponse.Ticks}, RankId: {leaderboardStateResponse.RankId}");
						ClubSystemData.Get().SetLeaderboardData(leaderboardStateResponse.RankId);
						ChangeRankType(EnumUtility.GetEnumType(leaderboardStateResponse.State, RankType.Reward), leaderboardStateResponse.StateCountdown);
						return;
					}
					UnityEngine.Debug.LogWarning($"获取高分榜状态，代码：{leaderboardStateResponse.ErrorCode}");
				}
				DelayDo(new WaitForSeconds(10f), GetLeaderboardState);
			}));
#endif
		}

		private void ChangeRankType(RankType rankType, int stateCountdown)
		{
			if (stateCountdown == 0)
			{
				stateCountdown = 10000;
			}
			StateCountdown = stateCountdown;
			UnityEngine.Debug.Log($"==============当前的排行榜状态{rankType}, 结束倒计时时间{StateCountdown}");
			_connect = true;
			if (_currentRankType != rankType)
			{
				_lastRankType = _currentRankType;
				_currentRankType = rankType;
				GetLeaderboardReward();
				RankChanged.Invoke(rankType);
				_Responses.Clear();
			}
			else if (GetRankType() != rankType)
			{
				RankChanged.Invoke(rankType);
				_Responses.Clear();
			}
		}

		public void GetLeaderboardRank()
		{
			if (!string.IsNullOrEmpty(GetClubIdentifier()) && _Responses.ContainsKey(GetClubIdentifier()))
			{
				RankEvent.Invoke(_Responses[GetClubIdentifier()]);
			}
			else if (!ContainsTaskId("GetClubLeaderboardRank"))
			{
				AppendTaskId("GetClubLeaderboardRank");
				ClubLeaderboardRequest clubLeaderboardRequest = new ClubLeaderboardRequest();
				clubLeaderboardRequest.RequestId = RequestId;
				clubLeaderboardRequest.Cmd = 203;
				clubLeaderboardRequest.Platform = Application.platform.ToString();
				clubLeaderboardRequest.AppVersion = Application.version;
				clubLeaderboardRequest.Channel = "Official";
				clubLeaderboardRequest.Group = "Normal";
				clubLeaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				ClubLeaderboardRequest obj = clubLeaderboardRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取俱乐部完整高分榜：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("GetClubLeaderboardRank");
					if (download.isDone)
					{
						ClubLeaderboardListResponse clubLeaderboardListResponse = ProtoDataUtility.Deserialize<ClubLeaderboardListResponse>(download.data);
						if (clubLeaderboardListResponse != null)
						{
							if (clubLeaderboardListResponse.ErrorCode == 1)
							{
								Segment = EnumUtility.GetEnumType(clubLeaderboardListResponse.Stage, SegmentType.Bronze);
								if (Segment == (SegmentType)0)
								{
									Segment = SegmentType.Bronze;
								}
								ClubSystemData.Get().SetLeaderboardData(clubLeaderboardListResponse.RankId);
								UnityEngine.Debug.Log($"获取俱乐部完整高分榜成功, RankId: {clubLeaderboardListResponse.RankId}, Stage: {clubLeaderboardListResponse.Stage}, UpgradePosition: {clubLeaderboardListResponse.UpgradePosition}, DowngradePosition: {clubLeaderboardListResponse.DowngradePosition}, TopPlayers: {clubLeaderboardListResponse.TopPlayers.Count}, State: {clubLeaderboardListResponse.State}, StateCountdown: {clubLeaderboardListResponse.StateCountdown}");
								string clubIdentifier = GetClubIdentifier();
								if (!string.IsNullOrEmpty(clubIdentifier))
								{
									if (_Responses.ContainsKey(clubIdentifier))
									{
										_Responses.Remove(clubIdentifier);
									}
									_Responses.Add(clubIdentifier, clubLeaderboardListResponse);
								}
								RankEvent.Invoke(clubLeaderboardListResponse);
								UnityEngine.Debug.Log("------------------------------------------------------");
								foreach (ClubRank topPlayer in clubLeaderboardListResponse.TopPlayers)
								{
									UnityEngine.Debug.Log($"#{topPlayer.Rank} {topPlayer.ClubIcon} {topPlayer.ClubName} {topPlayer.MemberCount}/{topPlayer.MemberLimit} {topPlayer.Level} {topPlayer.Score} {topPlayer.ClubId}");
								}
								UnityEngine.Debug.Log("------------------------------------------------------");
							}
							else
							{
								UnityEngine.Debug.Log($"获取俱乐部完整高分榜失败, 代码：{clubLeaderboardListResponse.ErrorCode}");
							}
						}
					}
				}));
			}
		}

		public void GetLeaderboardReward()
		{
#if ENABLE_AZURE
			if (!ContainsTaskId("GetClubLeaderboardReward"))
			{
				AppendTaskId("GetClubLeaderboardReward");
				LeaderboardRequest leaderboardRequest = new LeaderboardRequest();
				leaderboardRequest.RequestId = RequestId;
				leaderboardRequest.Cmd = 204;
				leaderboardRequest.Platform = Application.platform.ToString();
				leaderboardRequest.AppVersion = Application.version;
				leaderboardRequest.Channel = "Official";
				leaderboardRequest.Group = "Normal";
				leaderboardRequest.PlayerId = SolitaireTripeaksData.Get().GetPlayerId();
				LeaderboardRequest obj = leaderboardRequest;
				UnityWebRequest unityWebRequest = UnityWebRequest.Get($"{_clubApi}/container/club?args={WWW.EscapeURL(ProtoDataUtility.SerializeToBase64(obj))}");
				UnityEngine.Debug.Log("-------------------获取俱乐部高分榜奖励：" + unityWebRequest.url);
				StartCoroutine(StartUnityWeb(unityWebRequest, delegate(DownloadHandler download)
				{
					RemoveTaskId("GetClubLeaderboardReward");
					if (download.isDone)
					{
						LeaderboardRewardResponse leaderboardRewardResponse = ProtoDataUtility.Deserialize<LeaderboardRewardResponse>(download.data);
						if (leaderboardRewardResponse != null)
						{
							if (leaderboardRewardResponse.ErrorCode == 1)
							{
								Rewards = leaderboardRewardResponse.RewardTable.ToList();
								Segment = EnumUtility.GetEnumType(leaderboardRewardResponse.CurrentStage, SegmentType.Bronze);
								if (Segment == (SegmentType)0)
								{
									Segment = SegmentType.Bronze;
								}
								List<PurchasingCommodity> commoditys = GetCommoditys(leaderboardRewardResponse.RewardStage, leaderboardRewardResponse.RewardPosition, leaderboardRewardResponse.RewardTable.ToList());
								UnityEngine.Debug.Log("------------------------------------------------------");
								foreach (PurchasingCommodity item in commoditys)
								{
									UnityEngine.Debug.Log($"#{item.boosterType} {item.count}");
								}
								UnityEngine.Debug.Log("------------------------------------------------------");
								RankType enumType = EnumUtility.GetEnumType(leaderboardRewardResponse.State, RankType.Reward);
								if (enumType != RankType.Settle && leaderboardRewardResponse.RewardPosition != -1)
								{
									ClubSystemData.Get().SetLeaderboardData(leaderboardRewardResponse.RewardRankId, leaderboardRewardResponse.RewardPosition, EnumUtility.GetEnumType(leaderboardRewardResponse.RewardStage, SegmentType.Bronze), EnumUtility.GetEnumType(leaderboardRewardResponse.CurrentStage, SegmentType.Bronze), GetCommoditys(leaderboardRewardResponse.RewardStage, leaderboardRewardResponse.RewardPosition));
									UnityEngine.Debug.Log($"获取俱乐部高分榜奖励成功，RewardRankId: {leaderboardRewardResponse.RewardRankId}RewardStage：{leaderboardRewardResponse.RewardStage}, RewardPosition：{leaderboardRewardResponse.RewardPosition},, RewardTable = {leaderboardRewardResponse.RewardTable.Count}, CurrentRankId: {leaderboardRewardResponse.CurrentRankId}, CurrentStage: {leaderboardRewardResponse.CurrentStage}, State: {leaderboardRewardResponse.State}, StateCountDown: {leaderboardRewardResponse.StateCountdown}");
								}
							}
							else
							{
								UnityEngine.Debug.LogWarning($"获取高分榜奖励失败，代码：{leaderboardRewardResponse.ErrorCode}");
							}
						}
					}
				}));
			}
#endif
		}

		public List<PurchasingCommodity> GetCommoditys(int segment, int position)
		{
			return (from e in Rewards
				where e.Stage == segment
				where e.MinPosition <= position && e.MaxPosition >= position
				select new PurchasingCommodity
				{
					count = e.Amount,
					boosterType = EnumUtility.GetEnumType(e.ItemId, BoosterType.Coins)
				}).ToList();
		}

		public int GetCoins(int segment, int position)
		{
			List<PurchasingCommodity> commoditys = GetCommoditys(segment, position);
			return commoditys.Find((PurchasingCommodity e) => e.boosterType == BoosterType.Coins)?.count ?? 0;
		}

		public Vector2 GetCoins(int segment)
		{
			IEnumerable<RewardItem> enumerable = from e in Rewards
				where e.Stage == segment
				where e.ItemId == 1
				select e;
			if (enumerable == null || enumerable.Count() == 0)
			{
				return Vector2.zero;
			}
			return new Vector2(enumerable.Min((RewardItem e) => e.Amount), enumerable.Max((RewardItem e) => e.Amount));
		}

		public void TryShowClubLeaderBoard()
		{
			UnityAction LeaderboardRewards = null;
			LeaderboardRewards = delegate
			{
				List<RankRewardData> leaderboardDatas = ClubSystemData.Get().GetLeaderboardDatas();
				if (leaderboardDatas.Count > 0)
				{
					SingletonClass<MySceneManager>.Get().Popup<LeaderboardRewardScene>("Scenes/LeaderboardRewardScene", new NavigationEffect()).OnStart(isClan: true, leaderboardDatas[0], LeaderboardRewards);
				}
				else if (Rewards.Count > 0)
				{
					UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene e)
					{
						e.transform.FindType("Canvas/Parent/Groups", delegate(TabGroup tab)
						{
							tab.SetTabIndex(2);
						});
					});
				}
				else
				{
					GetLeaderboardReward();
					LoadingHelper.Get("LeaderboardRewardsScene").ShowLoading(delegate(LoadingHelper helper, float total)
					{
						if (Rewards.Count > 0)
						{
							TryShowClubLeaderBoard();
							helper.StopLoading();
						}
						else if (total > 15f)
						{
							helper.StopLoading();
						}
					}, delegate(LoadingHelper helper, float total)
					{
						helper.StopLoading();
					}, LocalizationUtility.Get("Localization_LeaderBoard.json").GetString("loading_leaderboard_rewards"));
				}
			};
			LeaderboardRewards();
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				GetLeaderboardState();
			}
		}

		private void Update()
		{
			if (_connect)
			{
				StateCountdown -= Time.unscaledDeltaTime * 1000f;
				if (StateCountdown <= 0.0)
				{
					GetLeaderboardState();
				}
			}
		}

		public void ShowClubNameCanotEmpty()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Club_Name_Canot_Empty_Title"), localizationUtility.GetString("Club_Name_Canot_Empty_Description"));
		}

		public void ShowWattingLeaderAccept(string clubName)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Watting_Leader_Accept_Title"), string.Format(localizationUtility.GetString("Watting_Leader_Accept_Description"), clubName));
		}

		public void JoinNewClub()
		{
			UpdateClubResponse(null);
			ClubSystemData.Get().ClearScore();
			CloseAllShowClubScene();
		}

		public void ShowNotInClub()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Not_In_Club_Title"), localizationUtility.GetString("Not_In_Club_Description"), CloseAllShowJoinClubScene);
		}

		public void ShowClubIsFull()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Club_IsFull_Title"), localizationUtility.GetString("Club_IsFull_Description"));
		}

		public void ShowModifyClubCompleted(UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("ModifyClub_Success_Title"), localizationUtility.GetString("ModifyClub_Success_Content"), unityAction);
		}

		public void ShowNotFoundClub(UnityAction unityAction = null)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Not_Found_Club_Title"), localizationUtility.GetString("Not_Found_Club_Content"), unityAction);
		}

		public void ShowOtherInClubNow(string name)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Other_In_Club_Now_Title"), string.Format(localizationUtility.GetString("Other_In_Club_Now_Description"), name));
		}

		public void ShowRequestJoinClubSuccess()
		{
			bool leaveFirst = false;
			if (_MyClubResponse != null && _MyClubResponse.Club != null)
			{
				UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene clubScene)
				{
					if (clubScene.ClubId.Equals(_MyClubResponse.Club.ClubId))
					{
						SingletonClass<MySceneManager>.Get().Close(clubScene);
						leaveFirst = true;
					}
				});
				_MyClubResponse.Club = null;
			}
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Request_Join_Club_Success_Title"), (!leaveFirst) ? localizationUtility.GetString("Request_Join_Club_Success_Description") : localizationUtility.GetString("Request_Join_Club_Success_And_Leave_Description"));
		}

		public void ShowInClub()
		{
			UpdateClubResponse(null);
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("In_Club_Title"), localizationUtility.GetString("In_Club_Description"), CloseAllShowClubScene);
		}

		public void ShowInviteJoinClubCompleted()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("InviteJoinClubCompleted_Title"), localizationUtility.GetString("InviteJoinClubCompleted_Description"));
		}

		public void ShowQuitClub()
		{
			ClubSystemData.Get().ClearScore();
			UpdateClubResponse(null);
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Quit_Club_Title"), localizationUtility.GetString("Quit_Club_Description"), CloseAllShowJoinClubScene);
		}

		public void CloseAllShowJoinClubScene()
		{
			UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene scene)
			{
				SingletonClass<MySceneManager>.Get().Close(scene);
			});
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ChatScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ClubAvatarScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ClubPlayerScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<CreatorClubScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<InviteJoinClubScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<JoinClubScene>());
			SingletonClass<MySceneManager>.Get().Popup<JoinClubScene>("Scenes/JoinClubScene");
		}

		public void CloseAllShowClubScene()
		{
			UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene scene)
			{
				SingletonClass<MySceneManager>.Get().Close(scene);
			});
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ChatScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ClubAvatarScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<ClubPlayerScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<CreatorClubScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<InviteJoinClubScene>());
			SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<JoinClubScene>());
			ShowClubScene();
		}

		public void ShowClubScene()
		{
			if (_MyClubResponse == null)
			{
				MyClub(delegate(bool success)
				{
					if (!success)
					{
						LoadingHelper.Get("ShowClub").StopLoading();
						ShowOperationFaild();
					}
				});
				LoadingHelper.Get("ShowClub").ShowLoading(delegate(LoadingHelper helper, float total)
				{
					if (_MyClubResponse != null)
					{
						ShowClubScene();
						helper.StopLoading();
					}
					else if (total > 15f)
					{
						helper.StopLoading();
					}
				}, delegate(LoadingHelper helper, float total)
				{
					helper.StopLoading();
				}, LocalizationUtility.Get("Localization_Club.json").GetString("loading_Club"));
			}
			else if (_MyClubResponse.Club == null)
			{
				MyClub();
				SingletonClass<MySceneManager>.Get().Popup<JoinClubScene>("Scenes/JoinClubScene");
			}
			else
			{
				UnityEngine.Object.FindObjectsOfType<ClubScene>().ForEach(delegate(ClubScene scene)
				{
					SingletonClass<MySceneManager>.Get().Close(scene);
				});
				SingletonClass<MySceneManager>.Get().Popup<ClubScene>("Scenes/ClubScene").OnStart();
			}
		}

		public void ShowLeaveClub(UnityAction unityAction = null)
		{
			if (_MyClubResponse != null && _MyClubResponse.Club != null)
			{
				bool flag = _MyClubResponse.Club.MemberCount == 1;
				LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
				SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility.GetString("LeaveClub_Title"), (!flag) ? localizationUtility.GetString("Leave_Club_description") : localizationUtility.GetString("Leave_Club_only_you_description"), localizationUtility.GetString("Btn_Leave"), localizationUtility.GetString("Btn_Cancel"), delegate(bool sure)
				{
					SingletonClass<MySceneManager>.Get().Close();
					if (sure)
					{
						LeaveClub();
					}
					if (unityAction != null)
					{
						unityAction();
					}
				});
			}
		}

		public void ShowRemoveUserClub(Member member)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility.GetString("Remove_User_Title"), localizationUtility.GetString("Remove_User_description"), localizationUtility.GetString("Btn_Remove"), localizationUtility.GetString("Btn_Cancel"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close();
				if (sure)
				{
					RemoveClubUser(member);
					SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
				}
			});
		}

		public void ShowRemoveUserClubCompleted()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Remove_Member_Completed_Title"), localizationUtility.GetString("Remove_ToMember_Completed_description"));
		}

		public void ShowEditorUserMessages(bool upgrde, string name, UnityAction unityAction)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility.GetString("Editor_User_Title"), (!upgrde) ? string.Format(localizationUtility.GetString("Editor_User_Demote_description"), name) : string.Format(localizationUtility.GetString("Editor_User_PromoteToCoLeader_description"), name), localizationUtility.GetString("btn_ok"), localizationUtility.GetString("Btn_Cancel"), delegate(bool sure)
			{
				SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
				if (sure && unityAction != null)
				{
					unityAction();
				}
			});
		}

		public void ShowMemberEditorCompleted(Member member)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			string @string = localizationUtility.GetString("Editor_ToMember_Completed_description");
			if (member.Role == 2)
			{
				@string = localizationUtility.GetString("Editor_To_Leader_Completed_description");
			}
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Editor_Member_Completed_Title"), string.Format(@string, member.PlayerName));
		}

		public void ShowOperationFaild()
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
			TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("Operation_Faild_Title"), localizationUtility.GetString("Operation_Faild_Description"));
		}

		public void ShowRequestJoinClubScene(Club club)
		{
			UnityAction JoinClubRequest = delegate
			{
				if (_MyClubResponse != null && _MyClubResponse.Club != null)
				{
					LocalizationUtility localizationUtility2 = LocalizationUtility.Get("Localization_Club.json");
					SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility2.GetString("Leave_Request_Join_Club_Title"), localizationUtility2.GetString("Leave_Request_Join_Club_description"), localizationUtility2.GetString("btn_ok"), localizationUtility2.GetString("Btn_Cancel"), delegate(bool sure)
					{
						SingletonClass<MySceneManager>.Get().Close();
						if (sure)
						{
							RequestJoinClub(club.ClubId, club.ClubName, club.ClubIcon);
						}
					});
				}
				else
				{
					RequestJoinClub(club.ClubId, club.ClubName, club.ClubIcon);
				}
			};
			if (club.MemberCount >= club.MemberLimit)
			{
				if (club.Private)
				{
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Club.json");
					SingletonClass<MySceneManager>.Get().Popup<TipPopupDoubleButtonScene>("Scenes/Pops/TipPopupDoubleButton").OnStart(localizationUtility.GetString("Club_IsFull_Title"), localizationUtility.GetString("Request_Join_Club_IsFull_Description"), localizationUtility.GetString("btn_ok"), localizationUtility.GetString("Btn_Cancel"), delegate(bool success)
					{
						SingletonClass<MySceneManager>.Get().Close();
						if (success)
						{
							JoinClubRequest();
						}
					});
				}
				else
				{
					ShowClubIsFull();
				}
			}
			else
			{
				JoinClubRequest();
			}
		}
	}
}
