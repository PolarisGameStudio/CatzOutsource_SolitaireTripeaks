using DG.Tweening;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ActiveViewUI : MonoBehaviour
	{
		public Text NumberLabel;

		public Text RemainTimeLabel;

		public Image Icon;

		public Image MiniIcon;

		public Button FindButton;

		public void Show()
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().IsActive() && SpecialActivityData.Get().ScheduleDatas.Count > 0)
			{
				NumberLabel.text = $"{SpecialActivityData.Get().Numbers}";
				Icon.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetSprite();
				Icon.SetNativeSize();
				MiniIcon.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetMiniSprite();
				MiniIcon.SetNativeSize();
				SingletonBehaviour<GlobalConfig>.Get().CreateNumber(FindButton.gameObject, 0.8f, SpecialActivityData.Get().ScheduleDatas.Count, left: false, -10f, -10f);
				FindButton.onClick.AddListener(delegate
				{
					JoinPlayHelper.JoinPlayByQuest(SpecialActivityData.Get().RandomScheduleData());
				});
				InvokeRepeating("RepeatingUpdate", 0f, 1f);
				RectTransform target = base.transform as RectTransform;
				Sequence s = DOTween.Sequence();
				s.Append(target.DOAnchorPosX(110f, 0.5f));
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public void Hide()
		{
			RectTransform target = base.transform as RectTransform;
			Sequence s = DOTween.Sequence();
			s.Append(target.DOAnchorPosX(-300f, 0.5f));
		}

		private void RepeatingUpdate()
		{
			RemainTimeLabel.text = SpecialActivityConfig.Get().GetEndTime().Subtract(DateTime.Now)
				.TOString();
		}
	}
}
