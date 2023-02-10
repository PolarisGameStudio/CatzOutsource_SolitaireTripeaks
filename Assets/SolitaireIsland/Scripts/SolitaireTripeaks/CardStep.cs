using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class CardStep
	{
		public int TotalStreaksCount;

		public int StreaksLevel;

		public int StreaksGrade;

		public int RealLinkCount;

		public virtual void Undo(UnityAction unityAction = null)
		{
			HandCardSystem.Get().FlyRightToLeft(unityAction);
			OperatingHelper.Get().UndoLinkCount(TotalStreaksCount);
			PlayStreaksSystem.Get().SetCurrentLink(StreaksGrade, StreaksLevel, RealLinkCount);
		}
	}
}
