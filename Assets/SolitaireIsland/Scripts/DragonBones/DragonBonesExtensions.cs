using System;
using System.Collections.Generic;

namespace DragonBones
{
	public static class DragonBonesExtensions
	{
		public static List<Tnew> ConvertAll<T, Tnew>(this List<T> src, Func<T, Tnew> fconv)
		{
			List<Tnew> list = new List<Tnew>(src.Count);
			for (int i = 0; i < src.Count; i++)
			{
				list.Add(fconv(src[i]));
			}
			return list;
		}
	}
}
