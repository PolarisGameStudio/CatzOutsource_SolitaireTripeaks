using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BankCoinsScene : SoundScene
	{
		public GameObject EmptyObject;

		public GameObject CollectObject;

		public GameObject FullObject;

		public Text CoinsLabel;

		public Text PriceLabel;

		public Button BuyButton;

		public Button OKButton;

		public Button CloseButton;

		public Text LowCoinsLabel;

		public Text FullCoinsLabel;

		public ProgressBarUI ProcessBar;

		public Text DescriptionLabel;

		private PurchasingPackage purchasingPackage;

		private void Awake()
		{
			UpdateUI();
		}

		private void CloseScene()
		{
			BankButtonUI bankButtonUI = Object.FindObjectOfType<BankButtonUI>();
			if (bankButtonUI != null)
			{
				SingletonClass<MySceneManager>.Get().Close(new PivotScaleEffect(bankButtonUI.transform.position));
			}
			else
			{
				SingletonClass<MySceneManager>.Get().Close();
			}
		}

		private void PriceRepeating()
		{
			if (SingletonBehaviour<UnityPurchasingHelper>.Get().IsInited)
			{
				if (PriceLabel != null)
				{
					PriceLabel.text = UnityPurchasingConfig.Get().GetLocalizedPriceString(purchasingPackage.id);
				}
			}
			else
			{
				Invoke("PriceRepeating", 1f);
			}
		}

		public void UpdateUI()
		{
			CoinsLabel.text = $"{CoinBankData.Get().CoinsInBank:N0}";
			LowCoinsLabel.text = CoinBankData.Get().GetLowCoinsLabel();
			FullCoinsLabel.text = CoinBankData.Get().GetFullCoinsLabel();
			ProcessBar.SetFillAmount(CoinBankData.Get().GetProcess());
			OKButton.onClick.AddListener(CloseScene);
			CloseButton.onClick.AddListener(CloseScene);
			string id = $"{CoinBankData.Get().Level}_CoinBank";
			if (AuxiliaryData.Get().HasView(id))
			{
				purchasingPackage = CoinBankData.Get().GetPurchasingPackage();
				PriceRepeating();
				OKButton.gameObject.SetActive(value: false);
				BuyButton.interactable = CoinBankData.Get().IsBankCollecting();
				if (BuyButton.interactable)
				{
					BuyButton.onClick.AddListener(delegate
					{
						SingletonBehaviour<UnityPurchasingHelper>.Get().OnPurchaseClicked(purchasingPackage);
					});
					if (CoinBankData.Get().IsFull())
					{
						DescriptionLabel.text = LocalizationUtility.Get("Localization_bank.json").GetString("Full_Bank");
						DescriptionLabel.rectTransform.anchoredPosition = new Vector2(0f, -250f);
						EmptyObject.SetActive(value: false);
						CollectObject.SetActive(value: false);
						FullObject.SetActive(value: true);
						ProcessBar.gameObject.SetActive(value: false);
					}
					else
					{
						DescriptionLabel.text = LocalizationUtility.Get("Localization_bank.json").GetString("Collect_Bank");
						EmptyObject.SetActive(value: false);
						CollectObject.SetActive(value: true);
						FullObject.SetActive(value: false);
					}
				}
				else
				{
					DescriptionLabel.text = string.Format(LocalizationUtility.Get("Localization_bank.json").GetString("Lower_Bank"), CoinBankData.Get().GetLowCoinsLabel());
					SingletonBehaviour<GlobalConfig>.Get().SetColor(BuyButton.transform, Color.gray);
					EmptyObject.SetActive(value: true);
					CollectObject.SetActive(value: false);
					FullObject.SetActive(value: false);
				}
			}
			else
			{
				DescriptionLabel.text = LocalizationUtility.Get("Localization_bank.json").GetString("Got_A_Bank");
				AuxiliaryData.Get().PutView(id);
				OKButton.gameObject.SetActive(value: true);
				BuyButton.gameObject.SetActive(value: false);
				CloseButton.gameObject.SetActive(value: false);
			}
		}
	}
}
