using Nightingale.Localization;
using System;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OldWildSaleUI : OldSaleUI
	{
		private void Awake()
		{
			try
			{
				Text component = base.transform.GetChild(0).GetChild(0).GetChild(0)
					.GetChild(1)
					.gameObject.GetComponent<Text>();
					if (component != null)
					{
						component.text = LocalizationUtility.Get("Localization_Sale.json").GetString("Title_Wilds");
					}
				}
				catch (Exception)
				{
				}
				AddLocalizationFont();
				AddBuyButtonEffect();
			}
		}
	}
