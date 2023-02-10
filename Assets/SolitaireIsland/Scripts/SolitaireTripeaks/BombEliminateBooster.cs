using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class BombEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<BombExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<BombExtra> extras = PlayDesk.Get().GetExtras<BombExtra>();
					foreach (BombExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
