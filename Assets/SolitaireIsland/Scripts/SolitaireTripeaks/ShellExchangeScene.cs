using Nightingale.Extensions;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class ShellExchangeScene : SoundScene
	{
		public Transform Content;

		public Button BuyShellsButton;

		private void Awake()
		{
			base.IsStay = true;
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/ShellExchangeUI");
			foreach (BoosterType booster in AppearNodeConfig.Get().GetBoosters())
			{
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.SetParent(Content, worldPositionStays: false);
				gameObject.GetComponent<ShellExchangeUI>().OnStart(booster);
			}
			BuyShellsButton.onClick.AddListener(delegate
			{
				base.IsStay = false;
				StoreScene.ShowStore();
				Object.FindObjectOfType<StoreScene>().HasVaule(delegate(StoreScene storeScene)
				{
					storeScene.transform.FindType("Canvas/Parent/Store", delegate(TabGroup group)
					{
						group.SetTabIndex(1);
					});
					storeScene.AddClosedListener(delegate
					{
						base.IsStay = true;
					});
				});
			});
		}
	}
}
