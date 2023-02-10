namespace SolitaireTripeaks
{
	public abstract class QuestTarget
	{
		public virtual void DoQuest(QuestInfo questData, ScheduleData scheduleData)
		{
		}

		public virtual string GetDescription(QuestConfig Config)
		{
			return string.Empty;
		}

		public virtual string GetLeftDescription(QuestConfig Config)
		{
			return string.Empty;
		}

		public virtual string GetRightDescription(QuestConfig Config)
		{
			return string.Empty;
		}

		public virtual bool IsEnable()
		{
			return true;
		}
	}
}
