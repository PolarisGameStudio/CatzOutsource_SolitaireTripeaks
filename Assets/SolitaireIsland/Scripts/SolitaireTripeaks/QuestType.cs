using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public enum QuestType
	{
		[Type(typeof(CollectNumberCardTarget))]
		CollectNumberCard,
		[Type(typeof(CollectColorCardTarget))]
		CollectColorCard,
		[Type(typeof(CollectShapeCardTarget))]
		CollectSuitCard,
		[Type(typeof(PlayTarget))]
		Play,
		[Type(typeof(WinGameTarget))]
		WinGame,
		[Type(typeof(WinCoinsTarget))]
		WinCoins,
		[Type(typeof(WinGameInSceneTarget))]
		WinGameInScene,
		[Type(typeof(UseWildTarget))]
		UseWild,
		[Type(typeof(UseRocketTarget))]
		UseRocket,
		[Type(typeof(ClearRopeTarget))]
		ClearRope,
		[Type(typeof(ClearBombTarget))]
		ClearBomb,
		[Type(typeof(ClearSankeTarget))]
		ClearSanke,
		[Type(typeof(ClearNumberUpDownTarget))]
		ClearNumberUpDown,
		[Type(typeof(GetNewStarTarget))]
		GetNewStar,
		[Type(typeof(GetStreakTarget))]
		GetStreak,
		[Type(typeof(WinRowTarget))]
		WinRow
	}
}
