using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class WinRowTarget : QuestTarget
	{
		public override void DoQuest(QuestInfo questInfo, ScheduleData questIndex)
		{
			if (questIndex.world > 0)
			{
				questInfo.CurrentCount++;
			}
			else
			{
				questInfo.CurrentCount = 0;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			return string.Format(localizationUtility.GetString("Win_Row_Count"), Config.NeedCount);
		}

		public override string GetLeftDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Win_Row_Count");
			@string = @string.Replace("{0}", "|");
			return @string.Substring(0, @string.IndexOf("|")).Trim();
		}

		public override string GetRightDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Win_Row_Count");
			@string = @string.Replace("{0}", "|");
			return @string.Substring(@string.IndexOf("|") + 1).Trim();
		}
	}
}
