using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class CheeseTacticsBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetExtras<SwallowedExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<SwallowedExtra> extras = PlayDesk.Get().GetExtras<SwallowedExtra>();
					foreach (SwallowedExtra item in extras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
