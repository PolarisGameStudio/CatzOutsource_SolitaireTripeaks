using Nightingale.Localization;
using System;

namespace SolitaireTripeaks
{
	public class DayActivityConfig
	{
		public DayActivityType Type;

		public DateTime Time;

		public string GetDescription()
		{
			return LocalizationUtility.Get("Localization_quest.json").GetString($"{Type.ToString()}_Description");
		}

		public string GetDescriptionInBox()
		{
			return string.Format(LocalizationUtility.Get("Localization_inbox.json").GetString("DayActivity_New_Desc"), Time.ToString("ddd, d MMM yyyy", LocalizationUtility.GetCultureInfo()), GetDescription());
		}

		public bool IsInvalid()
		{
			return Time.Subtract(DateTime.Today).TotalDays < 0.0;
		}

		public bool IsRunning()
		{
			return Time.Date == DateTime.Today;
		}
	}
}
