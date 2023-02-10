#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_EDITOR
namespace CodeStage.AntiCheat.EditorCode
{
	using System;

	internal class AllowedAssembly
	{
		public string name;
		public int[] hashes;

		public AllowedAssembly(string name, int[] hashes)
		{
			this.name = name;
			this.hashes = hashes;
		}

		public bool AddHash(int hash)
		{
			if (Array.IndexOf(hashes, hash) != -1) return false;

			var oldLen = hashes.Length;
			var newLen = oldLen + 1;

			var newHashesArray = new int[newLen];
			Array.Copy(hashes, newHashesArray, oldLen);

			hashes = newHashesArray;
			hashes[oldLen] = hash;

			return true;
		}

		public override string ToString()
		{
			return name + " (hashes: " + hashes.Length + ")";
		}
	}
}
#endif