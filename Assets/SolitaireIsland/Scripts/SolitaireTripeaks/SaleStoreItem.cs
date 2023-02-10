using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SaleStoreItem
	{
		public Text Label;

		public Text NormalLabel;

		public Image IconImage;

		public BoosterType boosterType;

		public string Content;

		public SaleStoreItem()
		{
		}

		public SaleStoreItem(Transform transform)
		{
			Label = FindType<Text>(transform, "SaleNumber");
			NormalLabel = FindType<Text>(transform, "NormalNumber");
			IconImage = FindType<Image>(transform, "Icon");
		}

		public void UpdateInfo(SaleCommodity commodity)
		{
			boosterType = commodity.boosterType;
			if (Label != null)
			{
				if (string.IsNullOrEmpty(Content))
				{
					Content = "{0:N0}";
				}
				Label.text = string.Format(Content, commodity.saleCount);
			}
			if (NormalLabel != null)
			{
				NormalLabel.text = $"{commodity.normalCount:N0}";
			}
			if (IconImage != null)
			{
				IconImage.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(commodity.boosterType);
				IconImage.SetNativeSize();
			}
		}

		private T FindType<T>(Transform transform, string name) where T : Component
		{
			Transform transform2 = transform.Find(name);
			if (transform2 == null)
			{
				return (T)null;
			}
			return transform2.gameObject.GetComponent<T>();
		}
	}
}
