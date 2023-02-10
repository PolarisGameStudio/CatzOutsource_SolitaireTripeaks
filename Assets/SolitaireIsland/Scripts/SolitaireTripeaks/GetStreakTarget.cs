using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class GetStreakTarget : QuestTarget
	{
		public override void DoQuest(QuestInfo questInfo, ScheduleData schedule)
		{
			if (schedule.world > questInfo.CurrentCount)
			{
				questInfo.CurrentCount = schedule.world;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			return string.Format(localizationUtility.GetString("Get_Streak_Count"), Config.NeedCount);
		}

		public override string GetLeftDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Get_Streak_Count");
			@string = @string.Replace("{0}", "|");
			return @string.Substring(0, @string.IndexOf("|")).Trim();
		}

		public override string GetRightDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Get_Streak_Count");
			@string = @string.Replace("{0}", "|");
			return @string.Substring(@string.IndexOf("|") + 1).Trim();
		}
	}
}
