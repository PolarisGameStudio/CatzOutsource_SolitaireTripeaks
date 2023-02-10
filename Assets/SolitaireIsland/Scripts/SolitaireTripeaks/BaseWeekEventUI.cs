using Nightingale.Localization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BaseWeekEventUI : MonoBehaviour
	{
		public Text DaysOfWeekLabel;

		public Text DaysOfYearLabel;

		public DateTime Time;

		protected void SetDateTime(DateTime startTime)
		{
			Time = startTime;
			DaysOfWeekLabel.text = startTime.ToString("dddd", LocalizationUtility.GetCultureInfo());
			DaysOfYearLabel.text = startTime.ToString("MMMM d", LocalizationUtility.GetCultureInfo());
			switch (startTime.Day % 10)
			{
			case 1:
				DaysOfYearLabel.text += "st";
				break;
			case 2:
				DaysOfYearLabel.text += "nd";
				break;
			case 3:
				DaysOfYearLabel.text += "rd";
				break;
			default:
				DaysOfYearLabel.text += "th";
				break;
			}
		}
	}
}
