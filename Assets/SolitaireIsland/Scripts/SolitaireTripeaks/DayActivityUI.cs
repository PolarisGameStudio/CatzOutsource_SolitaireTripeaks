using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class DayActivityUI : BaseWeekEventUI
	{
		public Image BackgroundImage;

		public void OnStart(DayActivityConfig config)
		{
			SetDateTime(config.Time);
		}
	}
}
