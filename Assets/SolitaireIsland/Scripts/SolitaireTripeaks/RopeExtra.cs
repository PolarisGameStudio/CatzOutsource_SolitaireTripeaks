using DG.Tweening;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class RopeExtra : DebuffExtra
	{
		private SpriteRenderer spriteRenderer;

		protected override void StartInitialized()
		{
			spriteRenderer = CreateSpriteRenderer("Rope");
			spriteRenderer.sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, "Sprites/SpecialPoker").GetSprite("rope");
		}

		public override void RemoveAnimtor(UnityAction unityAction)
		{
			spriteRenderer.DOFade(0f, 0.3f);
			spriteRenderer.transform.DOScale(2f, 0.3f).OnComplete(delegate
			{
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearRope);
				if (unityAction != null)
				{
					unityAction();
				}
			});
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

		public override void DestoryByBooster()
		{
			RemoveAnimtor(delegate
			{
				baseCard.RemoveExtra(this);
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().DestopChanged();
			});
		}

		public override bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByGolden()
		{
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
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
				RemoveAnimtor(delegate
				{
					UnityEngine.Object.Destroy(base.gameObject);
				});
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

		public override bool DestoryByMatch(BaseCard card)
		{
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			PlayDesk.Get().LinkOnce(baseCard.transform.position);
			baseCard.RemoveExtra(this);
			BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
			flyCard.UpdateOrderLayer(32667);
			DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
			flyCard.transform.parent = PlayDesk.Get().transform;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(MoveCard(flyCard.transform, delegate
			{
				DestoryByBooster();
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

		public override bool DestoryByRocket()
		{
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			PlayDesk.Get().LinkOnce(baseCard.transform.position);
			OperatingHelper.Get().ClearStep();
			baseCard.RemoveExtra(this);
			RemoveAnimtor(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().DestopChanged();
			});
			return true;
		}
	}
}
