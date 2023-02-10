using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SaleInfo
	{
		public SaleItemConfig SaleConfig;

		public bool IsStart;

		public long saleTimeLeft;

		public int ShowNumber;

		public List<string> purchasings;

		public TimeSpan GetTimeSpan()
		{
			return new DateTime(saleTimeLeft).Subtract(DateTime.Now);
		}

		public bool IsRunning()
		{
			return IsStart && !IsInvalid();
		}

		public bool IsInvalid()
		{
			if (!IsStart)
			{
				return false;
			}
			return GetTimeSpan().TotalSeconds <= 0.0;
		}

		public bool DOStart()
		{
			if (IsStart)
			{
				return false;
			}
			saleTimeLeft = DateTime.Now.AddMinutes(SaleConfig.minutes).Ticks;
			IsStart = true;
			return true;
		}

		public void Show(UnityAction unityAction = null)
		{
			if (SaleConfig != null && SaleConfig.IsReady())
			{
				SaleConfig.GetObject(delegate(GameObject sceneObject)
				{
					ShowNumber++;
					GameObject gameObject = UnityEngine.Object.Instantiate(sceneObject);
					if (gameObject.GetComponent<SaleScene>() != null)
					{
						SaleScene saleScene = SingletonClass<MySceneManager>.Get().Popup<SaleScene>(gameObject);
						saleScene.OnStart(this, new DateTime(saleTimeLeft));
						saleScene.AddClosedListener(unityAction);
					}
					else
					{
						StoreOffSaleScene storeOffSaleScene = SingletonClass<MySceneManager>.Get().Popup<StoreOffSaleScene>(gameObject);
						storeOffSaleScene.OnStart(DateTime.Now.Add(GetTimeSpan()));
						storeOffSaleScene.AddClosedListener(unityAction);
					}
				});
			}
			else if (unityAction != null)
			{
				unityAction();
			}
		}

		public void CreateSale(UnityAction<BaseScene> unityAction)
		{
			if (SaleConfig != null && SaleConfig.IsReady())
			{
				SaleConfig.GetObject(delegate(GameObject sceneObject)
				{
					ShowNumber++;
					GameObject gameObject = UnityEngine.Object.Instantiate(sceneObject);
					if (gameObject.GetComponent<SaleScene>() != null)
					{
						SaleScene component = gameObject.GetComponent<SaleScene>();
						component.OnStart(this, new DateTime(saleTimeLeft));
						if (component != null && unityAction != null)
						{
							unityAction(component);
						}
					}
					else
					{
						StoreOffSaleScene component2 = gameObject.GetComponent<StoreOffSaleScene>();
						component2.OnStart(DateTime.Now.Add(GetTimeSpan()));
						if (component2 != null && unityAction != null)
						{
							unityAction(component2);
						}
					}
				});
			}
		}

		public void PutBuy(string id)
		{
			if (purchasings == null)
			{
				purchasings = new List<string>();
			}
			purchasings.Add(id);
		}

		public string GetBuyInfo()
		{
			if (purchasings == null)
			{
				purchasings = new List<string>();
			}
			string text = string.Empty;
			foreach (IGrouping<string, string> purchasing in from e in purchasings
				group e by e)
			{
				SalePackage salePackage = SaleConfig.purchasingPackages.Find((SalePackage e) => e.id == purchasing.Key);
				if (salePackage != null)
				{
					text += $"{SaleConfig.purchasingPackages.IndexOf(salePackage)}_{purchasing.Count()}_{salePackage.id}|{salePackage.GetString()};";
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public string GetId()
		{
			return $"{SaleConfig.Type}_{SaleConfig.level}";
		}
	}
}
