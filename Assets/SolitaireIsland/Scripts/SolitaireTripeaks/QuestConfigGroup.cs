using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	[CreateAssetMenu(fileName = "QuestConfigGroup.asset", menuName = "Nightingale/Quest Config Group", order = 1)]
	public class QuestConfigGroup : ScriptableObject
	{
		public List<EventConfig> QuestSingleWeek;

		public List<EventConfig> QuestDoubleWeek;

		private static QuestConfigGroup group;

		public static QuestConfigGroup Get()
		{
			if (group == null)
			{
				group = SingletonBehaviour<LoaderUtility>.Get().GetAsset<QuestConfigGroup>("Configs/QuestConfigGroup");
			}
			return group;
		}

		public List<QuestConfig> GetQuests()
		{
			List<QuestConfig> nextQuests = GetNextQuests();
			DateTime weekEnd = DateTime.Now.Date.AddDays((double)(7 - DateTime.Now.Date.DayOfWeek));
			if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
			{
				weekEnd = DateTime.Today;
			}
			UnityEngine.Debug.LogFormat("结束时间： {0}", weekEnd);
			nextQuests.RemoveAll((QuestConfig p) => weekEnd.Subtract(DateTime.Parse(p.StartTime)).TotalSeconds < 0.0);
			return nextQuests;
		}

		public List<QuestConfig> GetNextQuests()
		{
			List<QuestConfig> quests = GetQuests(DateTime.Now);
			quests.AddRange(GetQuests(DateTime.Now.AddDays(7.0)));
			return quests;
		}

		public EventConfig GetDailyEvent()
		{
			List<EventConfig> list = QuestDoubleWeek;
			if ((int)DateTime.Today.Date.Subtract(new DateTime(2018, 5, 28)).TotalDays / 7 % 2 == 0)
			{
				list = QuestSingleWeek;
			}
			return list.Find((EventConfig e) => e.DayOfWeek == DateTime.Today.DayOfWeek);
		}

		private List<QuestConfig> GetQuests(DateTime time)
		{
			List<EventConfig> source = QuestDoubleWeek;
			if ((int)time.Date.Subtract(new DateTime(2018, 5, 28)).TotalDays / 7 % 2 == 0)
			{
				source = QuestSingleWeek;
			}
			return (from p in source
				select p.GetConfig(time)).ToList();
		}
	}
}
