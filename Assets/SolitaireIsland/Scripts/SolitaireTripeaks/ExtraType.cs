using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public enum ExtraType
	{
		[Type(typeof(BatterExtra))]
		Batter = 1,
		[Type(typeof(GrowBatterExtra))]
		GrowBatter = 2,
		[Type(typeof(BombExtra))]
		Bomb = 3,
		[Type(typeof(ColorExtra))]
		Color = 4,
		[Type(typeof(ContagionExtra))]
		Contagion = 5,
		[Type(typeof(RopeExtra))]
		Rope = 7,
		[Type(typeof(KeyExtra))]
		Key = 9,
		[Type(typeof(LockExtra))]
		Lock = 10,
		[Type(typeof(VineExtra))]
		Vine = 13,
		[Type(typeof(NumberGrowExtra))]
		NumberGrow = 14,
		[Type(typeof(SwallowedExtra))]
		Swallowed = 0x10,
		[Type(typeof(ClearanceExtra))]
		Clearance = 21,
		[Type(typeof(SkeletonExtra))]
		Skeleton = 22
	}
}
