using Nightingale.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class QuestUpcomingUI : QuestBaseUI
	{
		public Sprite DoubleBuyStepSprite;

		public Sprite DoubleTreauseSprite;

		public DateTime StartTime
		{
			get;
			protected set;
		}

		public void SetInfo(QuestConfig config)
		{
			SetConfigInfo(config);
			DescriptionLabel.text = config.GetDescriptionInBox();
			StartTime = config.GetStartTime();
			DelayDo(delegate
			{
				RemainLabel.SetRemainTime(StartTime);
				RemainLabel.OnCompleted.AddListener(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
				});
				UnityEngine.Object.FindObjectOfType<QuestsScene>().UpdateUI();
			});
		}

		public void SetActiveInfo(DayActivityConfig config)
		{
			RewardLabelUI.SetImage((config.Type != 0) ? DoubleTreauseSprite : DoubleBuyStepSprite);
			DescriptionLabel.text = config.GetDescriptionInBox();
			StartTime = config.Time;
			DelayDo(delegate
			{
				RemainLabel.SetRemainTime(StartTime);
				RemainLabel.OnCompleted.AddListener(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
				});
				UnityEngine.Object.FindObjectOfType<QuestsScene>().UpdateUI();
			});
		}

		public void SetActiveInfoInPlaying(DayActivityConfig config)
		{
			RewardLabelUI.SetImage((config.Type != 0) ? DoubleTreauseSprite : DoubleBuyStepSprite);
			DescriptionLabel.text = config.GetDescription();
			StartTime = config.Time.AddDays(1.0);
			base.transform.Find("btn_go").HasVaule(delegate(Transform button)
			{
				button.GetComponent<Button>().onClick.AddListener(delegate
				{
					JoinPlayHelper.JoinPlayByQuest(PlayData.Get().GetMaxPlayScheduleData());
				});
			});
			DelayDo(delegate
			{
				RemainLabel.SetRemainTime(StartTime);
				RemainLabel.OnCompleted.AddListener(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
				});
				UnityEngine.Object.FindObjectOfType<QuestsScene>().UpdateUI();
			});
		}
	}
}
