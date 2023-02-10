using Nightingale.Inputs;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SaleScene : SoundScene
	{
		public Text TimeLeftLabel;

		public SaleItemUI[] saleItemUIs;

		private SaleInfo SaleInfo;

		private UnityAction OnClosedAction;

		private DateTime TimeLeft;

		private Button ClosedButton;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (SaleInfo != null)
			{
				SaleInfo.SaleConfig.DestoryAssetBundle();
			}
			if (OnClosedAction != null)
			{
				OnClosedAction();
			}
			SingletonBehaviour<UnityPurchasingHelper>.Get().Remove(PurchasingCompleted);
		}

		private void PurchasingCompleted(string transactionID, PurchasingPackage package)
		{
			if (SaleInfo != null && base.gameObject.activeSelf)
			{
				SaleInfo.PutBuy(package.id);
			}
		}

		private void Update()
		{
			TimeSpan timeSpan = TimeLeft.Subtract(DateTime.Now);
			TimeLeftLabel.text = timeSpan.TOShortString();
		}

		public void OnStart(SaleInfo saleInfo, DateTime dateTime)
		{
			base.IsStay = true;
			SaleData.Get().saleOpenCount++;
			TimeLeft = dateTime;
			SaleInfo = saleInfo;
			RectTransform rectTransform = base.transform.Find("Canvas").Find("Parent") as RectTransform;
			ClosedButton = rectTransform.Find("CloseButton").GetComponent<Button>();
			if (SingletonClass<MySceneManager>.Get().GetTopScene() == this)
			{
				ClosedButton.gameObject.AddComponent<EscapeButtonControler>();
				JoinEffectCloseButton component = ClosedButton.gameObject.GetComponent<JoinEffectCloseButton>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				ClosedButton.onClick.AddListener(delegate
				{
					SaleButtonUI saleButtonUI = UnityEngine.Object.FindObjectOfType<SaleButtonUI>();
					if (saleButtonUI != null)
					{
						SingletonClass<MySceneManager>.Get().Close(new PivotScaleEffect(saleButtonUI.transform.position));
					}
					else
					{
						SingletonClass<MySceneManager>.Get().Close();
					}
				});
			}
			SaleItemConfig saleConfig = SaleInfo.SaleConfig;
			switch (saleConfig.sceneName)
			{
			case "remote/sale/coins.asset":
				base.gameObject.AddComponent<OldCoinSaleUI>();
				break;
			case "remote/sale/wilds.asset":
				base.gameObject.AddComponent<OldWildSaleUI>();
				break;
			case "remote/sale/boosters.asset":
				base.gameObject.AddComponent<OldRandomSaleUI>();
				break;
			case "remote/sale/rockets.asset":
				base.gameObject.AddComponent<OldBoosterSaleUI>();
				break;
			}
			string arg = string.Empty;
			switch (saleConfig.Type)
			{
			case "SaleCoins2":
				arg = "OnlineSale_Coins";
				break;
			case "SaleWild":
				arg = "OnlineSale_Wild";
				break;
			case "SaleRocket":
				arg = "OnlineSale_Rocket";
				break;
			case "SaleMegaPackage":
				arg = "OnlineSale_Season";
				break;
			case "SaleAssignPackage":
				arg = "OnlineSale_Crab";
				break;
			case "HightScoreSale":
				arg = "OnlineSale_Highscore";
				break;
			case "StoreSaleWild":
				arg = "LocalSale_Wild";
				break;
			case "StoreSaleRocket":
				arg = "LocalSale_Rocket";
				break;
			case "StoreSaleCoins":
				arg = "LocalSale_Coins";
				break;
			}
			int num = SaleConfig.GetNormalSale().FindIndex(saleConfig);
			if (num < 0)
			{
				num = SaleConfig.GetSpecialSale().FindIndex(saleConfig);
			}
			for (int i = 0; i < saleItemUIs.Length; i++)
			{
				if (i < saleConfig.purchasingPackages.Count)
				{
					saleItemUIs[i].SetPurchasingPackage(saleConfig.purchasingPackages[i], $"{arg}_{num + 1}_{i + 1}");
				}
			}
			SingletonBehaviour<UnityPurchasingHelper>.Get().Append(PurchasingCompleted);
		}

		public void SetClosedUnityAction(UnityAction unityAction)
		{
			OnClosedAction = unityAction;
		}
	}
}
