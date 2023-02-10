using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class ClearNumberUpDownTarget : QuestTarget
	{
		public override bool IsEnable()
		{
			return AppearNodeConfig.Get().HasExtraType(ExtraType.NumberGrow);
		}

		public override void DoQuest(QuestInfo questInfo, ScheduleData questIndex)
		{
			if (questInfo.Config.ScheduleData.world == questIndex.world)
			{
				questInfo.CurrentCount++;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			string[] array = new string[2]
			{
				"Up_Card",
				"Down_Card"
			};
			string key = array[Config.ScheduleData.world];
			return string.Format(localizationUtility.GetString("Clear_Number_Up_Down_Count"), Config.NeedCount, localizationUtility.GetString(key));
		}

		public override string GetLeftDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Clear_Bomb_Count");
			return @string.Substring(0, @string.IndexOf("{0}")).Trim();
		}

		public override string GetRightDescription(QuestConfig Config)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			string[] array = new string[2]
			{
				"Up_Card",
				"Down_Card"
			};
			string key = array[Config.ScheduleData.world];
			string @string = localizationUtility.GetString("Clear_Number_Up_Down_Count");
			return string.Format(@string.Substring(@string.IndexOf("{0}")), string.Empty, localizationUtility.GetString(key)).Trim();
		}
	}
}
