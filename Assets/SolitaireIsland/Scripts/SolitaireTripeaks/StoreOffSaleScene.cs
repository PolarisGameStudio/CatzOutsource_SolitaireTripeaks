using Nightingale.Inputs;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class StoreOffSaleScene : BaseScene
	{
		public RemainLabel RemainLabel;

		public Button Btn_SaveNow;

		public void OnStart(DateTime dateTime)
		{
			RemainLabel.SetRemainTime(dateTime);
			Btn_SaveNow.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
				StoreScene.ShowStore();
			});
			RectTransform rectTransform = base.transform.Find("Canvas").Find("Parent") as RectTransform;
			Button component = rectTransform.Find("CloseButton").GetComponent<Button>();
			if (SingletonClass<MySceneManager>.Get().GetTopScene() == this)
			{
				component.gameObject.AddComponent<EscapeButtonControler>();
				JoinEffectCloseButton component2 = component.gameObject.GetComponent<JoinEffectCloseButton>();
				if (component2 != null)
				{
					UnityEngine.Object.Destroy(component2);
				}
				component.onClick.AddListener(delegate
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
		}
	}
}
