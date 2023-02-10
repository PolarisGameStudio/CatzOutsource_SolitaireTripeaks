using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class CommodityNumber
	{
		public CommoditySource source;

		public long total;

		public long current;

		public CommodityNumber(CommoditySource source, long current)
		{
			this.current = current;
			this.source = source;
			total = current;
		}

		public void Put(long count)
		{
			total += count;
			current += count;
		}

		public void Set(long count)
		{
			total = count;
			current = count;
		}

		public long GetCurrent()
		{
			return current;
		}

		public long Use(long count)
		{
			current -= count;
			long result = 0L;
			if (current <= 0)
			{
				result = -current;
				current = 0L;
			}
			return result;
		}
	}
}
