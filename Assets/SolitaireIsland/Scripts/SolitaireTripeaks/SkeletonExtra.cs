using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class SkeletonExtra : DebuffExtra
	{
		protected override void StartInitialized()
		{
			base.transform.localPosition = new Vector3(0f, 1.4f, 0f);
			base.Index = 5;
			base.StartInitialized();
			base.PokerSpine.PlayIndex(base.Index);
		}

		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/SkeletonSpine";
		}

		public override void OnRemoveCard()
		{
			base.OnRemoveCard();
			if (!Object.FindObjectOfType<SkeletonEliminateBooster>() && PlayDesk.Get().Uppers.Contains(baseCard))
			{
				base.Index--;
				base.PokerSpine.PlayIndex(base.Index);
				if (base.Index <= 0)
				{
					AudioUtility.GetSound().Play("Audios/skeleton.mp3");
					GameObject gameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Particles/SkullDeadParticle"));
					Transform transform = gameObject.transform;
					Vector3 position = base.transform.position;
					float x = position.x;
					Vector3 position2 = base.transform.position;
					float y = position2.y - 2.5f;
					Vector3 position3 = base.transform.position;
					transform.position = new Vector3(x, y, position3.z);
					UnityEngine.Object.Destroy(gameObject, 5f);
					base.PokerSpine.PlayDestroy(null);
					PlayDesk.Get().GiveUp();
				}
			}
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

		public override void RemoveAnimtor(UnityAction unityAction)
		{
			Vector3 position = base.transform.position;
			position.x += 1.5f;
			position.y -= 6f;
			baseCard.SetPokerFaceAllShow();
			baseCard.transform.DOMove(position, 0.8f).OnComplete(delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
				UnityEngine.Object.Destroy(baseCard.gameObject);
			});
			baseCard.transform.DORotate(Random.insideUnitSphere * 360f * 3f, 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		}

		public override void DestoryByBooster()
		{
			if (PlayDesk.Get().RemoveCard(baseCard))
			{
				PlayDesk.Get().ClearCard();
				PlayDesk.Get().CalcTopCard();
				OperatingHelper.Get().ClearStep();
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				UnityEngine.Object.Destroy(base.gameObject);
				RemoveAnimtor(delegate
				{
					PlayDesk.Get().DestopChanged();
				});
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearBomb);
			}
		}

		public override bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByGolden()
		{
			PlayDesk.Get().RemoveCard(baseCard);
			BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
			flyCard.UpdateOrderLayer(32667);
			DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
			flyCard.transform.parent = PlayDesk.Get().transform;
			PlayDesk.Get().LinkOnce(baseCard.transform.position);
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			Sequence sequence = DOTween.Sequence();
			sequence.Append(MoveCard(flyCard.transform, delegate
			{
				PlayDesk.Get().ClearCard();
				PlayDesk.Get().DestopChanged();
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearBomb);
				RemoveAnimtor(null);
				UnityEngine.Object.Destroy(base.gameObject);
				PlayDesk.Get().CalcTopCard();
			}));
			sequence.Join(RotateCard(flyCard.transform));
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(flyCard.gameObject);
			});
			PlayDesk.Get().AppendBusyTime(sequence.Duration());
			OperatingHelper.Get().ClearStep();
			return true;
		}

		public override bool DestoryByMatch(BaseCard card)
		{
			return DestoryByGolden();
		}

		public override bool DestoryByRocket()
		{
			if (PlayDesk.Get().RemoveCard(baseCard))
			{
				PlayDesk.Get().ClearCard();
				PlayDesk.Get().CalcTopCard();
				OperatingHelper.Get().ClearStep();
				PlayDesk.Get().LinkOnce(baseCard.transform.position);
				AudioUtility.GetSound().Play("Audios/Rope.mp3");
				UnityEngine.Object.Destroy(base.gameObject);
				RemoveAnimtor(delegate
				{
					PlayDesk.Get().DestopChanged();
				});
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearBomb);
			}
			return true;
		}
	}
}
