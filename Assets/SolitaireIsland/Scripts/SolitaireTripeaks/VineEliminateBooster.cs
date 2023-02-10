using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class VineEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<VineExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<VineExtra> extras = PlayDesk.Get().GetExtras<VineExtra>();
					foreach (VineExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
