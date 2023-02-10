using I2.MiniGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class WheelRewardElement : MonoBehaviour
	{
		public PrizeWheel_Reward prizeWheel_Reward;

		public Image IconImage;

		public Text RewardLabel;

		public List<WheelSprite> WheelSprites;

		public void SetInfo(WheelData wheelData)
		{
			prizeWheel_Reward.Probability = wheelData.Probality;
			WheelSprite wheelSprite = WheelSprites.Find((WheelSprite e) => e.boosterType == wheelData.boosterType);
			if (wheelSprite != null)
			{
				IconImage.sprite = wheelSprite.sprite;
			}
			IconImage.SetNativeSize();
			if (wheelData.Rewards >= 1000)
			{
				RewardLabel.text = $"{wheelData.Rewards / 1000}K";
			}
			else
			{
				RewardLabel.text = $"X{wheelData.Rewards}";
			}
		}
	}
}
