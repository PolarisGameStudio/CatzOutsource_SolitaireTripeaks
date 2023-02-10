using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class RankLevelDataGroup
	{
		public LevelData LevelData;

		public int Level;

		public int Stars()
		{
			if (LevelData == null)
			{
				return 0;
			}
			return LevelData.Star;
		}
	}
}
