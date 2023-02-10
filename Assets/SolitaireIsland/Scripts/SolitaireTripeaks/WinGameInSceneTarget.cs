using Nightingale.Localization;

namespace SolitaireTripeaks
{
	public class WinGameInSceneTarget : QuestTarget
	{
		public override void DoQuest(QuestInfo questInfo, ScheduleData questIndex)
		{
			if (questInfo.Config.ScheduleData.world == questIndex.world && questInfo.Config.ScheduleData.chapter == questIndex.chapter)
			{
				questInfo.CurrentCount++;
			}
		}

		public override string GetDescription(QuestConfig Config)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_quest.json");
			return string.Format(localizationUtility.GetString("Win_InChapter_Count"), UniverseConfig.Get().GetChapterConfig(Config.ScheduleData.world, Config.ScheduleData.chapter).name, Config.NeedCount);
		}
	}
}
