using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class BatterEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			bool flag = PlayDesk.Get().GetAllExtras<BatterExtra>().Count > 0;
			if (flag)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<BatterExtra> allExtras = PlayDesk.Get().GetAllExtras<BatterExtra>();
					foreach (BatterExtra item in allExtras)
					{
						item.DestoryByBooster();
					}
				});
			}
			return flag;
		}
	}
}
