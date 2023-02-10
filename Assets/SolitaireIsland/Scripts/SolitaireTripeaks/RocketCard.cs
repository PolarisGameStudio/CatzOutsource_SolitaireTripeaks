using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class RocketCard : BaseCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/RocketPoker";
		}

		public override void CollectedToRightHand()
		{
			OperatingHelper.Get().ClearStep();
			AudioUtility.GetSound().Play("Audios/Booster.mp3");
			if (Object.FindObjectOfType<BellaBlessingBooster>() != null)
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateBoosterUseEffectUI(BoosterType.BellaBlessing);
			}
			else
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString("Rocket Tips"));
			}
			PlayScene.Get().AppendProp<RocketBooster>();
			base.CollectedToRightHand();
		}

		public override bool StayInTop()
		{
			return false;
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
