using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Socials;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OverInvitePop : MonoBehaviour
	{
		private RectTransform rectTransform;

		public GameObject LoginGameObject;

		public GameObject InviteGameObject;

		public FriendAvaterUI FriendAvaterUI;

		public Text NameLabel;

		private FacebookUser facebookUser;

		private string id = string.Empty;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			RectTransform obj = rectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			obj.anchoredPosition = new Vector2(anchoredPosition.x, 213f);
			UpdateInvitePopUI(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.AddListener(UpdateInvitePopUI);
		}

		private void UpdateInvitePopUI(bool login)
		{
			if (login)
			{
				LoginGameObject.SetActive(value: false);
				InviteGameObject.SetActive(value: true);
				rectTransform.DOAnchorPosY(215f, 0.3f);
			}
			else
			{
				LoginGameObject.SetActive(value: true);
				InviteGameObject.SetActive(value: false);
				OpenAnimator();
			}
		}

		private void OnDestroy()
		{
			SingletonBehaviour<FacebookMananger>.Get().LoginChanged.RemoveListener(UpdateInvitePopUI);
		}

		public void CloseAnimator()
		{
			Sequence s = DOTween.Sequence();
			s.Append(rectTransform.DOAnchorPosY(347f, 0.2f));
			s.Append(rectTransform.DOAnchorPosY(753f, 0.3f));
		}

		public void OpenAnimator()
		{
			Sequence s = DOTween.Sequence();
			s.Append(rectTransform.DOAnchorPosY(-193f, 0.3f));
			s.Append(rectTransform.DOAnchorPosY(-156f, 0.2f));
		}

		public void InviteFriend()
		{
			id = facebookUser.id;
			SingletonBehaviour<FacebookMananger>.Get().AppRequest(InviteRequest, LocalizationUtility.Get("Localization_facebook.json").GetString("fb_invite_friend"), new List<string>
			{
				facebookUser.id
			}, "Invite", "Pyramid Solitaire");
		}

		public void LoginFacebook()
		{
			SingletonBehaviour<GlobalConfig>.Get().ShowLoginOrInvitable(AuxiliaryData.Get().IsFacebookReward);
		}

		private void DownloadFriends(List<FacebookUser> users, int number)
		{
			if (users.Count != 0)
			{
				OpenAnimator();
				facebookUser = users[Random.Range(0, users.Count)];
				NameLabel.text = facebookUser.name;
				FriendAvaterUI.SetUser(facebookUser);
			}
		}

		private void InviteRequest(int number)
		{
			if (number > 0)
			{
				AchievementData.Get().DoAchievement(AchievementType.InviteFriend);
				AuxiliaryData.Get().PutRequestInvitableFriends(new List<string>
				{
					id
				});
				UpdateInvitePopUI(SingletonBehaviour<FacebookMananger>.Get().IsLogin());
			}
		}
	}
}
