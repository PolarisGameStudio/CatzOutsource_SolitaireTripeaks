using Nightingale.Localization;
using Nightingale.Utilitys;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class QuestConfig
	{
		public string identifier;

		public QuestType QuestType;

		public ScheduleData ScheduleData;

		public int NeedCount;

		public BoosterType BoosterType;

		public int RewardCount;

		public string StartTime;

		public int Hours;

		private QuestTarget QuestTarget;

		public void DoQuest(QuestInfo info, ScheduleData scheduleData)
		{
			if (QuestTarget == null)
			{
				QuestTarget = (QuestTarget)Activator.CreateInstance(EnumUtility.GetStringType(QuestType));
			}
			QuestTarget.DoQuest(info, scheduleData);
		}

		public string GetDescription()
		{
			if (QuestTarget == null)
			{
				QuestTarget = (QuestTarget)Activator.CreateInstance(EnumUtility.GetStringType(QuestType));
			}
			return QuestTarget.GetDescription(this);
		}

		public string GetDescriptionInBox()
		{
			return string.Format(LocalizationUtility.Get("Localization_inbox.json").GetString("Quest_New_Desc"), GetStartTime().ToString("ddd, d MMM yyyy", LocalizationUtility.GetCultureInfo()), $"{RewardCount} {AppearNodeConfig.Get().GetBoosterQuestTitle(BoosterType)}");
		}

		public string GetLeftDescription()
		{
			if (QuestTarget == null)
			{
				QuestTarget = (QuestTarget)Activator.CreateInstance(EnumUtility.GetStringType(QuestType));
			}
			return QuestTarget.GetLeftDescription(this);
		}

		public string GetRightDescription()
		{
			if (QuestTarget == null)
			{
				QuestTarget = (QuestTarget)Activator.CreateInstance(EnumUtility.GetStringType(QuestType));
			}
			return QuestTarget.GetRightDescription(this);
		}

		public bool IsStart()
		{
			if (string.IsNullOrEmpty(StartTime))
			{
				return true;
			}
			return DateTime.Now.Subtract(DateTime.Parse(StartTime)).TotalSeconds >= 0.0;
		}

		public bool IsInvalid()
		{
			if (string.IsNullOrEmpty(StartTime))
			{
				return false;
			}
			return DateTime.Now.Subtract(DateTime.Parse(StartTime).AddHours(Hours)).TotalSeconds >= 0.0;
		}

		public bool IsReady()
		{
			return IsStart() && !IsInvalid();
		}

		public DateTime GetStartTime()
		{
			return DateTime.Parse(StartTime);
		}

		public QuestConfig Clone()
		{
			QuestConfig questConfig = new QuestConfig();
			questConfig.ScheduleData = ScheduleData;
			questConfig.StartTime = StartTime;
			questConfig.BoosterType = BoosterType;
			questConfig.Hours = Hours;
			questConfig.identifier = identifier;
			questConfig.NeedCount = NeedCount;
			questConfig.QuestType = QuestType;
			questConfig.RewardCount = RewardCount;
			return questConfig;
		}
	}
}
