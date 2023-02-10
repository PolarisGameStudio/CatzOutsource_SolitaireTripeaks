using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class JackpotUI : MonoBehaviour
	{
		public Text NickNameLabel;

		public Text AgoLabel;

		public FriendAvaterUI _FriendAvaterUI;

		public Button GoodButton;

		public Image FillImage;

		private JackpotUser user;

		private void Start()
		{
			FillImage.fillAmount = (float)AuxiliaryData.Get().JackpotNumber / 5f;
			base.gameObject.SetActive(value: false);
			GoodButton.onClick.AddListener(GoodButtonClick);
			GoodButton.interactable = false;
			SingletonBehaviour<JackpotUtility>.Get().JackpotUserChanged.AddListener(UpdateJackpot);
			SingletonBehaviour<JackpotUtility>.Get().DownloadJackpot();
		}

		private void GoodButtonClick()
		{
			AuxiliaryData.Get().JackpotNumber++;
			if (AuxiliaryData.Get().JackpotNumber >= 5)
			{
				AuxiliaryData.Get().JackpotNumber = 0;
				SessionData.Get().PutCommodity(BoosterType.FreeSlotsPlay, CommoditySource.Free, 1L);
				PurchasingCommodity purchasingCommodity = new PurchasingCommodity();
				purchasingCommodity.boosterType = BoosterType.FreeSlotsPlay;
				purchasingCommodity.count = 1;
				TipPopupIconNumberScene.ShowPurchasingCommodity(purchasingCommodity, doubleCommodity: false);
			}
			GoodButton.interactable = false;
			AuxiliaryData.Get().JackpotId = user.JackpotId;
			FillImage.fillAmount = (float)AuxiliaryData.Get().JackpotNumber / 5f;
		}

		private void UpdateJackpot(JackpotUser user)
		{
			this.user = user;
			GoodButton.interactable = !user.JackpotId.Equals(AuxiliaryData.Get().JackpotId);
			base.gameObject.SetActive(value: true);
			RectTransform target = base.transform as RectTransform;
			float num = 0.5625f * (float)Screen.width;
			float num2 = 1f;
			if (num < (float)Screen.height)
			{
				num2 *= (float)Screen.height / num;
			}
			Sequence s = DOTween.Sequence();
			s.Append(target.DOAnchorPosY(0f, 0.3f));
			s.Append(target.DOLocalMoveY(70f, 0.1f));
			NickNameLabel.text = user.NickName;
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(user.JackpotTime);
			if (timeSpan.TotalHours >= 1.0)
			{
				AgoLabel.text = string.Format(LocalizationUtility.Get().GetString("{0} hours ago"), (int)timeSpan.TotalHours);
			}
			else
			{
				AgoLabel.text = string.Format(LocalizationUtility.Get().GetString("{0} min ago"), (int)timeSpan.TotalMinutes);
			}
			_FriendAvaterUI.SetUser(user.SocailId, user.SocialPlatform, user.AvaterId);
		}

		public void OnClose()
		{
			SingletonBehaviour<JackpotUtility>.Get().JackpotUserChanged.RemoveListener(UpdateJackpot);
			RectTransform target = base.transform as RectTransform;
			float num = 0.5625f * (float)Screen.width;
			float num2 = 1f;
			if (num < (float)Screen.height)
			{
				num2 *= (float)Screen.height / num;
			}
			Sequence s = DOTween.Sequence();
			s.Append(target.DOAnchorPosY(0f, 0.1f));
			s.Append(target.DOLocalMoveY(960f, 0.3f));
		}
	}
}
