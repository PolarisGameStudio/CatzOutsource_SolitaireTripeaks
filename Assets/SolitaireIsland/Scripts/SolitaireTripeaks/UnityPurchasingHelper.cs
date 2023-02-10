using Nightingale.Toasts;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

namespace SolitaireTripeaks
{
	public class UnityPurchasingHelper : SingletonBehaviour<UnityPurchasingHelper>, IStoreListener
	{
		private IStoreController controller;

		private IExtensionProvider extensions;

		private List<PurchasingEevet> delegateList = new List<PurchasingEevet>();

		public bool IsInited => IsInitialized();

		public void Append(PurchasingEevet handler)
		{
			if (!delegateList.Contains(handler))
			{
				delegateList.Add(handler);
			}
		}

		public void Insert(PurchasingEevet handler)
		{
			if (!delegateList.Contains(handler))
			{
				delegateList.Insert(0, handler);
			}
		}

		public void Remove(PurchasingEevet handler)
		{
			if (delegateList.Contains(handler))
			{
				delegateList.Remove(handler);
			}
		}

		public void Clear()
		{
			delegateList.Clear();
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			this.controller = controller;
			this.extensions = extensions;
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			LoadingHelper.Get("UnityPurchasing").StopLoading();
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			LoadingHelper.Get("UnityPurchasing").StopLoading();
			if (true)
			{
				PurchasingPackage purchasingPackage = ValidatorPayload(e.purchasedProduct.receipt);
				if (purchasingPackage != null)
				{
					PurchasingEevet[] array = delegateList.ToArray();
					PurchasingEevet[] array2 = array;
					foreach (PurchasingEevet purchasingEevet in array2)
					{
						purchasingEevet(e.purchasedProduct.transactionID, purchasingPackage);
					}
				}
			}
			return PurchaseProcessingResult.Complete;
		}

		public ProductMetadata GetProductMetadata(string id)
		{
			try
			{
				return controller.products.all.ToList().Find((Product e) => e.definition.id.Equals(id)).metadata;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			return null;
		}

		public void OnPurchaseClicked(PurchasingPackage package)
		{
            if (controller != null && controller.products != null && controller.products.all != null)
			{
				Product product = controller.products.all.ToList().Find((Product e) => package.id.Equals(e.definition.id));
				if (product != null)
				{
					LoadingHelper.Get("UnityPurchasing").ShowLoading(null, null, string.Empty);
					controller.InitiatePurchase(product, JsonUtility.ToJson(package));
				}
			}
		}

		public void StartInitialize()
		{
			if (!IsInitialized())
			{
				ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
				PurchasingInfo[] infos = UnityPurchasingConfig.Get().infos;
				IEnumerable<string> enumerable = from e in UnityPurchasingConfig.Get().Stores
					select e.ToString();
				PurchasingInfo[] array = infos;
				foreach (PurchasingInfo purchasingInfo in array)
				{
					IDs ds = new IDs();
					foreach (string item in enumerable)
					{
						ds.Add(purchasingInfo.inStore, item);
					}
					configurationBuilder.AddProduct(purchasingInfo.id, ProductType.Consumable, ds);
				}
				UnityPurchasing.Initialize(this, configurationBuilder);
			}
		}

		private bool IsInitialized()
		{
			return controller != null && extensions != null;
		}

		private PurchasingPackage ValidatorPayload(string content)
		{
			Dictionary<string, object> dictionary = MiniJson.JsonDecode(content) as Dictionary<string, object>;
			string text = (string)dictionary["Store"];
			try
			{
				if (text != null && text == "GooglePlay")
				{
					Dictionary<string, object> dictionary2 = MiniJson.JsonDecode(dictionary["Payload"].ToString()) as Dictionary<string, object>;
					Dictionary<string, object> dictionary3 = MiniJson.JsonDecode(dictionary2["json"].ToString()) as Dictionary<string, object>;
					dictionary3 = (MiniJson.JsonDecode(dictionary3["developerPayload"].ToString()) as Dictionary<string, object>);
					string @string = Encoding.Default.GetString(Convert.FromBase64String(dictionary3["developerPayload"].ToString()));
					return JsonUtility.FromJson<PurchasingPackage>(@string);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
			return null;
		}
	}
}
