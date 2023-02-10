#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Common
{
	using System;

	[Serializable]
	public struct ACTkByte4
	{
		public byte b1;
		public byte b2;
		public byte b3;
		public byte b4;

		public void Shuffle()
		{
			var buffer = b2;
			b2 = b3;
			b3 = buffer;
		}

		public void UnShuffle()
		{
			var buffer = b3;
			b3 = b2;
			b2 = buffer;
		}
	}
}