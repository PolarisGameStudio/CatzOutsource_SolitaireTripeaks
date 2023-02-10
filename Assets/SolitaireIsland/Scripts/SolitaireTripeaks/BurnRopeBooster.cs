using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class BurnRopeBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<RopeExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<RopeExtra> extras = PlayDesk.Get().GetExtras<RopeExtra>();
					foreach (RopeExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
