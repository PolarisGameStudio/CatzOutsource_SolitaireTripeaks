using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class CoinBankData
	{
		public int CoinsInBank;

		public int Level;

		public bool IsOpened;

		public static CoinBankData Get()
		{
			if (SolitaireTripeaksData.Get().CoinBank == null)
			{
				SolitaireTripeaksData.Get().CoinBank = new CoinBankData();
			}
			return SolitaireTripeaksData.Get().CoinBank;
		}

		public bool IsBankRunning()
		{
			return IsBankOpening() && IsOpened;
		}

		public bool IsBankOpening()
		{
			return PlayData.Get().HasThanLevelData(0, 0, 10);
		}

		public bool IsFull()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			return CoinsInBank >= bankItem.MaxCoins;
		}

		public bool IsFull(int number)
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			return number >= bankItem.MaxCoins;
		}

		public int GetFullCoins()
		{
			return BankConfig.Get().GetBankItem(Level).MaxCoins;
		}

		public bool IsBankCollecting()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			return CoinsInBank >= bankItem.MinCoins;
		}

		public void PutCoinsInBank(int coins)
		{
			if (IsBankRunning())
			{
				BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
				CoinsInBank += Mathf.FloorToInt(BankConfig.Get().OnceGameCoins * (float)coins);
				CoinsInBank = Mathf.Min(bankItem.MaxCoins, CoinsInBank);
			}
		}

		public void RestBank()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			CoinsInBank = 0;
			bankItem = BankConfig.Get().GetBankItem(Level);
			Level++;
		}

		public string GetLowCoinsLabel()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			return $"{bankItem.MinCoins:N0}";
		}

		public string GetFullCoinsLabel()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			return $"{bankItem.MaxCoins:N0}";
		}

		public float GetProcess()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			if ((float)CoinsInBank < (float)bankItem.MinCoins)
			{
				return Mathf.Clamp01((float)CoinsInBank / (float)bankItem.MinCoins * 0.5f);
			}
			return 0.5f + Mathf.Clamp01((float)(CoinsInBank - bankItem.MinCoins) / (float)(bankItem.MaxCoins - bankItem.MinCoins)) * 0.5f;
		}

		public PurchasingPackage GetPurchasingPackage()
		{
			BankItemData bankItem = BankConfig.Get().GetBankItem(Level);
			PurchasingPackage purchasingPackage = new PurchasingPackage();
			purchasingPackage.commoditys = new PurchasingCommodity[1]
			{
				new PurchasingCommodity
				{
					boosterType = BoosterType.CoinBank,
					count = CoinsInBank
				}
			};
			purchasingPackage.id = bankItem.Id;
			purchasingPackage.Type = "CoinBank";
			purchasingPackage.Content = $"PigBank_{Level + 1}";
			return purchasingPackage;
		}
	}
}
