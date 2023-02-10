using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class MathUtility
	{
		public static int CalcDays(DateTime dateTime1, DateTime dateTime2)
		{
			return dateTime1.Date.Subtract(dateTime2.Date).Days;
		}

		public static bool Probability(float probability)
		{
			int num = (int)(probability * 1000f);
			return UnityEngine.Random.Range(0, 1000) < num;
		}

		public static int Probability(List<int> probabilitys)
		{
			int num = UnityEngine.Random.Range(0, probabilitys.Sum() + 1);
			for (int i = 0; i < probabilitys.Count; i++)
			{
				num -= probabilitys[i];
				if (num <= 0)
				{
					return i;
				}
			}
			return 0;
		}

		public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve, float smoothness)
		{
			int num = 0;
			int num2 = 0;
			if (smoothness < 1f)
			{
				smoothness = 1f;
			}
			num = arrayToCurve.Count;
			num2 = num * Mathf.RoundToInt(smoothness) - 1;
			List<Vector3> list = new List<Vector3>(num2);
			float num3 = 0f;
			for (int i = 0; i < num2 + 1; i++)
			{
				num3 = Mathf.InverseLerp(0f, num2, i);
				List<Vector3> list2 = new List<Vector3>(arrayToCurve);
				for (int num4 = num - 1; num4 > 0; num4--)
				{
					for (int j = 0; j < num4; j++)
					{
						list2[j] = (1f - num3) * list2[j] + num3 * list2[j + 1];
					}
				}
				list.Add(list2[0]);
			}
			return list;
		}

		public static Vector3 CalcPosition(Vector3 center, Vector3 start, int turns, float t)
		{
			float num = Vector3.Distance(center, start);
			float num2 = Vector3.Angle(center, start) / 180f * 3.14159274f;
			num2 += 6.28318548f * (float)turns * t;
			return new Vector3(Mathf.Cos(num2) * num + center.x, Mathf.Sin(num2) * num + center.y, 0f);
		}

		public static float CalcAngle(Vector3 angle)
		{
			if (angle.x == 0f)
			{
				if (angle.y > 0f)
				{
					return 90f;
				}
				return 270f;
			}
			float num = Mathf.Atan(angle.y / angle.x);
			if (angle.x > 0f)
			{
				return num * 180f / 3.14159274f;
			}
			return 180f + num * 180f / 3.14159274f;
		}
	}
}
