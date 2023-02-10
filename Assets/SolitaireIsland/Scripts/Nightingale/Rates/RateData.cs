using System;
using UnityEngine;

namespace Nightingale.Rates
{
	[Serializable]
	public class RateData
	{
		public int Star;

		public string Reason;

		public string Platform;

		public RateData()
		{
			Platform = Application.platform.ToString();
		}
	}
}
