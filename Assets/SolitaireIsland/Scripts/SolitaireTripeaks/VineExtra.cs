using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class VineExtra : TimeExtra
	{
		private enum LifeState
		{
			Running,
			Hiding
		}

		private LifeState _LifeState;

		protected override void StartInitialized()
		{
			RestTime(2);
		}

		protected override bool IsRunning()
		{
			return base.IsRunning() && _LifeState == LifeState.Hiding;
		}

		protected override void LifeOver()
		{
			base.LifeOver();
			GrowOut();
		}

		protected virtual void GrowOut()
		{
			_LifeState = LifeState.Running;
			base.PokerSpine.PlayActivation(null);
			RestTime(15);
			AudioUtility.GetSound().Play("Audios/vine_grow.ogg");
		}

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

		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/VineSpine";
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
			LifeState lifeState = _LifeState;
			if (lifeState == LifeState.Running)
			{
				PlayDesk.Get().RemoveCard(baseCard);
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				baseCard.RemoveExtra(this);
				BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
				flyCard.UpdateOrderLayer(32667);
				DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
				flyCard.transform.parent = PlayDesk.Get().transform;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(MoveCard(flyCard.transform, delegate
				{
					AudioUtility.GetSound().Play("Audios/vine_destory.mp3");
					RemoveAnimtor(null);
					HandCardSystem.Get().FromDeskToRightHandCard(baseCard);
					PlayDesk.Get().CalcTopCard();
				}));
				sequence.Join(RotateCard(flyCard.transform));
				sequence.OnComplete(delegate
				{
					PlayDesk.Get().DestopChanged();
					UnityEngine.Object.Destroy(flyCard.gameObject);
				});
				PlayDesk.Get().AppendBusyTime(sequence.Duration());
				OperatingHelper.Get().ClearStep();
				return true;
			}
			DestoryByBooster();
			baseCard.DestoryCollect(step: false);
			return true;
		}

		public override bool DestoryByMatch(BaseCard card)
		{
			LifeState lifeState = _LifeState;
			if (lifeState == LifeState.Running)
			{
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				_LifeState = LifeState.Hiding;
				playing = false;
				BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
				flyCard.UpdateOrderLayer(32667);
				DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
				flyCard.transform.parent = PlayDesk.Get().transform;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(MoveCard(flyCard.transform, delegate
				{
					AudioUtility.GetSound().Play("Audios/vine_destory.mp3");
					RemoveAnimtor(delegate
					{
						playing = true;
					});
				}));
				sequence.Join(RotateCard(flyCard.transform));
				sequence.OnComplete(delegate
				{
					PlayDesk.Get().DestopChanged();
					UnityEngine.Object.Destroy(flyCard.gameObject);
				});
				PlayDesk.Get().AppendBusyTime(sequence.Duration());
				OperatingHelper.Get().ClearStep();
				return true;
			}
			DestoryByBooster();
			baseCard.DestoryCollect(step: false);
			return true;
		}

		public override bool DestoryByRocket()
		{
			if (_LifeState == LifeState.Running)
			{
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				OperatingHelper.Get().ClearStep();
				baseCard.RemoveExtra(this);
				AudioUtility.GetSound().Play("Audios/vine_destory.mp3");
				RemoveAnimtor(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
					PlayDesk.Get().DestopChanged();
				});
				return true;
			}
			DestoryByBooster();
			baseCard.DestoryCollect(step: false);
			return true;
		}
	}
}
