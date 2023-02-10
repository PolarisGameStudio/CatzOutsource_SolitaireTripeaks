using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class KeyExtra : DebuffExtra
	{
		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/KeySpine";
		}

		public override void DestoryByBooster()
		{
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				List<LockExtra> allExtras = PlayDesk.Get().GetAllExtras<LockExtra>();
				foreach (LockExtra item in allExtras)
				{
					item.DestoryByBooster();
				}
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().CalcTopCard();
				PlayDesk.Get().DestopChanged();
			});
			OperatingHelper.Get().ClearStep();
		}

		public override bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByGolden()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByMatch(BaseCard card)
		{
			return DestoryByRocket();
		}

		public override bool DestoryByRocket()
		{
			OperatingHelper.Get().ClearStep();
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				List<LockExtra> allExtras = PlayDesk.Get().GetAllExtras<LockExtra>();
				foreach (LockExtra item in allExtras)
				{
					item.DestoryByBooster();
				}
				UnityEngine.Object.Destroy(base.gameObject);
			});
			baseCard.DestoryCollect(step: false);
			return true;
		}
	}
}
