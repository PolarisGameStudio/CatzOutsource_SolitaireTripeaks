using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SnakeCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/SnakePoker";
		}

		private ForkCard FindForkCard()
		{
			BaseCard[] array = (from e in PlayDesk.Get().Uppers
				where e is ForkCard
				select e).ToArray();
			BaseCard[] array2 = array;
			foreach (BaseCard baseCard in array2)
			{
				if (baseCard.IsFree())
				{
					return (ForkCard)baseCard;
				}
			}
			return null;
		}

		private Sequence JumpAndRotate(Transform transform, Vector3 position, bool left)
		{
			Sequence sequence = DOTween.Sequence();
			position.x += ((!left) ? 5 : (-5));
			position.y += 8.2f;
			sequence.Append(transform.DOMove(position, 1f));
			sequence.Join(transform.DORotateZ(720f, 1f));
			return sequence;
		}

		public override void DestoryByBooster()
		{
			PlayDesk.Get().RemoveCard(this);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(JumpAndRotate(base.transform, base.transform.position, left: false));
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			PlayDesk.Get().CalcTopCard();
			PlayDesk.Get().DestopChanged();
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return FindForkCard();
		}

		public override bool IsApplyGloden()
		{
			return false;
		}

		public override void DestoryCollect(bool step)
		{
			PlayDesk.Get().DestopChanged();
			if (PlayDesk.Get().RemoveCard(this))
			{
				PlayDesk.Get().LinkOnce(base.transform.position);
				OperatingHelper.Get().ClearStep();
				SetBoxCollider(enable: false);
				ForkCard forkCard = FindForkCard();
				forkCard.SetBoxCollider(enable: false);
				PlayDesk.Get().RemoveCard(forkCard);
				forkCard.EatSnakeCard(this);
			}
		}
	}
}
