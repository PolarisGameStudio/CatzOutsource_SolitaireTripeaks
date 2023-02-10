using Nightingale.Extensions;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public abstract class QuestBaseUI : DelayBehaviour
	{
		public Text DescriptionLabel;

		public ImageUI RewardLabelUI;

		public RemainLabel RemainLabel;

		protected void SetConfigInfo(QuestConfig config)
		{
			DescriptionLabel.text = config.GetDescription();
			if (config.RewardCount >= 1000)
			{
				RewardLabelUI.SetLabel($"x{config.RewardCount / 1000}K");
			}
			else
			{
				RewardLabelUI.SetLabel($"x{config.RewardCount}");
			}
			RewardLabelUI.SetImage(AppearNodeConfig.Get().GetBoosterSprite(config.BoosterType));
		}
	}
}
