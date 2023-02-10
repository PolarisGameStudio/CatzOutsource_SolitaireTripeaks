using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class ColorCard : BaseCard
	{
		protected override string GetFont()
		{
			if (GetColor() == 0)
			{
				return "Prefabs/Pokers/BlackColorPoker";
			}
			return "Prefabs/Pokers/RedColorPoker";
		}

		public override int GetColor()
		{
			return Config.Index % 2;
		}

		public override int GetNumber()
		{
			return -10;
		}

		public override void CollectedToRightHand()
		{
			AudioUtility.GetSound().Play("Audios/Booster.mp3");
			if (Object.FindObjectOfType<BellaBlessingBooster>() != null)
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateBoosterUseEffectUI(BoosterType.BellaBlessing);
			}
			else
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString((GetColor() != 0) ? "Red Tips" : "Black Tips"));
			}
			base.CollectedToRightHand();
			PlayScene.Get().AppendProp<ColorMatchBooster>(GetColor());
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
