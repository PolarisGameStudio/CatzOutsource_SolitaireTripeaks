using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class QuestUI : QuestBaseUI
	{
		public Image StyleBackground;

		public Button Button;

		public Text ButtonLabel;

		public ProgressBarUI _ProgressBarUI;

		public Text ScheduleLabel;

		public GameObject _NewObject;

		private QuestInfo questInfo;

		private UnityAction<QuestStyle> unityAction;

		public void UpdateUI(QuestInfo questInfo, UnityAction<QuestStyle> unityAction)
		{
			this.questInfo = questInfo;
			questInfo.Viewd = true;
			this.unityAction = unityAction;
			_ProgressBarUI.gameObject.SetActive(value: true);
			_ProgressBarUI.SetFillAmount(0f);
			_NewObject.SetActive(value: false);
			SetConfigInfo(questInfo.Config);
			switch (questInfo.QuestStyle)
			{
			default:
				StyleBackground.color = Color.white;
				break;
			case QuestStyle.Bouns:
				StyleBackground.color = new Color32(143, byte.MaxValue, 181, byte.MaxValue);
				break;
			case QuestStyle.Event:
				StyleBackground.color = new Color32(byte.MaxValue, 183, 119, byte.MaxValue);
				break;
			}
			RemainLabel.gameObject.SetActive(value: false);
			if (questInfo.IsComplete())
			{
				ButtonLabel.text = LocalizationUtility.Get("Localization_quest.json").GetString("btn_Collect");
				SingletonBehaviour<GlobalConfig>.Get().CreateExclamationMark(Button.gameObject, number: true, left: false, 0.8f, 0f, 0f);
			}
			else
			{
				ButtonLabel.text = LocalizationUtility.Get("Localization_quest.json").GetString("btn_Go");
			}
			Button.transform.localScale = Vector2.zero;
			base.transform.localScale = new Vector3(0f, 1f, 1f);
			base.transform.DOScaleX(1f, 0.3f).OnComplete(delegate
			{
				Button.transform.DOScale(1f, 0.2f);
				if (questInfo.IsInvalid())
				{
					InvalidAnmitor();
				}
				else
				{
					if (!questInfo.IsComplete())
					{
						RemainLabel.gameObject.SetActive(value: true);
						RemainLabel.SetRemainTime(new DateTime(questInfo.Ticks));
						RemainLabel.OnCompleted.AddListener(InvalidAnmitor);
					}
					float num = questInfo.GetStep() * 1f;
					AudioUtility.GetSound().PlayLoop("Audios/loop_coin.mp3", num);
					_ProgressBarUI.UpdateFillAmount(questInfo.GetStep(), num, delegate
					{
						Button.onClick.AddListener(delegate
						{
							if (this.questInfo.IsComplete())
							{
								CompleteAnmitor();
							}
							else
							{
								ScheduleData schedule = PlayData.Get().GetMaxPlayScheduleData();
								if (questInfo.Config.QuestType == QuestType.WinGameInScene)
								{
									schedule = questInfo.Config.ScheduleData;
								}
								JoinPlayHelper.JoinPlayByQuest(schedule);
							}
						});
					});
				}
			});
			ScheduleLabel.text = questInfo.GetStepString();
		}

		public void SetNewObject()
		{
			_NewObject.SetActive(value: true);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(UnityEngine.Random.Range(0.2f, 0.4f));
			sequence.Append(_NewObject.transform.DOScale(1.2f, 0.12f));
			sequence.Append(_NewObject.transform.DOScale(1.1f, 0.09f));
			sequence.Append(_NewObject.transform.DOScale(1.2f, 0.09f));
			sequence.Append(_NewObject.transform.DOScale(1f, 0.09f));
			sequence.SetLoops(-1);
		}

		private void CompleteAnmitor(bool success = false)
		{
			int num = (!success) ? 1 : 2;
			AchievementData.Get().DoAchievement(AchievementType.Quest);
			QuestData.Get().RemoveQuest(questInfo);
			SessionData.Get().PutCommodity(questInfo.Config.BoosterType, CommoditySource.Free, questInfo.Config.RewardCount * num);
			PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
			{
				new PurchasingCommodity
				{
					boosterType = questInfo.Config.BoosterType,
					count = questInfo.Config.RewardCount * num
				}
			});
			SingletonBehaviour<TripeaksLogUtility>.Get().UploadEvent(questInfo);
			AudioUtility.GetSound().Play("Audios/QuestCompleted.mp3");
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "Particles/ConfettiBlastRainbow"));
			gameObject.transform.position = base.transform.position;
			UnityEngine.Object.Destroy(gameObject, 2f);
			Button.onClick.RemoveAllListeners();
			base.transform.DOScaleX(0f, 0.2f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
				if (unityAction != null)
				{
					unityAction(questInfo.QuestStyle);
				}
			});
		}

		private void InvalidAnmitor()
		{
			QuestData.Get().RemoveQuest(questInfo);
			RemainLabel.gameObject.SetActive(value: true);
			RemainLabel.Label.text = LocalizationUtility.Get("Localization_quest.json").GetString("ui_time_up");
			Sequence sequence = DOTween.Sequence();
			sequence.Append(RemainLabel.Label.transform.DOShakeRotateZ(10f, 10, 1f));
			sequence.AppendInterval(0.5f);
			sequence.OnComplete(delegate
			{
				base.transform.DOScaleX(0f, 0.2f).OnComplete(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
					if (unityAction != null)
					{
						unityAction(questInfo.QuestStyle);
					}
				});
			});
		}
	}
}
