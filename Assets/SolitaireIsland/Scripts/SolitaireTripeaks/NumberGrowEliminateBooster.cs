using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class NumberGrowEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<NumberGrowExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<NumberGrowExtra> extras = PlayDesk.Get().GetExtras<NumberGrowExtra>();
					foreach (NumberGrowExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
