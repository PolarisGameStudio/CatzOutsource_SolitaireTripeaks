using DG.Tweening;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class QuestsScene : BaseScene
	{
		public GameObject _ClosedButton;

		public GameObject _UpcomingQuestObject;

		public GameObject _NoQuestObject;

		public RectTransform _QuestRectTransform;

		private UnityAction unityAction;

		private bool hasGoButton;

		public static void TryShow(UnityAction unityAction)
		{
			if (SingletonClass<QuestHelper>.Get().HasQuest())
			{
				ShowQuest(hasGoButton: false, unityAction);
				return;
			}
			unityAction?.Invoke();
			SingletonBehaviour<SynchronizeUtility>.Get().UploadGameData();
			SolitaireTripeaksData.Get().FlushData();
		}

		public static void ShowQuest(bool hasGoButton, UnityAction unityAction = null)
		{
			if (QuestData.Get().Contains(DateTime.Today.ToString("yyyy/M/d")))
			{
				SingletonClass<MySceneManager>.Get().Popup<QuestsScene>("Scenes/QuestsScene").OnStart(hasGoButton, unityAction);
				return;
			}
			EventConfig dailyEvent = QuestConfigGroup.Get().GetDailyEvent();
			if (dailyEvent == null)
			{
				SingletonClass<MySceneManager>.Get().Popup<QuestsScene>("Scenes/QuestsScene").OnStart(hasGoButton, unityAction);
			}
			else if (dailyEvent.GetConfigs(DateTime.Now).Count <= 1)
			{
				QuestData.Get().quests.RemoveAll((QuestInfo e) => e.QuestStyle == QuestStyle.Event);
				QuestData.Get().AppendQuest(QuestStyle.Event, dailyEvent.GetConfig(DateTime.Now));
				SingletonClass<MySceneManager>.Get().Popup<QuestsScene>("Scenes/QuestsScene").OnStart(hasGoButton, unityAction);
			}
			else
			{
				SingletonClass<MySceneManager>.Get().Popup<MakeAChoicePopup>("Scenes/EventPopup/MakeAChoicePopup").AddClosedListener(delegate
				{
					SingletonClass<MySceneManager>.Get().Popup<QuestsScene>("Scenes/QuestsScene").OnStart(hasGoButton, unityAction);
				});
			}
		}

		public static bool IsOpen()
		{
			return PlayData.Get().HasThanLevelData(0, 0, 11);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(QuestsScene).Name);
		}

		private void OnStart(bool hasGoButton, UnityAction unityAction = null)
		{
			base.IsStay = true;
			if (!QuestData.Get().QuestOpen)
			{
				QuestData.Get().QuestOpen = true;
				QuestData.Get().AppendQuest(QuestStyle.Daily, DailyConfigUtility.Get(QuestStyle.Daily).QuestEasyConfig());
			}
			this.unityAction = unityAction;
			this.hasGoButton = hasGoButton;
			List<QuestInfo> list = QuestData.Get().quests.ToList();
			SingletonClass<QuestHelper>.Get().UpdateQuestReceives();
			SingletonClass<QuestHelper>.Get().TryAppendQuests();
			foreach (QuestInfo item in from e in QuestData.Get().quests
				orderby e.QuestStyle
				select e)
			{
				QuestUI component = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "UI/QuestUI")).GetComponent<QuestUI>();
				component.transform.SetParent(_QuestRectTransform, worldPositionStays: false);
				component.transform.SetSiblingIndex(0);
				component.UpdateUI(item, TryRefQuests);
				if (!list.Contains(item) && item.QuestStyle == QuestStyle.Event)
				{
					component.SetNewObject();
				}
			}
			foreach (QuestConfig quest in QuestConfigGroup.Get().GetQuests())
			{
				if (!quest.IsStart() && !quest.IsInvalid())
				{
					QuestUpcomingUI component2 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "UI/QuestUpcomingUI")).GetComponent<QuestUpcomingUI>();
					component2.transform.SetParent(_QuestRectTransform, worldPositionStays: false);
					component2.SetInfo(quest);
				}
			}
			foreach (DayActivityConfig dayActivityConfig in SingletonClass<DayActivityHelper>.Get().GetDayActivityConfigs())
			{
				if (!dayActivityConfig.IsInvalid() && !dayActivityConfig.IsRunning())
				{
					QuestUpcomingUI component3 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "UI/QuestUpcomingUI")).GetComponent<QuestUpcomingUI>();
					component3.transform.SetParent(_QuestRectTransform, worldPositionStays: false);
					component3.SetActiveInfo(dayActivityConfig);
				}
			}
			(from e in _QuestRectTransform.GetComponentsInChildren<QuestUpcomingUI>()
				orderby e.StartTime
				select e).ToList().ForEach(delegate(QuestUpcomingUI e)
			{
				e.transform.SetAsLastSibling();
			});
			_ClosedButton.transform.localScale = Vector2.zero;
			_ClosedButton.transform.DOScale(1f, 0.2f).SetDelay((!hasGoButton) ? 1f : 0.2f);
			foreach (DayActivityConfig dayActivityConfig2 in SingletonClass<DayActivityHelper>.Get().GetDayActivityConfigs())
			{
				if (dayActivityConfig2.IsRunning())
				{
					QuestUpcomingUI component4 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "UI/QuestUpcomingUI2")).GetComponent<QuestUpcomingUI>();
					component4.transform.SetParent(_QuestRectTransform, worldPositionStays: false);
					component4.transform.SetSiblingIndex(0);
					component4.SetActiveInfoInPlaying(dayActivityConfig2);
				}
			}
			UpdateUI();
		}

		public void OnCloseClick()
		{
			if (hasGoButton)
			{
				SingletonClass<MySceneManager>.Get().Close();
			}
			else
			{
				SingletonBehaviour<SynchronizeUtility>.Get().UploadGameData();
			}
			if (unityAction != null)
			{
				unityAction();
			}
			SolitaireTripeaksData.Get().FlushData();
			SetCanvasGraphicRaycaster(enabled: false);
		}

		private void TryRefQuests(QuestStyle style)
		{
			QuestInfo questInfo = SingletonClass<QuestHelper>.Get().TryAppendOnceQuests(style);
			if (questInfo != null)
			{
				QuestUI component = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(QuestsScene).Name, "UI/QuestUI")).GetComponent<QuestUI>();
				component.transform.SetParent(_QuestRectTransform, worldPositionStays: false);
				component.transform.SetSiblingIndex(_UpcomingQuestObject.transform.GetSiblingIndex() - 1);
				component.UpdateUI(questInfo, TryRefQuests);
				if (questInfo.QuestStyle == QuestStyle.Event)
				{
					component.SetNewObject();
				}
			}
			UpdateUI();
		}

		public void UpdateUI()
		{
			DelayDo(delegate
			{
				if (_QuestRectTransform.childCount == 1)
				{
					_NoQuestObject.SetActive(value: true);
					_UpcomingQuestObject.SetActive(value: false);
				}
				else
				{
					_UpcomingQuestObject.SetActive(_QuestRectTransform.GetComponentInChildren<QuestUpcomingUI>() != null);
					_NoQuestObject.SetActive(value: false);
				}
			});
		}
	}
}
