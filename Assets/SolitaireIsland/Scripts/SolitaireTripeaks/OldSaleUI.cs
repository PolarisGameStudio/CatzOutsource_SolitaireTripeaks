using Nightingale.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OldSaleUI : MonoBehaviour
	{
		public void AddLocalizationFont()
		{
			Text[] componentsInChildren = base.transform.gameObject.GetComponentsInChildren<Text>();
			Text[] array = componentsInChildren;
			foreach (Text text in array)
			{
				text.gameObject.AddComponent<LocalizationFont>();
				FontConfig fontConfig = text.gameObject.AddComponent<FontConfig>();
				fontConfig.fonts = new List<Nightingale.Localization.FontData>
				{
					new Nightingale.Localization.FontData
					{
						Language = SystemLanguage.Korean,
						SizeScaler = 0.9f,
						LineSpacingScaler = 1f
					},
					new Nightingale.Localization.FontData
					{
						Language = SystemLanguage.Japanese,
						SizeScaler = 0.9f,
						LineSpacingScaler = 1f
					},
					new Nightingale.Localization.FontData
					{
						Language = SystemLanguage.Spanish,
						SizeScaler = 0.9f,
						LineSpacingScaler = 1f
					},
					new Nightingale.Localization.FontData
					{
						Language = SystemLanguage.German,
						SizeScaler = 0.9f,
						LineSpacingScaler = 1f
					},
					new Nightingale.Localization.FontData
					{
						Language = SystemLanguage.French,
						SizeScaler = 0.9f,
						LineSpacingScaler = 1f
					}
				};
			}
		}

		public void AddBuyButtonEffect()
		{
			Button[] componentsInChildren = base.transform.gameObject.GetComponentsInChildren<Button>();
			Button[] array = componentsInChildren;
			foreach (Button button in array)
			{
				if (!(button.name == "CloseButton"))
				{
					button.gameObject.AddComponent<ButtonLightMask>();
				}
			}
		}

		public void SetBooster(string content, Transform transform)
		{
			if (!(transform == null))
			{
				Text component = transform.GetComponent<Text>();
				if (!(component == null))
				{
					component.text = LocalizationUtility.Get("Localization_Sale.json").GetString(content);
				}
			}
		}
	}
}
