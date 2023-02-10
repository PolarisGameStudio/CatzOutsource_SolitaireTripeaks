using Nightingale.Localization;
using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public class CoinCard : NumberCard
	{
		protected override string GetFont()
		{
			return "Prefabs/Pokers/CoinPoker";
		}

		protected override string GetBackground()
		{
			return "Prefabs/Pokers/CoinBackgroundPoker";
		}

		public override void DestoryCollect(bool step)
		{
			PlayDesk.Get().RemoveCard(this);
			int num = ScoringSystem.Get().Config.LevelTicketCoins / 2 + ScoringSystem.Get().GetNumber(OperatingHelper.Get().GetLink() - 1);
			SingletonClass<OnceGameData>.Get().StreaksCoins += num;
			SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(base.transform.position, LocalizationUtility.Get().GetString("Bonus"), num.ToString(), null);
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, num);
			PlayStreaksSystem.Get().StreaksOnce();
			HandCardSystem.Get().FromDeskToRightHandCard(this);
			PlayDesk.Get().ClearCard();
			PlayDesk.Get().DestopChanged();
			PlayDesk.Get().CalcTopCard();
			AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
			OperatingHelper.Get().ClearStep();
		}
	}
}
