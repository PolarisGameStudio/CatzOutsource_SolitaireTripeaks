using Nightingale.Utilitys;

namespace SolitaireTripeaks
{
	public enum CardType
	{
		[Type(typeof(NumberCard))]
		Number = 0,
		[Type(typeof(WildCard))]
		Wild = 1,
		[Type(typeof(ColorCard))]
		Color = 2,
		[Type(typeof(ScarecrowCard))]
		Scarecrow = 3,
		[Type(typeof(ForkCard))]
		Fork = 4,
		[Type(typeof(SnakeCard))]
		Snake = 5,
		[Type(typeof(FoxCard))]
		Fox = 10,
		[Type(typeof(GoldenCard))]
		Golden = 11,
		[Type(typeof(RocketCard))]
		Rocket = 12,
		[Type(typeof(SellRocketCard))]
		SellRocket = 13,
		[Type(typeof(CoinCard))]
		Coin = 14,
		[Type(typeof(SwallowedCard))]
		Swallowed = 0xF,
		[Type(typeof(SeagullCard))]
		Seagull = 0x10
	}
}
