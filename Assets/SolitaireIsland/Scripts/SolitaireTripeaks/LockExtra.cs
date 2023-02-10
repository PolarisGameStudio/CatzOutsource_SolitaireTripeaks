using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class LockExtra : DebuffExtra
	{
		public override bool IsFree()
		{
			return false;
		}

		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/LockSpine";
		}

		public override void RemoveAnimtor(UnityAction unityAction)
		{
			base.RemoveAnimtor(unityAction);
			AudioUtility.GetSound().Play("Audios/lock_destory.wav");
		}

		public override void DestoryByBooster()
		{
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
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
				UnityEngine.Object.Destroy(base.gameObject);
			});
			baseCard.DestoryCollect(step: false);
			return true;
		}
	}
}
