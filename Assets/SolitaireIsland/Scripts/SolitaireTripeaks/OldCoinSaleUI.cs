using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class OldCoinSaleUI : OldSaleUI
	{
		private void Awake()
		{
			try
			{
				Transform child = base.transform.GetChild(0).GetChild(0).GetChild(0)
					.GetChild(0);
				SetBooster("Title_Coins", child.GetChild(8));
				SetBooster("ui_was", child.GetChild(5));
				SetBooster("ui_now", child.GetChild(4));
			}
			catch (Exception)
			{
			}
			AddLocalizationFont();
			AddBuyButtonEffect();
		}
	}
}
