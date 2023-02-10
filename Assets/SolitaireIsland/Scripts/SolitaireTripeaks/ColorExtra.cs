using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ColorExtra : DebuffExtra
	{
		private Sequence MoveCard(Transform transform, TweenCallback tweenCallback)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOMove(baseCard.transform.position, 0.7f));
			sequence.AppendCallback(tweenCallback);
			Vector3 position = baseCard.transform.position;
			Vector3 position2 = baseCard.transform.position;
			float x = position2.x;
			Vector3 position3 = transform.position;
			if (x < position3.x)
			{
				position.x -= 10f;
			}
			else
			{
				position.x += 10f;
			}
			sequence.Append(transform.DOMove(position, 0.35f));
			sequence.SetEase(Ease.Linear);
			return sequence;
		}

		private Sequence RotateCard(Transform transform)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DORotateX(80f, 0.2f));
			sequence.Join(transform.DORotateZ(1800f, 1f));
			sequence.SetEase(Ease.Linear);
			return sequence;
		}

		protected override void StartInitialized()
		{
			base.StartInitialized();
			baseCard.FrontPoker.SetSuitVisable(visable: false);
		}

		protected override string PokerPrefab()
		{
			if (baseCard.GetColor() == 0)
			{
				return "Prefabs/Extras/BlackColorSpine";
			}
			return "Prefabs/Extras/RedColorSpine";
		}

		public override void DestoryByBooster()
		{
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				baseCard.FrontPoker.SetSuitVisable(visable: true);
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
			if (!(card is NumberCard) || baseCard.GetColor() == card.GetColor())
			{
				DestoryByRocket();
			}
			else
			{
				PlayDesk.Get().RemoveCard(baseCard);
				PlayDesk.Get().ClearCard();
				BaseCard flyCard = HandCardSystem.Get()._RightHandGroup.FlyCard();
				flyCard.UpdateOrderLayer(32667);
				DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
				flyCard.transform.parent = PlayDesk.Get().transform;
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				AudioUtility.GetSound().Play("Audios/Rope.mp3");
				Sequence sequence = DOTween.Sequence();
				sequence.Append(MoveCard(flyCard.transform, delegate
				{
					PlayDesk.Get().DestopChanged();
					Vector3 position = base.transform.position;
					position.x += 1.5f;
					position.y -= 6f;
					baseCard.SetPokerFaceAllShow();
					baseCard.transform.DOMove(position, 0.8f).OnComplete(delegate
					{
						UnityEngine.Object.Destroy(baseCard.gameObject);
					});
					baseCard.transform.DORotate(Random.insideUnitSphere * 360f * 3f, 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
					baseCard.RemoveExtra(this);
					UnityEngine.Object.Destroy(base.gameObject);
					PlayDesk.Get().CalcTopCard();
				}));
				sequence.Join(RotateCard(flyCard.transform));
				sequence.OnComplete(delegate
				{
					UnityEngine.Object.Destroy(flyCard.gameObject);
				});
				PlayDesk.Get().AppendBusyTime(sequence.Duration());
				HandCardSystem.Get().DoNextCardForce(thinkMiss: false);
			}
			OperatingHelper.Get().ClearStep();
			return true;
		}

		public override bool DestoryByRocket()
		{
			baseCard.FrontPoker.SetSuitVisable(visable: true);
			baseCard.RemoveExtra(this);
			UnityEngine.Object.Destroy(base.gameObject);
			baseCard.DestoryCollect(step: false);
			return true;
		}

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			if (base.PokerSpine != null)
			{
				base.PokerSpine.UpdateOrderLayer(zIndex, -1);
			}
		}
	}
}
