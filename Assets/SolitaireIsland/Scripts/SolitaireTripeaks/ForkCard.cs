using DG.Tweening;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ForkCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/ForkPoker";
		}

		public void EatSnakeCard(SnakeCard snakeCard)
		{
			AudioUtility.GetSound().Play("Audios/Fork_Snake.mp3");
			PlayDesk.Get().ClearCard();
			snakeCard.UpdateOrderLayer(32667);
			UpdateOrderLayer(32677);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOJump(snakeCard.transform.position, 2f, 1, 0.5f));
			sequence.AppendCallback(delegate
			{
				PlayDesk.Get().CalcTopCard();
			});
			sequence.Append(JumpAndRotate(base.transform, snakeCard.transform.position, left: true));
			sequence.Join(JumpAndRotate(snakeCard.transform, snakeCard.transform.position, left: false));
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
				UnityEngine.Object.Destroy(snakeCard.gameObject);
				PlayDesk.Get().DestopChanged();
			});
			PlayDesk.Get().AppendBusyTime(sequence.Duration());
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

		private SnakeCard FindSnakeCard()
		{
			BaseCard[] array = (from e in PlayDesk.Get().Uppers
				where e is SnakeCard
				select e).ToArray();
			BaseCard[] array2 = array;
			foreach (BaseCard baseCard in array2)
			{
				if (baseCard.IsFree())
				{
					return (SnakeCard)baseCard;
				}
			}
			return null;
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
			return FindSnakeCard();
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
				OperatingHelper.Get().ClearStep();
				SetBoxCollider(enable: false);
				SnakeCard snakeCard = FindSnakeCard();
				snakeCard.SetBoxCollider(enable: false);
				PlayDesk.Get().RemoveCard(snakeCard);
				EatSnakeCard(snakeCard);
				PlayDesk.Get().LinkOnce(base.transform.position);
			}
		}
	}
}
