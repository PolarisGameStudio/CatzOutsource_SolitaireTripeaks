using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class NightingaleTime
	{
		private static Dictionary<string, float> TimeScale = new Dictionary<string, float>();

		public static float DeltaTime
		{
			get
			{
				float num = Time.deltaTime;
				foreach (float value in TimeScale.Values)
				{
					num *= value;
				}
				return num;
			}
		}

		public static void Append(string key, float scale = 0f)
		{
			if (TimeScale.ContainsKey(key))
			{
				TimeScale[key] = scale;
			}
			else
			{
				TimeScale.Add(key, scale);
			}
		}

		public static void Remove(string key)
		{
			if (TimeScale.ContainsKey(key))
			{
				TimeScale.Remove(key);
			}
		}
	}
}
