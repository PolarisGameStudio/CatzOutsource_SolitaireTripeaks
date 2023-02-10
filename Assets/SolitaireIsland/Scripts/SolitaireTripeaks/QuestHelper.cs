using Nightingale.Localization;
using Nightingale.Notifications;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	public class QuestHelper : SingletonClass<QuestHelper>
	{
		public bool HasQuest()
		{
			List<QuestConfig> quests = QuestConfigGroup.Get().GetQuests();
			if (!QuestData.Get().QuestOpen)
			{
				return false;
			}
			if (QuestData.Get().quests.Count > 0)
			{
				return true;
			}
			foreach (QuestConfig item in quests)
			{
				if (!QuestData.Get().Contains(item.identifier) && item.IsStart() && !item.IsInvalid())
				{
					return true;
				}
			}
			if (QuestData.Get().PushDailyQuest(QuestStyle.Daily))
			{
				return true;
			}
			if (QuestData.Get().PushDailyQuest(QuestStyle.Bouns))
			{
				return true;
			}
			return false;
		}

		public void UpdateQuestReceives()
		{
			List<QuestConfig> quests = QuestConfigGroup.Get().GetQuests();
			List<string> list = new List<string>();
			if (quests.Count > 0)
			{
				list.AddRange((from e in quests
					select e.identifier).ToArray());
			}
			QuestData.Get().UpdateQuestReceives(list);
		}

		public QuestConfig GetQuestConfig(string identifier)
		{
			List<QuestConfig> quests = QuestConfigGroup.Get().GetQuests();
			List<QuestConfig> list = new List<QuestConfig>();
			list.AddRange(quests);
			return list.Find((QuestConfig e) => e.identifier.Equals(identifier));
		}

		public QuestInfo TryAppendOnceQuests(QuestStyle style)
		{
			if (QuestData.Get().PushDailyQuest(style))
			{
				if (QuestData.Get().Contains(style))
				{
					return null;
				}
				return QuestData.Get().AppendQuest(style, DailyConfigUtility.Get(style).RandomQuestConfig());
			}
			return null;
		}

		public List<QuestInfo> TryAppendQuests()
		{
			List<QuestInfo> datas = new List<QuestInfo>();
			if (!QuestData.Get().QuestOpen)
			{
				return datas;
			}
			Func<QuestInfo, bool> func = delegate(QuestInfo info)
			{
				if (info == null)
				{
					return true;
				}
				datas.Add(info);
				return false;
			};
			while (!func(TryAppendOnceQuests(QuestStyle.Bouns)))
			{
			}
			while (!func(TryAppendOnceQuests(QuestStyle.Daily)))
			{
			}
			return datas;
		}

		public void DoQuest(QuestType questType, int questIndex = 1)
		{
			DoQuest(questType, new ScheduleData(questIndex));
		}

		public void DoQuest(QuestType questType, ScheduleData scheduleData)
		{
			List<QuestInfo> quests = QuestData.Get().quests;
			foreach (QuestInfo item in quests)
			{
				if (item.Config.QuestType == questType)
				{
					item.DoQuest(scheduleData);
				}
			}
		}

		public void OpenNotification()
		{
			string @string = LocalizationUtility.Get("Localization_Notification.json").GetString("EventQuest_Notification_Title");
			string string2 = LocalizationUtility.Get("Localization_Notification.json").GetString("EventQuest_Notification_Message");
			List<QuestConfig> nextQuests = QuestConfigGroup.Get().GetNextQuests();
			foreach (QuestConfig item in nextQuests)
			{
				if (!item.IsInvalid() && !item.IsStart())
				{
					DateTime dateTime = DateTime.Parse(item.StartTime);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 0, 0);
					string arg = $"{item.RewardCount} {AppearNodeConfig.Get().GetBoosterQuestTitle(item.BoosterType)}";
					LocalNotification.NotificationMessage(@string, string.Format(string2, arg), dateTime.Subtract(DateTime.Now));
				}
			}
		}
	}
}
