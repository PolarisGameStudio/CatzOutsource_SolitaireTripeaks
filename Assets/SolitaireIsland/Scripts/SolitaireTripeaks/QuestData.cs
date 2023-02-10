using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	[Serializable]
	public class QuestData
	{
		public List<QuestInfo> quests;

		public List<string> receives;

		public int QuestCount;

		public int QuestBonusCount;

		public bool QuestOpen;

		public QuestData()
		{
			quests = new List<QuestInfo>();
			receives = new List<string>();
		}

		public static QuestData Get()
		{
			if (SolitaireTripeaksData.Get().Quest == null)
			{
				SolitaireTripeaksData.Get().Quest = new QuestData();
			}
			return SolitaireTripeaksData.Get().Quest;
		}

		public int GetNumber()
		{
			return quests.Count((QuestInfo e) => e.IsComplete() || !e.Viewd);
		}

		public bool PushDailyQuest(QuestStyle style)
		{
			if (style == QuestStyle.Daily)
			{
				return QuestCount < SingletonBehaviour<ClubSystemHelper>.Get().GetDailyLimit();
			}
			return QuestBonusCount < 1;
		}

		public void RestbyNewDay()
		{
			QuestCount = quests.Count((QuestInfo e) => e.QuestStyle == QuestStyle.Daily && !e.IsInvalid());
			QuestBonusCount = quests.Count((QuestInfo e) => e.QuestStyle == QuestStyle.Bouns && !e.IsInvalid());
		}

		public bool Contains(string key)
		{
			return receives.Contains(key);
		}

		public bool Contains(QuestStyle style)
		{
			return quests.Find((QuestInfo e) => e.QuestStyle == style) != null;
		}

		public QuestInfo AppendQuest(QuestStyle style, QuestConfig config)
		{
			switch (style)
			{
			case QuestStyle.Daily:
				QuestCount++;
				break;
			case QuestStyle.Bouns:
				QuestBonusCount++;
				break;
			}
			QuestInfo questInfo = new QuestInfo(style, config);
			quests.Add(questInfo);
			receives.Add(config.identifier);
			SingletonBehaviour<TripeaksLogUtility>.Get().UploadEvent(questInfo);
			return questInfo;
		}

		public void RemoveQuest(QuestInfo data)
		{
			if (data.QuestStyle == QuestStyle.Daily || data.QuestStyle == QuestStyle.Bouns)
			{
				receives.Remove(data.identifier);
			}
			quests.Remove(data);
		}

		public void UpdateQuestReceives(List<string> keys)
		{
			for (int i = 0; i < receives.Count; i++)
			{
				if (!keys.Contains(receives[i]))
				{
					receives.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
