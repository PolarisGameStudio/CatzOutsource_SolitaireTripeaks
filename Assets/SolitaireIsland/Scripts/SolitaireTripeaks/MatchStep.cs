using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class MatchStep : CardStep
	{
		private List<BaseCard> opening;

		public MatchStep(List<BaseCard> opening)
		{
			this.opening = opening;
		}

		public override void Undo(UnityAction unityAction = null)
		{
			foreach (BaseCard item in opening)
			{
				item.TryOpenCard(open: false);
			}
			BaseCard baseCard = HandCardSystem.Get().FlyRightCard();
			PlayStreaksSystem.Get().SetCurrentLink(StreaksGrade, StreaksLevel, RealLinkCount);
			baseCard.transform.SetParent(PlayDesk.Get().transform, worldPositionStays: true);
			DOTween.Kill($"MoveToHandCard_{baseCard.GetInstanceID()}");
			Sequence sequence = DOTween.Sequence();
			sequence.Append(baseCard.transform.DORotate(Vector3.forward * 360f * Random.Range(3, 5) + baseCard.Config.EulerAngles * Vector3.forward, 0.7f, RotateMode.FastBeyond360));
			sequence.Join(baseCard.transform.DOLocalJump(baseCard.Config.GetPosition(), 4f, 1, 0.7f));
			sequence.Join(baseCard.transform.DOScale(1f, 0.7f));
			sequence.SetEase(Ease.Linear);
			sequence.OnComplete(delegate
			{
				baseCard.Initialized();
				PlayDesk.Get().CalcTopCard();
				PlayDesk.Get().DestopChanged();
				if (unityAction != null)
				{
					unityAction();
				}
			});
			PlayDesk.Get().Pokers.Add(baseCard);
			OperatingHelper.Get().UndoLinkCount(TotalStreaksCount);
			PlayDesk.Get().AppendBusyTime(sequence.Duration());
		}
	}
}
