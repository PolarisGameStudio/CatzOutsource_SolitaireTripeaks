using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class RightHandGroup : HandGroup
	{
		public override void AppendCard(BaseCard baseCard)
		{
			if (!(baseCard == null))
			{
				baseCard.CollectedToRightHand();
				BaseCard[] array = baseCards.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdateOrderLayer(1000 - i * 5 - 5);
				}
				baseCards.Push(baseCard);
				baseCard.Initialized();
				baseCard.TryOpenCard();
			}
		}

		public override BaseCard FlyCard()
		{
			if (baseCards.Count > 0)
			{
				return baseCards.Pop();
			}
			return null;
		}

		public override void DestoryWhenFaild(UnityAction unityAction)
		{
			base.IsDestory = true;
			int count = baseCards.Count;
			Sequence sequence = null;
			for (int i = 0; i < count; i++)
			{
				BaseCard baseCard = baseCards.Pop();
				baseCard.SetPokerFaceAllShow();
				Vector3 position = baseCard.transform.position;
				position.y += 11.3f;
				baseCard.transform.DORotate(Random.insideUnitSphere * 360f * 10f, 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)i * 0.02f);
				sequence.Append(baseCard.transform.DOMove(position, 2f));
				sequence.SetEase(Ease.Linear);
			}
			if (unityAction != null)
			{
				if (sequence == null)
				{
					unityAction();
				}
				sequence.OnComplete(delegate
				{
					unityAction();
				});
			}
		}

		public override void DestoryWhenSuccess(UnityAction unityAction)
		{
			base.IsDestory = true;
			int count = baseCards.Count;
			Sequence sequence = null;
			for (int i = 0; i < count; i++)
			{
				sequence = DOTween.Sequence();
				sequence.Append(baseCards.Pop().transform.DOMoveY(-7f, 0.2f));
			}
			if (unityAction != null)
			{
				if (sequence == null)
				{
					unityAction();
				}
				sequence.OnComplete(delegate
				{
					unityAction();
				});
			}
		}
	}
}
