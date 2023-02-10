using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class SkeletonEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			if (PlayDesk.Get().GetExtras<SkeletonExtra>().Count > 0)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<SkeletonExtra> extras = PlayDesk.Get().GetExtras<SkeletonExtra>();
					foreach (SkeletonExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
				return true;
			}
			return false;
		}
	}
}
