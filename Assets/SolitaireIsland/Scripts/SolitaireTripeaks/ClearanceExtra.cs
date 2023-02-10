using UnityEngine;

namespace SolitaireTripeaks
{
	public class ClearanceExtra : GainExtra
	{
		protected override string PokerPrefab()
		{
			return "Prefabs/Extras/VineSpine";
		}

		public override bool DestoryByRocket()
		{
			baseCard.RemoveExtra(this);
			UnityEngine.Object.Destroy(base.gameObject);
			baseCard.DestoryCollect(step: false);
			OperatingHelper.Get().ClearStep();
			if (PlayDesk.Get().GetAllExtras<ClearanceExtra>().Count == 0)
			{
				PlayDesk.Get().LevelCompleted();
			}
			return true;
		}
	}
}
