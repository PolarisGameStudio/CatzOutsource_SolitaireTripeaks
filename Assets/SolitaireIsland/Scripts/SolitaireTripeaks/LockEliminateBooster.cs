using DG.Tweening;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	public class LockEliminateBooster : GlobalBooster
	{
		public override bool OpenPoker()
		{
			if (PlayDesk.Get().GetAllExtras<KeyExtra>().Count > 0)
			{
				Sequence sequence = DOTween.Sequence();
				sequence.PrependInterval(1f);
				sequence.OnComplete(delegate
				{
					List<KeyExtra> allExtras = PlayDesk.Get().GetAllExtras<KeyExtra>();
					foreach (KeyExtra item in allExtras)
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
