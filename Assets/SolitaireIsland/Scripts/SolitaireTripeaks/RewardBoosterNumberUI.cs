using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class RewardBoosterNumberUI : MonoBehaviour
	{
		public Image BoosterImage;

		public Text BoosterLabel;

		public void SetPurchasingCommodity(PurchasingCommodity commodity)
		{
			BoosterImage.sprite = AppearNodeConfig.Get().GetBoosterSprite(commodity.boosterType);
			BoosterImage.SetNativeSize();
			BoosterLabel.text = $"x{commodity.count}";
		}
	}
}
