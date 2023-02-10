using System;
using System.Collections.Generic;
using System.Linq;

namespace SolitaireTripeaks
{
	public class MakeAChoicePopup : SoundScene
	{
		public List<EventChoiceUI> EventChoiceUIs;

		private void Awake()
		{
			EventConfig dailyEvent = QuestConfigGroup.Get().GetDailyEvent();
			List<QuestConfig> configs = dailyEvent.GetConfigs(DateTime.Now);
			string eventId = string.Format("{0}:{1}", dailyEvent.QuestType, string.Join(";", (from e in configs
				select $"{e.NeedCount}_{e.BoosterType}_{e.RewardCount}").ToArray()));
			for (int i = 0; i < configs.Count; i++)
			{
				if (i < EventChoiceUIs.Count)
				{
					EventChoiceUIs[i].OnStart(configs[i], eventId);
				}
			}
		}
	}
}
