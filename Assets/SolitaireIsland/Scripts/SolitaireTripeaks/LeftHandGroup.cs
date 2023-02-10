using DG.Tweening;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class LeftHandGroup : HandGroup
	{
		private const float min = 0.2f;

		private const float width = 6f;

		private const string _UpdateMagicEye = "_UpdateMagicEye";

		public SpriteRenderer FrontRenderer;

		public SpriteRenderer BackgroundRenderer;

		public Color StartColor;

		public Color EndColor;

		public bool IsMagicEye
		{
			get;
			private set;
		}

		private void Awake()
		{
			SpriteRenderer backgroundRenderer = BackgroundRenderer;
			Sprite sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite("back");
			FrontRenderer.sprite = sprite;
			backgroundRenderer.sprite = sprite;
			SpriteRenderer frontRenderer = FrontRenderer;
			Color clear = Color.clear;
			BackgroundRenderer.color = clear;
			frontRenderer.color = clear;
		}

		public override void AppendCard(BaseCard card)
		{
			if (!(card == null))
			{
				BaseCard[] array = baseCards.ToArray();
				baseCards.Push(card);
				card.Initialized();
				for (int num = array.Length - 1; num >= 0; num--)
				{
					array[num].UpdateOrderLayer(1000 - num * 5 - 5);
				}
				card.TryOpenCard(open: false);
				card.transform.SetParent(base.transform, worldPositionStays: true);
				card.transform.localScale = Vector3.one;
			}
		}

		public void InsertCard(BaseCard card)
		{
			if (!(card == null))
			{
				BaseCard[] array = baseCards.ToArray();
				baseCards.Clear();
				baseCards.Push(card);
				card.Initialized();
				for (int num = array.Length - 1; num >= 0; num--)
				{
					array[num].UpdateOrderLayer(1000 - num * 5 - 5);
					baseCards.Push(array[num]);
				}
				card.TryOpenCard(open: false);
				card.transform.SetParent(base.transform, worldPositionStays: true);
				card.transform.localScale = Vector3.one;
			}
		}

		public override BaseCard FlyCard()
		{
			if (baseCards.Count > 0)
			{
				BaseCard baseCard = baseCards.Pop();
				baseCard.UpdateFaceWithConfig();
				UpdatePosition();
				UpdateMagicEye();
				return baseCard;
			}
			return null;
		}

		public override void DestoryWhenFaild(UnityAction unityAction)
		{
			DOTween.Kill("_UpdateMagicEye");
			SpriteRenderer frontRenderer = FrontRenderer;
			Color clear = Color.clear;
			BackgroundRenderer.color = clear;
			frontRenderer.color = clear;
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
			DOTween.Kill("_UpdateMagicEye");
			SpriteRenderer frontRenderer = FrontRenderer;
			Color clear = Color.clear;
			BackgroundRenderer.color = clear;
			frontRenderer.color = clear;
			base.IsDestory = true;
			int count = baseCards.Count;
			Sequence sequence = null;
			for (int i = 0; i < count; i++)
			{
				BaseCard baseCard = baseCards.Pop();
				int index = i;
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)i * 0.2f);
				sequence.AppendCallback(delegate
				{
					AudioUtility.GetSound().Play("Audios/buy_booster.mp3");
				});
				sequence.Append(baseCard.transform.DOMoveY(-3f, 0.2f));
				sequence.AppendCallback(delegate
				{
					ScoringSystem.Get().CreateScoreUIByHand(baseCard.transform.position, index + 1, null);
					UnityEngine.Object.Destroy(baseCard.gameObject);
				});
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

		public override void UpdatePosition()
		{
			BaseCard[] array = baseCards.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateOrderLayer(1000 - i * 5);
				array[i].transform.localPosition = new Vector3(GetPositionX(i), 0f, 0f);
				array[i].transform.eulerAngles = Vector3.zero;
			}
		}

		public override float GetPositionX(int index)
		{
			float num = 0.2f * (float)index;
			if (num > 6f)
			{
				num = 6f;
			}
			return 0f - num;
		}

		public void UpdateMagicEye()
		{
			if (IsMagicEye)
			{
				DOTween.Kill("_UpdateMagicEye");
				baseCards.ToList().ForEach(delegate(BaseCard e)
				{
					e.UpdateFaceWithConfig();
				});
				BaseCard top = GetTop();
				if (top == null)
				{
					FrontRenderer.color = Color.clear;
					BackgroundRenderer.color = Color.clear;
					return;
				}
				top.SetMagicEye();
				FrontRenderer.color = EndColor;
				BackgroundRenderer.color = Color.white;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(FrontRenderer.DOColor(StartColor, 1f));
				sequence.AppendInterval(0.5f);
				sequence.Append(FrontRenderer.DOColor(EndColor, 1f));
				sequence.AppendInterval(1.5f);
				sequence.SetLoops(-1);
				sequence.SetId("_UpdateMagicEye");
			}
		}

		public void OpenMagicEye()
		{
			IsMagicEye = true;
		}
	}
}
