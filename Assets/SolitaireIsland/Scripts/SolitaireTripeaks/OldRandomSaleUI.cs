using Nightingale.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class OldRandomSaleUI : OldSaleUI
	{
		private void Awake()
		{
			Transform child = base.transform.GetChild(0).GetChild(0).GetChild(0);
			try
			{
				Text component = child.GetChild(1).gameObject.GetComponent<Text>();
				if (component != null)
				{
					component.text = LocalizationUtility.Get("Localization_Sale.json").GetString("Title_Boosters");
				}
			}
			catch (Exception)
			{
			}
			try
			{
				SetBooster("ui_RandomBoosters", child.GetChild(2).GetChild(3));
				SetBooster("ui_Coins", child.GetChild(2).GetChild(4));
				SetBooster("ui_RandomBoosters", child.GetChild(3).GetChild(3));
				SetBooster("ui_Coins", child.GetChild(3).GetChild(4));
			}
			catch (Exception)
			{
			}
			AddLocalizationFont();
			AddBuyButtonEffect();
		}
	}
}
