using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[Serializable]
	public class EventChoiceUI
	{
		public Text QuestTitle;

		public Text QuestReward;

		public Image RewardIcon;

		public Button GoButton;

		public void OnStart(QuestConfig questConfig, string eventId)
		{
			RewardIcon.sprite = AppearNodeConfig.Get().GetBoosterSprite(questConfig.BoosterType);
			RewardIcon.SetNativeSize();
			QuestTitle.text = questConfig.GetDescription();
			if (questConfig.RewardCount >= 1000)
			{
				QuestReward.text = $"x{questConfig.RewardCount / 1000}K";
			}
			else
			{
				QuestReward.text = $"x{questConfig.RewardCount}";
			}
			GoButton.onClick.AddListener(delegate
			{
				QuestData.Get().quests.RemoveAll((QuestInfo e) => e.QuestStyle == QuestStyle.Event);
				QuestData.Get().AppendQuest(QuestStyle.Event, questConfig);
				SingletonClass<MySceneManager>.Get().Close();
				SingletonBehaviour<TripeaksLogUtility>.Get().UploadMakeAChoice($"{questConfig.QuestType}_{questConfig.NeedCount}_{questConfig.BoosterType.ToString()}_{questConfig.RewardCount}", eventId);
			});
		}
	}
}
