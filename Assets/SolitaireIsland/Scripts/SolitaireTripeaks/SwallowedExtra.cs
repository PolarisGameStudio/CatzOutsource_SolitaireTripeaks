using DG.Tweening;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class SwallowedExtra : TimeExtra
	{
		private SpriteRenderer MouseSpriteRenderer;

		private Transform MouseTransform;

		protected override void StartInitialized()
		{
			SpriteManager assetComponent = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, "Sprites/SpecialPoker");
			GameObject gameObject = new GameObject();
			MouseTransform = gameObject.transform;
			MouseTransform.SetParent(base.transform, worldPositionStays: false);
			MouseSpriteRenderer = CreateSpriteRenderer("Mouse", MouseTransform);
			MouseSpriteRenderer.transform.localPosition = new Vector3(0.4f, 1f, 0f);
			MouseSpriteRenderer.sprite = assetComponent.GetSprite("Mouse");
			base.Index = 14;
			base.StartInitialized();
			RestTime(2);
		}

		public override void SetState(bool state)
		{
			base.SetState(state);
			if (state)
			{
				Sequence s = DOTween.Sequence();
				s.AppendInterval(0.4f);
				s.Append(MouseTransform.DOScale(1.2f, 0.3f));
				s.Append(MouseTransform.DOScale(1f, 0.1f));
			}
		}

		protected override bool IsRunning()
		{
			return base.IsRunning() && HandCardSystem.Get()._LeftHandGroup.GetTop() != null;
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

		public override void UpdateOrderLayer(int zIndex, int index)
		{
			int num = 1;
			foreach (SpriteRenderer spriteRenderer in base.spriteRenderers)
			{
				num++;
				spriteRenderer.sortingOrder = zIndex + index + num;
			}
		}

		protected override void LifeUpdate()
		{
		}

		protected override void LifeStep()
		{
		}

		protected override void LifeOver()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(MouseTransform.DOScale(1.2f, 0.3f));
			sequence.Append(MouseTransform.DOScale(1f, 0.1f));
			GameObject mouseObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/Mouse"));
			Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(-500f, 0f));
			Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width + 500, 0f));
			Vector3 position2 = HandCardSystem.Get()._LeftHandGroup.transform.position;
			Vector3 endValue = new Vector3(2.6f + position2.x, left.y, 0f);
			left.z = (position.z = 0f);
			AudioUtility.GetSound().Play("Audios/mouse_card.mp3");
			mouseObject.transform.position = position;
			PlayDesk.Get().AppendBusyTime(0.5f);
			PlayDesk.Get().AppendStepBusyTime(0.5f);
			mouseObject.transform.DOMove(endValue, 0.5f).OnComplete(delegate
			{
				BaseCard baseCard = HandCardSystem.Get().FlyLeftCard();
				if (baseCard == null)
				{
					mouseObject.transform.DOMove(left, 0.5f).OnComplete(delegate
					{
						UnityEngine.Object.Destroy(mouseObject);
					});
				}
				else
				{
					baseCard.UpdateOrderLayer(1010);
					sequence = DOTween.Sequence();
					sequence.AppendCallback(delegate
					{
						baseCard.transform.SetParent(mouseObject.transform, worldPositionStays: true);
						baseCard.SetPokerFaceAllShow();
					});
					sequence.Append(baseCard.transform.DOLocalRotate(new Vector3(30f, 180f, -20f), 0.1f));
					sequence.AppendInterval(0.2f);
					sequence.Append(mouseObject.transform.DOMove(left, 0.8f));
					sequence.OnComplete(delegate
					{
						UnityEngine.Object.Destroy(baseCard.gameObject);
						UnityEngine.Object.Destroy(mouseObject);
					});
					PlayDesk.Get().AppendBusyTime(sequence.Duration());
					PlayDesk.Get().AppendStepBusyTime(sequence.Duration());
				}
			});
			RestTime();
		}

		public override void DestoryByBooster()
		{
			playing = false;
			UnityEngine.Object.Destroy(base.gameObject);
			baseCard.RemoveExtra(this);
			baseCard.DestoryCollect(step: false);
		}

		public override bool DestoryByColor()
		{
			return DestoryByRocket();
		}

		public override bool DestoryByGolden()
		{
			playing = false;
			PlayDesk.Get().RemoveCard(baseCard);
			baseCard.RemoveExtra(this);
			UnityEngine.Object.Destroy(base.gameObject);
			BaseCard flyCard = HandCardSystem.Get().FlyRightCard();
			flyCard.UpdateOrderLayer(32667);
			DOTween.Kill($"MoveToHandCard_{flyCard.GetInstanceID()}");
			flyCard.transform.parent = PlayDesk.Get().transform;
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			Sequence sequence = DOTween.Sequence();
			sequence.Append(MoveCard(flyCard.transform, delegate
			{
				baseCard.DestoryCollect(step: false);
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
			playing = false;
			baseCard.RemoveExtra(this);
			UnityEngine.Object.Destroy(base.gameObject);
			PlayDesk.Get().RemoveCard(baseCard);
			OperatingHelper.Get().ClearStep();
			AudioUtility.GetSound().Play("Audios/Rope.mp3");
			baseCard.DestoryCollect(step: false);
			return true;
		}
	}
}
