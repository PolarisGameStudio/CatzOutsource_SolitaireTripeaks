using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[CreateAssetMenu(fileName = "BankConfig.asset", menuName = "Nightingale/Bank Config", order = 1)]
	public class BankConfig : ScriptableObject
	{
		public List<BankItemData> Banks;

		[Header("一次游戏，奖励的金币数目")]
		public float OnceGameCoins;

		private static BankConfig bankConfig;

		public static BankConfig Get()
		{
			if (bankConfig == null)
			{
				bankConfig = SingletonBehaviour<LoaderUtility>.Get().GetAsset<BankConfig>("Configs/BankConfig");
			}
			return bankConfig;
		}

		public BankItemData GetBankItem(int level)
		{
			level = Mathf.Min(level, Banks.Count - 1);
			return Banks[level];
		}
	}
}
