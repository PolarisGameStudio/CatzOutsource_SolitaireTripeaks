using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyConfig
	{
		public string Type;

		public DailyLevel[] dailyLevels;

		private QuestTarget QuestTarget;

		public bool IsEnable()
		{
			if (QuestTarget == null)
			{
				QuestTarget = (QuestTarget)Activator.CreateInstance(EnumUtility.GetStringType(GetQuestType()));
			}
			return QuestTarget.IsEnable();
		}

		public QuestType GetQuestType()
		{
			return EnumUtility.GetEnumType(Type, QuestType.Play);
		}
	}
}
