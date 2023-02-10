using System;
using System.Collections.Generic;

namespace DragonBones
{
	internal static class Helper
	{
		public static readonly int INT16_SIZE = 2;

		public static readonly int UINT16_SIZE = 2;

		public static readonly int FLOAT_SIZE = 4;

		internal static void Assert(bool condition, string message)
		{
		}

		internal static void ResizeList<T>(this List<T> list, int count, T value = default(T))
		{
			if (list.Count == count)
			{
				return;
			}
			if (list.Count > count)
			{
				list.RemoveRange(count, list.Count - count);
				return;
			}
			for (int i = list.Count; i < count; i++)
			{
				list.Add(value);
			}
		}

		internal static List<float> Convert(this List<object> list)
		{
			List<float> list2 = new List<float>();
			for (int i = 0; i < list.Count; i++)
			{
				list2[i] = float.Parse(list[i].ToString());
			}
			return list2;
		}

		internal static bool FloatEqual(float f0, float f1)
		{
			float num = Math.Abs(f0 - f1);
			return num < 1E-09f;
		}
	}
}
