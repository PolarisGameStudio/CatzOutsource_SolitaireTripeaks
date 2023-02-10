using System;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class SystemTime
	{
		private static TimeSpan offsetTime = TimeSpan.FromSeconds(0.0);

		public static DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow.Add(offsetTime);
			}
			set
			{
				IsConnect = true;
				offsetTime = value.Subtract(DateTime.UtcNow);
				UnityEngine.Debug.LogWarningFormat("服务器时间差值：{0}", offsetTime);
			}
		}

		public static DateTime Now => DateTime.Now.Add(offsetTime);

		public static bool IsConnect
		{
			get;
			set;
		}
	}
}
