using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class LevelData
	{
		public bool StarComplete;

		public bool StarTime;

		public bool StarSteaks;

		public int WonCoins;

		public int Star
		{
			get
			{
				int num = 0;
				if (StarComplete)
				{
					num++;
				}
				if (StarTime)
				{
					num++;
				}
				if (StarSteaks)
				{
					num++;
				}
				return num;
			}
		}

		public RecordDataType PutWonCoins(int coins)
		{
			if (WonCoins >= coins)
			{
				return RecordDataType.Normal;
			}
			WonCoins = coins;
			return RecordDataType.NewRecord;
		}
	}
}
