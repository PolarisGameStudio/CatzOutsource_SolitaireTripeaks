using DG.Tweening;
using Nightingale.Localization;
using Nightingale.Notifications;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class SpecialActivityUtility : SingletonBehaviour<SpecialActivityUtility>
	{
		public UnityEvent OnRefresh = new UnityEvent();

		public const string ActivityId = "Special_Active";

		private void Refresh(bool force)
		{
			SpecialActivityData.Get().SetNewEvent(SpecialActivityConfig.Get().StartTime);
			if (!force && SpecialActivityData.Get().ScheduleDatas.Count >= 10 && SpecialActivityData.Get().ScheduleDatas.Count != 0)
			{
				return;
			}
			float num = (float)DateTime.Now.Subtract(new DateTime(SpecialActivityData.Get().Ticks)).TotalMinutes;
			int refreshCd = GetRefreshCd();
			if (num >= (float)refreshCd)
			{
				int num2 = Mathf.FloorToInt(num / (float)refreshCd);
				List<ScheduleData> playScheduleDatas = PlayData.Get().GetPlayScheduleDatas();
				playScheduleDatas.RemoveAll((ScheduleData e) => SpecialActivityData.Get().ScheduleDatas.Contains(e));
				while (num2 > 0 && playScheduleDatas.Count > 0 && SpecialActivityData.Get().ScheduleDatas.Count < 10)
				{
					num2--;
					ScheduleData item = playScheduleDatas[UnityEngine.Random.Range(0, playScheduleDatas.Count)];
					playScheduleDatas.Remove(item);
					SpecialActivityData.Get().ScheduleDatas.Add(item);
					SpecialActivityData.Get().Put();
				}
				OnRefresh.Invoke();
			}
		}

		private int GetRefreshCd()
		{
			int b = 11 - PlayData.Get().GetPlayChapterNumbers();
			return Mathf.Max(5, b);
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				CheckSpecialActivity();
			}
		}

		public bool IsActive()
		{
			return SpecialActivityConfig.Get().IsActive();
		}

		public Sprite GetSprite()
		{
			return SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("Activity/Icon");
		}

		public Sprite GetMiniSprite()
		{
			return SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("Activity/Small");
		}

		public Sprite GetSaleButtonSprite()
		{
			return SingletonBehaviour<LoaderUtility>.Get().GetAsset<Sprite>("Activity/SaleButton");
		}

		public bool HasSpecialActivity(ScheduleData schedule)
		{
			if (IsActive())
			{
				return SpecialActivityData.Get().ScheduleDatas.Contains(schedule);
			}
			return false;
		}

		public bool HasSpecialActivity(int world, int chapter)
		{
			if (IsActive())
			{
				return SpecialActivityData.Get().ScheduleDatas.Count((ScheduleData e) => e.world == world && e.chapter == chapter) > 0;
			}
			return false;
		}

		public bool HasSpecialActivity(int world)
		{
			if (IsActive())
			{
				return SpecialActivityData.Get().ScheduleDatas.Count((ScheduleData e) => e.world == world) > 0;
			}
			return false;
		}

		public void CheckSpecialActivity()
		{
			if (SpecialActivityConfig.Get().IsActive())
			{
				Refresh(force: false);
			}
		}

		public void CreateSpecialActivityInLevelCompleted(ScheduleData schedule, Transform transform)
		{
			if (HasSpecialActivity(schedule))
			{
				GameObject g = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/SpecialActivityCollectUI"));
				ImageUI component = g.GetComponent<ImageUI>();
				component.SetImage(GetMiniSprite());
				component.SetLabel("X1");
				g.transform.SetParent(transform, worldPositionStays: false);
				g.transform.localPosition = Vector3.zero;
				g.transform.localScale = Vector3.zero;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(g.transform.DOScale(3f, 0.3f));
				sequence.Append(g.transform.DOShakeRotateZ(40f, 4, 1.2f));
				sequence.Append(g.transform.DOScale(0f, 0.2f));
				sequence.OnComplete(delegate
				{
					UnityEngine.Object.Destroy(g);
				});
				SpecialActivityData.Get().Collect();
				Refresh(force: true);
				SpecialActivityData.Get().ScheduleDatas.Remove(schedule);
			}
		}

		public bool CreateSpecialActivityInDetail(ScheduleData schedule, Transform transform)
		{
			if (HasSpecialActivity(schedule))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/SpecialActivityCollectUI"));
				ImageUI component = gameObject.GetComponent<ImageUI>();
				component.SetImage(GetMiniSprite());
				component.SetLabel(string.Empty);
				gameObject.transform.SetParent(transform, worldPositionStays: false);
				gameObject.transform.localPosition = new Vector3(63f, 13f, 0f);
				gameObject.transform.SetParent(transform.parent.parent, worldPositionStays: true);
				gameObject.transform.SetSiblingIndex(1);
				Sequence sequence = DOTween.Sequence();
				sequence.AppendInterval(UnityEngine.Random.Range(0.2f, 0.4f));
				sequence.Append(gameObject.transform.DOScale(1.2f, 0.12f));
				sequence.Append(gameObject.transform.DOScale(1.1f, 0.09f));
				sequence.Append(gameObject.transform.DOScale(1.2f, 0.09f));
				sequence.Append(gameObject.transform.DOScale(1f, 0.09f));
				sequence.SetLoops(-1);
				return true;
			}
			return false;
		}

		public bool CreateSpecialActivityInSelectIsland(int world, int chapter, Transform transform)
		{
			if (HasSpecialActivity(world, chapter))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/SpecialActivityCollectUI"));
				ImageUI component = gameObject.GetComponent<ImageUI>();
				component.SetImage(GetMiniSprite());
				component.SetLabel(string.Empty);
				gameObject.transform.SetParent(transform, worldPositionStays: false);
				gameObject.transform.localPosition = new Vector3(100f, 0f, 0f);
				Sequence sequence = DOTween.Sequence();
				sequence.AppendInterval(UnityEngine.Random.Range(0.2f, 0.4f));
				sequence.Append(gameObject.transform.DOScale(1.2f, 0.12f));
				sequence.Append(gameObject.transform.DOScale(1.1f, 0.09f));
				sequence.Append(gameObject.transform.DOScale(1.2f, 0.09f));
				sequence.Append(gameObject.transform.DOScale(1f, 0.09f));
				sequence.SetLoops(-1);
				return true;
			}
			return false;
		}

		public bool CreateSpecialActivityInWorld(int world, Transform transform)
		{
			if (HasSpecialActivity(world))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/SpecialActivityCollectUI"));
				ImageUI component = gameObject.GetComponent<ImageUI>();
				component.SetImage(GetMiniSprite());
				component.SetLabel(string.Empty);
				gameObject.transform.SetParent(transform, worldPositionStays: false);
				gameObject.transform.localPosition = new Vector3(100f, 0f, 0f);
				return true;
			}
			return false;
		}

		public DateTime GetRemainLabel()
		{
			return SpecialActivityConfig.Get().GetEndTime();
		}

		public bool ShowSpecialActivitySale(UnityAction unityAction)
		{
			SingletonClass<MySceneManager>.Get().Popup<SpecialActivitySaleScene>("Activity/ActivitySale").AddClosedListener(unityAction);
			return true;
		}

		public bool ShowSpecialActivity(UnityAction unityAction)
		{
			SingletonClass<MySceneManager>.Get().Popup<SpecialActivityScene>("Activity/SpecialActivityScene").AddClosedListener(unityAction);
			return true;
		}

		public void Notification()
		{
			if (IsActive())
			{
				int count = SpecialActivityData.Get().ScheduleDatas.Count;
				if (count < 10)
				{
					DateTime dateTime = new DateTime(SpecialActivityData.Get().Ticks);
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_Notification.json");
#if ENABLE_LOCAL_NOTIFICATION
                    Nightingale.Notifications.LocalNotification.NotificationMessage(localizationUtility.GetString("Special_Activity_Notification_Title"), 
                        string.Format(localizationUtility.GetString("Special_Activity_Notification_Message"), 
                        SpecialActivityConfig.Get().Name), 
                        dateTime.AddMinutes(GetRefreshCd() * (10 - count)).Subtract(DateTime.Now));
#endif
				}
			}
		}
	}
}
