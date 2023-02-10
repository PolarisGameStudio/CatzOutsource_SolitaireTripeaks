using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class CollectShapeCardTarget : QuestTarget
	{
		public override void DoQuest(QuestInfo questInfo, ScheduleData questIndex)
		{
			base.DoQuest(questInfo, questIndex);
			if (questInfo.Config.ScheduleData.world == questIndex.world)
			{
				questInfo.CurrentCount++;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			string[] array = new string[4]
			{
				"Spades",
				"Hearts",
				"Club",
				"Diamond"
			};
			string key = array[Config.ScheduleData.world];
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			return string.Format(localizationUtility.GetString("Collect_Shape_Card"), Config.NeedCount, localizationUtility.GetString(key));
		}

		public override string GetLeftDescription(QuestConfig Config)
		{
			string @string = LocalizationUtility.Get("Localization_quest.json").GetString("Clear_Bomb_Count");
			return @string.Substring(0, @string.IndexOf("{0}")).Trim();
		}

		public override string GetRightDescription(QuestConfig Config)
		{
			string[] array = new string[4]
			{
				"Spade",
				"Heart",
				"Club",
				"Diamond"
			};
			string key = array[Config.ScheduleData.world];
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			string @string = localizationUtility.GetString("Collect_Shape_Card");
			return string.Format(@string.Substring(@string.IndexOf("{0}")), string.Empty, localizationUtility.GetString(key)).Trim();
		}
	}
}
