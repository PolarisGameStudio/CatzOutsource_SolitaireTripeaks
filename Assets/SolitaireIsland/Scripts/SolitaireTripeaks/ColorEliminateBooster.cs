using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class ColorEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<ColorExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<ColorExtra> extras = PlayDesk.Get().GetExtras<ColorExtra>();
					foreach (ColorExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
