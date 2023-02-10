using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PurchasSuccessPopup : SoundScene
	{
		public Button OKButton;

		public RectTransform ParentTransform;

		public void OnStart(PurchasingCommodity[] commoditys, UnityAction unityAction = null)
		{
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PurchasSuccessPopup).Name, "UI/PurchasSuccessItem");
			List<PurchasingCommodity> arrays = (from e in commoditys
				group e by e.boosterType into e
				select new PurchasingCommodity
				{
					boosterType = e.Key,
					count = e.Sum((PurchasingCommodity x) => x.count)
				}).ToList();
			arrays.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.World);
			arrays.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.Poker);
			foreach (PurchasingCommodity item in arrays)
			{
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.SetParent(ParentTransform, worldPositionStays: false);
				gameObject.GetComponent<PurchasSuccessItem>().SetNumber(item.boosterType, item.count);
			}
			DelayDo(delegate
			{
				Vector2 sizeDelta = ParentTransform.sizeDelta;
				float a = sizeDelta.y + 300f;
				a = Mathf.Max(a, 600f);
				(ParentTransform.parent as RectTransform).sizeDelta = new Vector2(900f, a);
			});
			OKButton.onClick.AddListener(delegate
			{
				foreach (PurchasingCommodity item2 in arrays)
				{
					SingletonBehaviour<EffectUtility>.Get().CreateBoosterType(item2.boosterType, OKButton.transform.position);
					if (item2.boosterType == BoosterType.UnlimitedDoubleStar)
					{
						MenuUITopLeft.UpdateDoubleStarRemianUI();
					}
					if (item2.boosterType == BoosterType.UnlimitedPlay)
					{
						MenuUITopLeft.UpdateUnlimitedPlayRemianUI();
					}
				}
				SingletonClass<MySceneManager>.Get().Close(new JoinEffect(), unityAction);
			});
		}

		public static void ShowPurchasSuccessPopup(PurchasingCommodity[] commoditys, UnityAction unityAction = null)
		{
			SingletonClass<MySceneManager>.Get().Popup<PurchasSuccessPopup>("Scenes/Pops/PurchasSuccessPopup").OnStart(commoditys, unityAction);
		}
	}
}
