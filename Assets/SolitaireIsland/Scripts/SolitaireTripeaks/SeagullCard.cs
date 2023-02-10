using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SeagullCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/SeagullPoker";
		}

		public override void DestoryCollect(bool step)
		{
			PlayDesk.Get().RemoveCard(this);
			PlayStreaksSystem.Get().StreaksOnce();
			PlayDesk.Get().ClearCard();
			PlayDesk.Get().DestopChanged();
			PlayDesk.Get().CalcTopCard();
			AudioUtility.GetSound().Play("Audios/collectBirdCard.wav");
			OperatingHelper.Get().ClearStep();
			Seagull.CreateSeagulls(Config.ExtraContent);
			Vector3 position = base.transform.position;
			position.x += 1.5f;
			position.y -= 6f;
			SetPokerFaceAllShow();
			base.transform.DOMove(position, 0.8f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			base.transform.DORotate(Random.insideUnitSphere * 360f * 3f, 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		}

		public override bool StayInTop()
		{
			return false;
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
