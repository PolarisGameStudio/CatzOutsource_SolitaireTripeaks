using Nightingale.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DailyBonusItemUI : MonoBehaviour
	{
		public Text TitleLabel;

		public Text DesLabel;

		public Image Background;

		public Image Light;

		public CanvasGroup group;

		public void SetInfo(int day, DailyRewardState state)
		{
			TitleLabel.text = string.Format(LocalizationUtility.Get("Localization_bonus.json").GetString("Title_Day"), day + 1);
			switch (state)
			{
			case DailyRewardState.Rewarded:
				group.alpha = 0.8f;
				Background.color = new Color32(188, 188, 188, byte.MaxValue);
				Light.gameObject.SetActive(value: false);
				break;
			case DailyRewardState.Rewarding:
				Background.color = new Color32(byte.MaxValue, 251, 122, byte.MaxValue);
				Light.gameObject.SetActive(value: true);
				break;
			case DailyRewardState.Rewardinging:
				Light.gameObject.SetActive(value: false);
				break;
			}
		}
	}
}
