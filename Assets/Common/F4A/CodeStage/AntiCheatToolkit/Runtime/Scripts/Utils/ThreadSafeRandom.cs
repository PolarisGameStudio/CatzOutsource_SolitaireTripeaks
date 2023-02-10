#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2015-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Utils
{
	using System;

	public class ThreadSafeRandom
	{
		private static readonly Random Global = new Random();

		[ThreadStatic]
		private static Random local;

		public static int Next(int minInclusive, int maxExclusive)
		{
			var inst = local;
			if (inst != null) return inst.Next(minInclusive, maxExclusive);

			int seed;

			lock (Global)
				seed = Global.Next();

			local = inst = new Random(seed);
			return inst.Next(minInclusive, maxExclusive);
		}

		public static int Next()
		{
			return Next(1, int.MaxValue);
		}

		public static int Next(int maxExclusive)
		{
			return Next(1, maxExclusive);
		}
	}
}