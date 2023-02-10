using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SaleButtonUI : MonoBehaviour
	{
		public Text TimeLabel;

		public Button SaleButton;

		private void Start()
		{
			UpdateButton();
			SingletonClass<OptimizationSystem>.Get().SaleChanged.AddListener(UpdateButton);
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		private void OnDestroy()
		{
			SingletonClass<OptimizationSystem>.Get().SaleChanged.RemoveListener(UpdateButton);
		}

		private void UpdateButton()
		{
			MenuUITopLeft.UpdateStoreUIRemianUI();
			SaleButton.onClick.RemoveAllListeners();
			base.gameObject.SetActive(SaleData.Get().HasSale());
			if (SaleData.Get().HasSale())
			{
				SaleButton.onClick.AddListener(delegate
				{
					SaleGroupScene.ShowSale();
				});
				UpdateTimeLeft();
			}
			BankButtonUI.UUUUpdateUI();
		}

		private void RepeatingUpdate()
		{
			UpdateTimeLeft();
		}

		private void UpdateTimeLeft()
		{
			if (base.gameObject.activeSelf && !SaleData.Get().HasSale("SaleSpecialStore"))
			{
				MenuUITopLeft.UpdateStoreUIRemianUI();
			}
			TimeSpan leftTime = SaleData.Get().GetLeftTime();
			TimeLabel.text = leftTime.TOShortString();
			if (leftTime.TotalSeconds <= 0.0)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
