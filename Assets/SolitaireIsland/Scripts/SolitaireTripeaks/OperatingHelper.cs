using Nightingale.Extensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class OperatingHelper : MonoBehaviour
	{
		private static OperatingHelper _OperatingHelper;

		private Stack<CardStep> _cardSteps = new Stack<CardStep>();

		private bool isBusy;

		public int TotalStreaksCount
		{
			get;
			private set;
		}

		public static OperatingHelper Get()
		{
			if (_OperatingHelper == null)
			{
			}
			return _OperatingHelper;
		}

		private void Awake()
		{
			_OperatingHelper = this;
		}

		public void UndoLinkCount(int count)
		{
			ScoringSystem.Get().UndoScoreUI(TotalStreaksCount);
			TotalStreaksCount = count;
		}

		public void ClearLinkCount()
		{
			TotalStreaksCount = 0;
		}

		public int GetLink()
		{
			TotalStreaksCount += GlobalBoosterUtility.Get().MultipleStreaks();
			AudioUtility.GetSound().Play($"Audios/combo{Mathf.Min(TotalStreaksCount, 21)}.mp3");
			return TotalStreaksCount;
		}

		public void AppendStep(CardStep step)
		{
			step.TotalStreaksCount = TotalStreaksCount;
			step.StreaksGrade = PlayStreaksSystem.Get().StreaksGrade;
			step.StreaksLevel = PlayStreaksSystem.Get().StreaksLevel;
			step.RealLinkCount = PlayStreaksSystem.Get().RealLinkCount;
			PlayScene.Get().SetUndoButtonVisable(visable: true);
			_cardSteps.Push(step);
		}

		public void ClearStep()
		{
			PlayScene.Get().SetUndoButtonVisable(visable: false);
			_cardSteps.Clear();
		}

		public void ClearStepOnlyOne()
		{
			if (_cardSteps.Count > 1)
			{
				CardStep t = _cardSteps.Pop();
				while (_cardSteps.Count > 0)
				{
					_cardSteps.Pop();
				}
				_cardSteps.Push(t);
			}
			PlayScene.Get().SetUndoButtonVisable(_cardSteps.Count > 0);
		}

		public bool HasSteps()
		{
			return _cardSteps.Count > 0;
		}

		public bool UndoStep()
		{
			if (_cardSteps.Count > 0)
			{
				if (PlayDesk.Get().IsAnimionBusy)
				{
					return false;
				}
				if (PlayDesk.Get().IsBusyHandBusy)
				{
					return false;
				}
				if (isBusy)
				{
					return false;
				}
				isBusy = true;
				CardStep cardStep = _cardSteps.Pop();
				cardStep.Undo(delegate
				{
					isBusy = false;
				});
				PlayScene.Get().SetUndoButtonVisable(_cardSteps.Count > 0);
				BaseCard[] array = PlayDesk.Get().Pokers.ToArray();
				BaseCard[] array2 = array;
				foreach (BaseCard baseCard in array2)
				{
					baseCard.Undo(cardStep is MatchStep);
				}
				HandCardSystem.Get().StorageHand.HasVaule(delegate(IStorageHandGroup e)
				{
					e.Undo();
				});
				return true;
			}
			return false;
		}
	}
}
