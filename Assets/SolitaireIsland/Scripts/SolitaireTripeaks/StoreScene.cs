using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class StoreScene : SoundScene
	{
		private UnityAction unityAction;

		public Button CloseButton;

		private void Start()
		{
			base.IsStay = true;
			SaleData.Get().storeOpenCount++;
			CloseButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				if (unityAction != null)
				{
					unityAction();
				}
			});
			base.transform.Find("Canvas/Parent/Store/Viewport/CoinContent/Coins/moreOffers").HasVaule(delegate(Transform tr)
			{
				StoreScene storeScene = this;
				tr.GetComponent<Button>().onClick.AddListener(delegate
				{
					tr.gameObject.SetActive(value: false);
					int num = 0;
					Transform transform = storeScene.transform.Find("Canvas/Parent/Store/Viewport/CoinContent/Packages");
					for (int i = 0; i < transform.childCount; i++)
					{
						Transform child = transform.GetChild(i);
						child.gameObject.SetActive(value: true);
						child.localScale = Vector3.zero;
						child.DOScale(1f, 0.2f).SetDelay((float)num++ * 0.1f);
					}
					Transform transform2 = storeScene.transform.Find("Canvas/Parent/Store/Viewport/CoinContent/Coins");
					for (int j = 0; j < transform2.childCount - 1; j++)
					{
						Transform child2 = transform2.GetChild(j);
						child2.gameObject.SetActive(value: true);
						child2.localScale = Vector3.zero;
						child2.DOScale(1f, 0.2f).SetDelay((float)num++ * 0.1f);
					}
				});
			});
		}

		public void SetUnityClose(UnityAction unityAction)
		{
			this.unityAction = unityAction;
		}

		public static StoreScene ShowStore()
		{
			if (SaleData.Get().HasSale("SaleSpecialStore"))
			{
				return SingletonClass<MySceneManager>.Get().Popup<StoreScene>("Scenes/StoreSaleScene", new JoinEffect());
			}
			return SingletonClass<MySceneManager>.Get().Popup<StoreScene>("Scenes/StoreScene", new JoinEffect());
		}

		public static void ShowOutofCoins(UnityAction unityAction = null)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("title_out_of_coins"), localizationUtility.GetString("desc_out_of_coins"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				ShowStore().SetUnityClose(unityAction);
			});
		}

		public static void ShowOutofCoinsInLevelScene(UnityAction unityAction = null)
		{
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
			SingletonClass<MySceneManager>.Get().Popup<TipPopupNoIconScene>("Scenes/Pops/TipPopupNoIcon").OnStart(localizationUtility.GetString("title_out_of_coins"), localizationUtility.GetString("desc_out_of_coins"), localizationUtility.GetString("btn_ok"), delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
				if (!OnceBuyCoinsScene.TryShow())
				{
					ShowStore().SetUnityClose(unityAction);
				}
			});
		}
	}
}
