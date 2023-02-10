using Nightingale.Localization;
using Nightingale.U2D;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class FriendViewUI : MonoBehaviour
	{
		public Button SendButton;

		public Transform ContentTransform;

		public LocalizationLabel SelectedLabel;

		public DoubleSpriteButton SelectAllButton;

		public LoopVerticalScrollRect scroll;

		public GameObject LoadingGameObject;

		public GameObject NoFriendsGameObject;

		private List<TripeaksPlayerInView> Friends;

		private void Awake()
		{
			NoFriendsGameObject.SetActive(value: false);
			LoadingGameObject.SetActive(value: true);
			SelectedLabel.SetText(0);
		}

		public void PutFriend(List<TripeaksPlayerInView> friends, UnityAction<List<TripeaksPlayerInView>> unityAction)
		{
			Friends = friends;
			LoadingGameObject.SetActive(value: false);
			if (Friends == null || Friends.Count == 0)
			{
				NoFriendsGameObject.SetActive(value: true);
			}
			SelectAllButton.RemoveAllListeners();
			SendButton.onClick.RemoveAllListeners();
			scroll.objectsToFill = friends.ToArray();
			scroll.totalCount = friends.Count;
			scroll.RefreshCells();
			scroll.RefillCells();
			if (friends != null && friends.Count != 0)
			{
				SelectAllButton.AddListener(delegate
				{
					if (friends.Count((TripeaksPlayerInView e) => !e.IsSelect && !e.IsWait) == 0)
					{
						friends.ForEach(delegate(TripeaksPlayerInView e)
						{
							e.IsSelect = false;
						});
					}
					else
					{
						friends.ForEach(delegate(TripeaksPlayerInView e)
						{
							e.IsSelect = true;
						});
					}
					UpdateViewContent();
				});
				SendButton.onClick.AddListener(delegate
				{
					List<TripeaksPlayerInView> list = (from e in friends
						where e.IsSelect && !e.IsWait
						select e).ToList();
					if (list.Count == 0)
					{
						TipPopupNoIconScene.ShowNoSelectFriends();
					}
					else if (unityAction != null)
					{
						unityAction(list);
					}
				});
				UpdateViewContent();
			}
		}

		public void UpdateViewContent()
		{
			List<FacebookFriendUI> list = ContentTransform.GetComponentsInChildren<FacebookFriendUI>().ToList();
			list.ForEach(delegate(FacebookFriendUI e)
			{
				e.UpdateState();
			});
			if (Friends != null && Friends.Count != 0)
			{
				SelectAllButton.SetState(Friends.Count((TripeaksPlayerInView e) => !e.IsSelect && !e.IsWait) == 0);
				SelectedLabel.SetText(Friends.Count((TripeaksPlayerInView e) => e.IsSelect && !e.IsWait));
			}
		}
	}
}
