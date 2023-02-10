using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class CollectNumberCardTarget : QuestTarget
	{
		public override void DoQuest(QuestInfo questInfo, ScheduleData questIndex)
		{
			if (questInfo.Config.ScheduleData.world == questIndex.world)
			{
				questInfo.CurrentCount++;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			string[] array = new string[13]
			{
				"A",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"10",
				"J",
				"Q",
				"K"
			};
			string key = array[Config.ScheduleData.world];
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			return string.Format(localizationUtility.GetString("Collect_Number_Card"), Config.NeedCount, localizationUtility.GetString(key));
		}

		public override string GetLeftDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Clear_Bomb_Count");
			return @string.Substring(0, @string.IndexOf("{0}")).Trim();
		}

		public override string GetRightDescription(QuestConfig Config)
		{
			string[] array = new string[13]
			{
				"A",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"10",
				"J",
				"Q",
				"K"
			};
			string key = array[Config.ScheduleData.world];
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			string @string = localizationUtility.GetString("Collect_Number_Card");
			return string.Format(@string.Substring(@string.IndexOf("{0}")), string.Empty, localizationUtility.GetString(key)).Trim();
		}
	}
}
