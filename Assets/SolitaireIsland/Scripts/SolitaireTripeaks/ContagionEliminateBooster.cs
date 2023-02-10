using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class ContagionEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			if (PlayDesk.Get().GetExtras<ContagionExtra>().Count > 0)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<ContagionExtra> extras = PlayDesk.Get().GetExtras<ContagionExtra>();
					foreach (ContagionExtra item in extras)
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
