using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class StoreLevelConfig
	{
		public List<string> Sales;

		public List<string> Stores;

		public List<string> Wilds;

		public List<string> Rockets;

		private static StoreLevelConfig group;

		public static StoreLevelConfig Get()
		{
			if (group == null)
			{
				group = JsonUtility.FromJson<StoreLevelConfig>(SingletonBehaviour<LoaderUtility>.Get().GetText("Configs/StoreLevelConfig.json"));
			}
			return group;
		}
	}
}
