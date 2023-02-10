using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class BankItemData
	{
		[Header("价格信息")]
		public string Id;

		[Header("最低购买金币数目")]
		public int MinCoins;

		[Header("最大购买金币数目")]
		public int MaxCoins;
	}
}
