using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class PlayLevel
	{
		public int Index;

		public int world;

		public int chapter;

		public int level;

		public float completionCoins;

		public float timeCoins;

		public float streakCoins;

		public float time;

		public float rewardCoins;

		public float BuyStepCoins;

		public float UndoCoins;

		public PlayBoosterGroup LevelBoosters;
	}
}
