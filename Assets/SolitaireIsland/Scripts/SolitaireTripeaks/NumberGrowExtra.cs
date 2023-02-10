using DG.Tweening;
using Nightingale.U2D;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class NumberGrowExtra : DebuffExtra
	{
		private bool growing;

		private SpriteRenderer left_bottom;

		private SpriteRenderer right_top;

		private SpriteRenderer spriteRenderer;

		protected override void StartInitialized()
		{
			growing = (base.Index % 2 == 0);
			string text = (!growing) ? "reduce" : "plus";
			spriteRenderer = CreateSpriteRenderer(typeof(NumberGrowExtra).Name);
			spriteRenderer.sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, "Sprites/SpecialPoker").GetSprite(text);
			Sprite sprite = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(typeof(PlayScene).Name, "Sprites/SpecialPoker").GetSprite(text + "2");
			left_bottom = CreateSpriteRenderer("left_bottom");
			left_bottom.sprite = sprite;
			left_bottom.transform.localPosition = new Vector3(-0.6f, -1.05f, 0f);
			right_top = CreateSpriteRenderer("right_top");
			right_top.sprite = sprite;
			right_top.transform.localPosition = new Vector3(0.6f, 1.05f, 0f);
			Sequence sequence = DOTween.Sequence();
			Sequence s = sequence;
			Transform transform = left_bottom.transform;
			Vector3 localPosition = left_bottom.transform.localPosition;
			s.Append(transform.DOLocalMoveY(localPosition.y + ((!growing) ? (-0.05f) : 0.05f), 0.7f));
			Sequence s2 = sequence;
			Transform transform2 = left_bottom.transform;
			Vector3 localPosition2 = left_bottom.transform.localPosition;
			s2.Append(transform2.DOLocalMoveY(localPosition2.y, 0.7f));
			sequence.SetEase(Ease.Linear);
			sequence.SetLoops(-1);
			sequence = DOTween.Sequence();
			Sequence s3 = sequence;
			Transform transform3 = right_top.transform;
			Vector3 localPosition3 = right_top.transform.localPosition;
			s3.Append(transform3.DOLocalMoveY(localPosition3.y + ((!growing) ? (-0.05f) : 0.05f), 0.7f));
			Sequence s4 = sequence;
			Transform transform4 = right_top.transform;
			Vector3 localPosition4 = right_top.transform.localPosition;
			s4.Append(transform4.DOLocalMoveY(localPosition4.y, 0.7f));
			sequence.SetEase(Ease.Linear);
			sequence.SetLoops(-1);
		}

		public override void RemoveAnimtor(UnityAction unityAction)
		{
			left_bottom.DOFade(0f, 0.3f);
			left_bottom.transform.DOScale(2f, 0.3f).OnComplete(delegate
			{
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearNumberUpDown, (!growing) ? 1 : 0);
				if (unityAction != null)
				{
					unityAction();
				}
			});
			right_top.DOFade(0f, 0.3f);
			right_top.transform.DOScale(2f, 0.3f);
			spriteRenderer.DOFade(0f, 0.3f);
			spriteRenderer.transform.DOScale(2f, 0.3f);
		}

		public override void OnHandChange()
		{
			if (PlayDesk.Get().Uppers.Contains(baseCard))
			{
				int suit = baseCard.GetSuit();
				int number = baseCard.GetNumber();
				AudioUtility.GetSound().Play((!growing) ? "Audios/Extra_Number_DOWN.mp3" : "Audios/Extra_Number_UP.mp3");
				number = (number + 13 + (growing ? 1 : (-1))) % 13 + 1;
				baseCard.SetIndex(number + suit * 13);
				left_bottom.transform.DOScale(1.6f, 0.2f).OnComplete(delegate
				{
					left_bottom.transform.DOScale(1f, 0.2f);
				});
				right_top.transform.DOScale(1.6f, 0.2f).OnComplete(delegate
				{
					right_top.transform.DOScale(1f, 0.2f);
				});
			}
		}

		public override void OnUndo(bool match)
		{
			if (PlayDesk.Get().Uppers.Contains(baseCard) && !match)
			{
				int suit = baseCard.GetSuit();
				int number = baseCard.GetNumber();
				number = (number + 13 + ((!growing) ? 1 : (-1))) % 13 + 1;
				baseCard.SetIndex(number + suit * 13);
			}
		}
	}
}
