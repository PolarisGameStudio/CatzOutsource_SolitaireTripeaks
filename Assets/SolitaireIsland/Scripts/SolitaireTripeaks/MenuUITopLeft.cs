using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Inputs;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class MenuUITopLeft : DelayBehaviour
	{
		public Button SettingButton;

		public Button RandomBoosterButton;

		public RemainLabel UnlimitedPlayLabel;

		public RemainLabel DoubleStarLabel;

		private bool isSale;

		public Transform CoinTransform;

		public static void UpdateDoubleStarRemianUI()
		{
			MenuUITopLeft menu = GetMenu();
			if (!(menu == null))
			{
				menu.UpdateDoubleStar(animation: true);
			}
		}

		public static void UpdateUnlimitedPlayRemianUI()
		{
			MenuUITopLeft menu = GetMenu();
			if (!(menu == null))
			{
				menu.UpdateUnlimitedPlay(animation: true);
			}
		}

		public static void UpdateStoreUIRemianUI()
		{
			MenuUITopLeft menu = GetMenu();
			if (!(menu == null))
			{
				menu.UpdateStoreUI();
			}
		}

		public static MenuUITopLeft GetMenu()
		{
			return UnityEngine.Object.FindObjectOfType<MenuUITopLeft>();
		}

		public static MenuUITopLeft CreateMenuUITopLeft(Transform transform, bool hasEsc = true)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/MenuUI_TopLeft"));
			gameObject.transform.SetParent(transform.Find("Canvas"), worldPositionStays: false);
			MenuUITopLeft component = gameObject.GetComponent<MenuUITopLeft>();
			if (hasEsc)
			{
				component.SettingButton.gameObject.AddComponent<EscapeButtonControler>();
			}
			return component;
		}

		private void Awake()
		{
			RandomBoosterButton.onClick.AddListener(delegate
			{
				SingletonClass<MySceneManager>.Get().Popup<ShellExchangeScene>("Scenes/ShellExchangeScene");
			});
			UpdateStoreUI();
			UpdateUnlimitedPlay();
			UpdateDoubleStar();
		}

		private void UpdateDoubleStar(bool animation = false)
		{
			DoubleStarLabel.gameObject.SetActive(RankCoinData.Get().IsDouble());
			DoubleStarLabel.OnCompleted.RemoveAllListeners();
			if (DoubleStarLabel.gameObject.activeSelf)
			{
				DelayDo(delegate
				{
					DoubleStarLabel.SetRemainTime(RankCoinData.Get().GetRemainTime());
					DoubleStarLabel.OnCompleted.AddListener(delegate
					{
						UpdateDoubleStar();
					});
					if (animation)
					{
						SingletonBehaviour<TopCanvasHelper>.Get().CreateBooster(BoosterType.DoubleStar, Vector3.zero, DoubleStarLabel.transform.GetChild(0).position, 0f, delegate
						{
							DoubleStarLabel.transform.DOScale(1.1f, 0.3f);
							DoubleStarLabel.transform.DOScale(1f, 0.1f);
						});
					}
				});
			}
		}

		private void UpdateUnlimitedPlay(bool animation = false)
		{
			UnlimitedPlayLabel.gameObject.SetActive(AuxiliaryData.Get().IsUnlimitedPlay());
			UnlimitedPlayLabel.OnCompleted.RemoveAllListeners();
			if (UnlimitedPlayLabel.gameObject.activeSelf)
			{
				DelayDo(delegate
				{
					UnlimitedPlayLabel.SetRemainTime(new DateTime(AuxiliaryData.Get().UnlimitedPlayTicks));
					UnlimitedPlayLabel.OnCompleted.AddListener(delegate
					{
						UpdateUnlimitedPlay();
					});
					if (animation)
					{
						SingletonBehaviour<TopCanvasHelper>.Get().CreateBooster(BoosterType.UnlimitedPlay, Vector3.zero, UnlimitedPlayLabel.transform.GetChild(0).position, 0.1f, delegate
						{
							UnlimitedPlayLabel.transform.DOScale(1.1f, 0.3f);
							UnlimitedPlayLabel.transform.DOScale(1f, 0.1f);
						});
					}
				});
			}
		}

		private void UpdateStoreUI()
		{
			isSale = SaleData.Get().HasSale("SaleSpecialStore");
			base.transform.Find("Coin_Frame/Sale").gameObject.SetActive(isSale);
		}

		public void MenuClick()
		{
			SingletonClass<MySceneManager>.Get().Popup<SettingsScene>("Scenes/SettingScene", new JoinEffect());
		}

		public void StoreClick()
		{
			StoreScene.ShowStore();
		}
	}
}
