using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Socials;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Club;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ClubScene : SoundScene
	{
		public Button CloseButton;

		public Button LeaveButton;

		public Button InviteButton;

		public Button RequestJoinButton;

		public Button ModifyButton;

		public Button StoreButton;

		public Button LeaderboardButton;

		public ClubDetailUI ClubDetailUI;

		public LoopScrollRect loopScrollRect;

		public Text RequestJoinLabel;

		public GameObject LoadingGameObject;

		public TabGroup tabGroup;

		public string ClubId
		{
			get;
			private set;
		}

		public void OnStart()
		{
			tabGroup.SetVisable(visable: true);
			RequestJoinButton.gameObject.SetActive(value: false);
			LeaveButton.gameObject.SetActive(value: true);
			InviteButton.gameObject.SetActive(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			StoreButton.gameObject.SetActive(value: true);
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
			});
			StoreButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ClubStoreScene>("Scenes/ClubStoreScene");
			});
			DelayDo(delegate
			{
				if (ClubSystemData.Get().GetLeaderboardDatas().Count > 0)
				{
					SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(LeaderboardButton.gameObject, number: true);
				}
				LeaderboardButton.onClick.RemoveAllListeners();
				LeaderboardButton.onClick.AddListener(delegate
				{
					SingletonBehaviour<ClubSystemHelper>.Get().TryShowClubLeaderBoard();
					SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(LeaderboardButton.gameObject, number: false);
				});
			});
			LeaveButton.onClick.AddListener(delegate
			{
				base.IsStay = true;
				SingletonBehaviour<ClubSystemHelper>.Get().ShowLeaveClub(delegate
				{
					base.IsStay = false;
				});
			});
			InviteButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<InviteJoinClubScene>("Scenes/InviteJoinClubScene");
			});
			UpdateClub(SingletonBehaviour<ClubSystemHelper>.Get()._MyClubResponse);
			SingletonBehaviour<ClubSystemHelper>.Get().AddMyClubResponseListener(UpdateClub);
			SingletonBehaviour<ClubSystemHelper>.Get().MyClub();
		}

		public void OnClickInfo()
		{
			SingletonClass<MySceneManager>.Get().Popup<LeaderboarGuidePopup>("Scenes/ClubLeaderboarGuidePopup").OnStart(isClan: true, SingletonBehaviour<ClubSystemHelper>.Get().Segment, SingletonBehaviour<ClubSystemHelper>.Get().Rewards);
		}

		public void OnStart(Club club)
		{
			tabGroup.SetVisable(visable: false);
			ClubId = club.ClubId;
			ModifyButton.gameObject.SetActive(value: false);
			LeaveButton.gameObject.SetActive(value: false);
			InviteButton.gameObject.SetActive(value: false);
			StoreButton.gameObject.SetActive(value: false);
			LeaderboardButton.gameObject.SetActive(value: false);
			RequestJoinButton.gameObject.SetActive(club.ClubId != SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier());
			RequestJoinButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<ClubSystemHelper>.Get().ShowRequestJoinClubScene(club);
			});
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
			});
			UpdateClub(club.ClubId, club);
			SingletonBehaviour<ClubSystemHelper>.Get().AddClubListener(UpdateClub);
			SingletonBehaviour<ClubSystemHelper>.Get().GetClub(club.ClubId);
		}

		public void OnFind()
		{
			tabGroup.ShowOnce(1);
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				SingletonClass<MySceneManager>.Get().Popup<JoinClubScene>("Scenes/JoinClubScene");
			});
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveMyClubResponseListener(UpdateClub);
			SingletonBehaviour<ClubSystemHelper>.Get().RemoveClubListener(UpdateClub);
		}

		private void UpdateClub(string id, Club club)
		{
			if (!(ClubId != id) && club != null)
			{
				ClubDetailUI.SetInfo(club);
				UpdateClubMembers(club);
				if (club.Private)
				{
					RequestJoinLabel.text = LocalizationUtility.Get("Localization_Club.json").GetString("btn_Request");
				}
				else
				{
					RequestJoinLabel.text = LocalizationUtility.Get("Localization_Club.json").GetString("btn_Join_Now");
				}
			}
		}

		private void UpdateClubMembers(Club club)
		{
			if (club != null && club.Members != null)
			{
				List<Member> list = (from e in club.Members.ToList()
					orderby e.LeaderboardScore descending
					select e).ToList();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Rank = i + 1;
				}
				Member[] arrays = list.ToArray();
				int index = list.FindIndex((Member e) => e.PlayerId.Equals(SolitaireTripeaksData.Get().GetPlayerId())) - 2;
				loopScrollRect.objectsToFill = arrays;
				loopScrollRect.totalCount = arrays.Length;
				loopScrollRect.RefreshCells();
				LoopDelayDo(delegate
				{
					if (loopScrollRect.gameObject.activeInHierarchy)
					{
						if (LoadingGameObject.activeSelf)
						{
							if (index > arrays.Length - 5)
							{
								loopScrollRect.RefillCellsFromEnd();
							}
							else
							{
								loopScrollRect.RefillCells((index >= 0) ? index : 0);
							}
						}
						LoadingGameObject.SetActive(value: false);
						return false;
					}
					return true;
				}, null);
			}
		}

		private void UpdateClub(MyClubResponse response)
		{
			if (response != null)
			{
				if (response.Club == null)
				{
					SingletonBehaviour<ClubSystemHelper>.Get().ShowNotInClub();
					return;
				}
				ClubId = response.Club.ClubId;
				ModifyButton.gameObject.SetActive(SingletonBehaviour<ClubSystemHelper>.Get().GetRoleInClub(response.Club) == ClubRoles.Chairman);
				ClubDetailUI.SetInfo(response.Club);
				UpdateClubMembers(response.Club);
				ModifyButton.onClick.RemoveAllListeners();
				ModifyButton.onClick.AddListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Popup<CreatorClubScene>("Scenes/CreatorClubScene").OnStart(response.Club);
				});
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(StoreButton.gameObject, SingletonBehaviour<ClubSystemHelper>.Get().HasSuperTreasure() == SuperTreasure.Normal);
			}
		}
	}
}
