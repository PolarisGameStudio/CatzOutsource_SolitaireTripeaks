using System;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyLevel
	{
		public DailyInfo[] dailyInfos;

		public DailyInfo RandomDailyInfo(QuestType questType)
		{
			if (questType == QuestType.WinGameInScene)
			{
				DailyInfo[] array = (from e in dailyInfos
					where PlayData.Get().HasLevelData(e.scheduleData)
					select e).ToArray();
				return array[UnityEngine.Random.Range(0, array.Length)];
			}
			return dailyInfos[UnityEngine.Random.Range(0, dailyInfos.Length)];
		}
	}
}
