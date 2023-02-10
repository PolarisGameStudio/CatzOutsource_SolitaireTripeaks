using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SpecialActivitySaleScene : SoundScene
	{
		public Text TimeLeftLabel;

		private void Awake()
		{
			base.IsStay = true;
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
			SpecialActivityData.Get().OpenSpecialSaleNumbers++;
			SingletonBehaviour<UnityPurchasingHelper>.Get().Append(PurchasingCompleted);
			AuxiliaryData.Get().PutDailyCompleted("Special_Sale");
		}

		private void PurchasingCompleted(string transactionID, PurchasingPackage package)
		{
			SpecialActivityData.Get().SpecialSaleNumbers++;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingCompleted);
		}

		private void RepeatingUpdate()
		{
			TimeLeftLabel.text = SpecialActivityConfig.Get().GetEndTime().Subtract(DateTime.Now)
				.TOString();
		}

		public void OnCloseClick()
		{
			SpecialActivitySaleButtonUI specialActivitySaleButtonUI = UnityEngine.Object.FindObjectOfType<SpecialActivitySaleButtonUI>();
			if (specialActivitySaleButtonUI != null)
			{
				SingletonClass<MySceneManager>.Get().Close(new PivotScaleEffect(specialActivitySaleButtonUI.transform.position));
			}
			else
			{
				SingletonClass<MySceneManager>.Get().Close();
			}
		}
	}
}
