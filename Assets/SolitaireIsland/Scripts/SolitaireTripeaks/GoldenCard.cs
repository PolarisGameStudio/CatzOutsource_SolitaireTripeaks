using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class GoldenCard : BaseCard
	{
		protected override void StartInitialized()
		{
			Config.Index = -100;
		}

		protected override string GetFont()
		{
			return "Prefabs/Pokers/GoldenPoker";
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
				SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString("Golden Tips"));
			}
			base.CollectedToRightHand();
		}

		public override bool CalcClickMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
