using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class QuestInfo
	{
		public QuestStyle QuestStyle;

		public string identifier;

		public int CurrentCount;

		public long Ticks;

		public double ReceiveTime;

		public bool Viewd;

		public QuestConfig Config;

		private QuestTarget QuestTarget;

		public QuestInfo(QuestStyle style, QuestConfig config)
		{
			QuestStyle = style;
			identifier = config.identifier;
			Config = config;
			ReceiveTime = SaleData.Get().hours;
			switch (style)
			{
			default:
				Ticks = DateTime.Now.AddHours(config.Hours).Ticks;
				break;
			case QuestStyle.Event:
				Ticks = DateTime.Parse(config.StartTime).AddHours(config.Hours).Ticks;
				break;
			}
		}

		public void DoQuest(ScheduleData scheduleData)
		{
			if (!IsInvalid())
			{
				Config.DoQuest(this, scheduleData);
			}
		}

		public string GetDescription()
		{
			return Config.GetDescription();
		}

		public string GetStepString()
		{
			return $"{GetCurrentCount()}/{Config.NeedCount}";
		}

		public int GetCurrentCount()
		{
			if (CurrentCount > Config.NeedCount)
			{
				CurrentCount = Config.NeedCount;
			}
			return CurrentCount;
		}

		public float GetStep()
		{
			return (float)GetCurrentCount() / (float)Config.NeedCount;
		}

		public bool IsComplete()
		{
			return GetCurrentCount() >= Config.NeedCount;
		}

		public bool IsInvalid()
		{
			if (Config == null)
			{
				return true;
			}
			if (IsComplete())
			{
				return false;
			}
			TimeSpan timeSpan = new DateTime(Ticks).Subtract(DateTime.Now);
			if (timeSpan.TotalHours > 24.0)
			{
				return true;
			}
			return timeSpan.TotalSeconds <= 0.0;
		}

		public string GetId()
		{
			string text = (Config.ScheduleData.world + 1).ToString();
			switch (Config.QuestType)
			{
			case QuestType.CollectNumberCard:
				switch (Config.ScheduleData.world)
				{
				case 10:
					text = "J";
					break;
				case 11:
					text = "Q";
					break;
				case 12:
					text = "K";
					break;
				}
				break;
			case QuestType.CollectColorCard:
				switch (Config.ScheduleData.world)
				{
				case 0:
					text = "BlackCard";
					break;
				case 1:
					text = "RedCard";
					break;
				}
				break;
			case QuestType.CollectSuitCard:
				switch (Config.ScheduleData.world)
				{
				case 0:
					text = "Spade";
					break;
				case 1:
					text = "Heart";
					break;
				case 2:
					text = "Club";
					break;
				case 3:
					text = "Diamond";
					break;
				}
				break;
			}
			return $"{Config.QuestType}_{Config.NeedCount}_{text}_{Config.BoosterType}_{Config.RewardCount}";
		}
	}
}
