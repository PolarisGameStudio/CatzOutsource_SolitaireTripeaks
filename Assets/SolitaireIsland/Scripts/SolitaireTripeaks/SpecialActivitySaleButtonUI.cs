using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SpecialActivitySaleButtonUI : MonoBehaviour
	{
		public Image SaleButtonImage;

		public Button SaleButton;

		public Text TimeLeftLabel;

		private void Awake()
		{
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
			SaleButton.onClick.AddListener(delegate
			{
				SingletonBehaviour<SpecialActivityUtility>.Get().ShowSpecialActivitySale(null);
			});
		}

		private void RepeatingUpdate()
		{
			SaleButton.gameObject.SetActive(SingletonBehaviour<SpecialActivityUtility>.Get().IsActive());
			if (SaleButton.gameObject.activeSelf)
			{
				TimeLeftLabel.text = SpecialActivityConfig.Get().GetEndTime().Subtract(DateTime.Now)
					.TOString();
				SaleButtonImage.sprite = SingletonBehaviour<SpecialActivityUtility>.Get().GetSaleButtonSprite();
				SaleButtonImage.SetNativeSize();
			}
			BankButtonUI.UUUUpdateUI();
		}
	}
}
