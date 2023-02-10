using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class BankButtonUI : MonoBehaviour
	{
		public Sprite GraySprite;

		public Sprite LightSprite;

		public Text CoinsLabel;

		public Image ButtonImage;

		public Button BankButton;

		private static BankButtonUI bankButtonUI;

		private void Awake()
		{
			bankButtonUI = this;
			UpdateUI();
		}

		private void UpdateUI()
		{
			base.gameObject.SetActive(CoinBankData.Get().IsBankRunning());
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			if (CoinBankData.Get().IsFull())
			{
				CoinsLabel.text = LocalizationUtility.Get("Localization_bank.json").GetString("Full");
			}
			else
			{
				CoinsLabel.text = $"{CoinBankData.Get().CoinsInBank:N0}";
			}
			ButtonImage.sprite = ((!CoinBankData.Get().IsBankCollecting()) ? GraySprite : LightSprite);
			BankButton.onClick.RemoveAllListeners();
			BankButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<BankCoinsScene>("Scenes/BankCoinsScene");
			});
			RectTransform rectTransform = base.transform.parent as RectTransform;
			rectTransform.anchoredPosition = new Vector2(0f, 0f);
			SpecialActivitySaleButtonUI specialActivitySaleButtonUI = UnityEngine.Object.FindObjectOfType<SpecialActivitySaleButtonUI>();
			if (specialActivitySaleButtonUI != null && specialActivitySaleButtonUI.SaleButton.gameObject.activeInHierarchy)
			{
				rectTransform.anchoredPosition = new Vector2(0f, -200f);
				return;
			}
			SpecialActivityButton specialActivityButton = UnityEngine.Object.FindObjectOfType<SpecialActivityButton>();
			if (specialActivityButton != null && specialActivityButton.Button.gameObject.activeInHierarchy)
			{
				rectTransform.anchoredPosition = new Vector2(0f, -200f);
			}
		}

		public static void UUUUpdateUI()
		{
			if (bankButtonUI != null)
			{
				bankButtonUI.UpdateUI();
			}
		}
	}
}
