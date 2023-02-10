using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PurchasSuccessItem : MonoBehaviour
	{
		public Text Label;

		public Text DesLabel;

		public Image Image;

		public void SetNumber(BoosterType boosterType, int count)
		{
			Label.text = AppearNodeConfig.Get().GetBoosterByNumber(boosterType, count);
			DesLabel.text = AppearNodeConfig.Get().GetBoosterQuestTitle(boosterType);
			Image.sprite = AppearNodeConfig.Get().GetBoosterMiniSprite(boosterType);
			Image.SetNativeSize();
			if (Image.sprite == null)
			{
				Image.gameObject.SetActive(value: false);
			}
		}
	}
}
