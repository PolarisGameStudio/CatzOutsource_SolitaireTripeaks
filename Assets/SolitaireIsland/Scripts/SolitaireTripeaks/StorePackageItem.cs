using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[Serializable]
	public class StorePackageItem : MonoBehaviour
	{
		public void SetNumber(PurchasingCommodity commodity)
		{
			if (commodity == null)
			{
				return;
			}
			Text componentInChildren = base.gameObject.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				if (commodity.boosterType != BoosterType.Coins)
				{
					componentInChildren.text = $"x{commodity.count:N0}";
				}
				else
				{
					componentInChildren.text = $"{commodity.count:N0}";
				}
			}
			Image componentInChildren2 = base.gameObject.GetComponentInChildren<Image>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(commodity.boosterType);
			}
		}
	}
}
