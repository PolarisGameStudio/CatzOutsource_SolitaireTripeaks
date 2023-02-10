using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public enum BoosterType
	{
		None = -1,
		FreePlay = 0,
		Coins = 1,
		Wild = 2,
		Rocket = 3,
		FreeWheelPlay = 4,
		FreeSlotsPlay = 5,
		UnlimitedPlay = 6,
		ExpiredPlay = 7,
		BuyStep = 100,
		Undo = 101,
		Poker = 102,
		DoubleStar = 103,
		World = 104,
		ClubStore = 110,
		CoinBank = 111,
		WildEvent = 120,
		UnlimitedDoubleStar = 121,
		GuessGame = 122,
		RandomBooster = 599,
		[Type(typeof(FullFlipBooster))]
		FullFlip = 600,
		[Type(typeof(MultipleStreaksBooster))]
		MultipleStreaks = 601,
		[Type(typeof(BellaBlessingBooster))]
		BellaBlessing = 602,
		[Type(typeof(BurnRopeBooster))]
		BurnRope = 603,
		[Type(typeof(SnakeEliminateBooster))]
		SnakeEliminate = 604,
		[Type(typeof(BombEliminateBooster))]
		BombEliminate = 605,
		[Type(typeof(NumberGrowEliminateBooster))]
		NumberGrowEliminate = 606,
		[Type(typeof(CheeseTacticsBooster))]
		CheeseTactics = 607,
		[Type(typeof(VineEliminateBooster))]
		VineEliminate = 608,
		[Type(typeof(BatterEliminateBooster))]
		BatterEliminate = 609,
		[Type(typeof(ColorEliminateBooster))]
		ColorEliminate = 610,
		[Type(typeof(LockEliminateBooster))]
		LockEliminate = 611,
		[Type(typeof(DoubleSeagullBooster))]
		DoubleSeagull = 612,
		[Type(typeof(ContagionEliminateBooster))]
		NOContagion = 613,
		[Type(typeof(SkeletonEliminateBooster))]
		NOSkeleton = 614
	}
}
