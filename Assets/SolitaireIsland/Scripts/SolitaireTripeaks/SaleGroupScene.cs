using Nightingale.Inputs;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SaleGroupScene : SoundScene
	{
		public Button _NextButton;

		public Button _PreButton;

		private List<BaseScene> Pages = new List<BaseScene>();

		private int pageIndex;

		private bool isAnitiom;

		private string sortingLayerName;

		private int sceneLayerIndex;

		public static void ShowSale()
		{
			List<SaleInfo> saleInfos = SaleData.Get().GetSaleInfos();
			if (saleInfos.Count == 0)
			{
				return;
			}
			if (saleInfos.Count == 1)
			{
				SaleButtonUI saleButtonUI = Object.FindObjectOfType<SaleButtonUI>();
				if (saleButtonUI != null)
				{
					saleButtonUI.SaleButton.interactable = false;
				}
				saleInfos[0].Show(delegate
				{
					if (saleButtonUI != null)
					{
						saleButtonUI.SaleButton.interactable = true;
					}
				});
			}
			else
			{
				SingletonClass<MySceneManager>.Get().Popup<SaleGroupScene>("Scenes/SaleGroupScene").OnStart(saleInfos);
			}
		}

		public void OnStart(List<SaleInfo> sales)
		{
			base.IsStay = true;
			sales.ForEach(delegate(SaleInfo sale)
			{
				sale.CreateSale(delegate(BaseScene scene)
				{
					scene.transform.SetParent(base.transform, worldPositionStays: false);
					Pages.Add(scene);
					for (int i = 0; i < Pages.Count; i++)
					{
						Pages[i].SetLayer(sortingLayerName, sceneLayerIndex);
						Pages[i].gameObject.SetActive(pageIndex == i);
						if (pageIndex == i && Pages[i] == scene)
						{
							new JoinEffect().Open(Pages[i]);
						}
					}
				});
			});
			_NextButton.onClick.AddListener(delegate
			{
				UpdatePageIndex(pageIndex + 1);
			});
			_PreButton.onClick.AddListener(delegate
			{
				UpdatePageIndex(pageIndex - 1);
			});
			SingletonBehaviour<EscapeInputManager>.Get().Append(OnBackKeyDown);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<EscapeInputManager>.Get().Remove(OnBackKeyDown);
		}

		public override void OnSceneStateChanged(SceneState state)
		{
			base.OnSceneStateChanged(state);
			if (state == SceneState.Closing)
			{
				foreach (BaseScene page in Pages)
				{
					new JoinEffect().Closed(page);
				}
			}
		}

		private bool OnBackKeyDown()
		{
			if (base.gameObject.activeInHierarchy)
			{
				GraphicRaycaster componentInChildren = base.gameObject.GetComponentInChildren<GraphicRaycaster>();
				if (componentInChildren == null)
				{
					return true;
				}
				if (!componentInChildren.enabled)
				{
					return true;
				}
				if (SingletonClass<MySceneManager>.Get().GetTopScene() == this)
				{
					SingletonClass<MySceneManager>.Get().Close(new JoinEffect());
					return true;
				}
			}
			return false;
		}

		public override void SetLayer(string sortingLayerName, int index)
		{
			this.sortingLayerName = sortingLayerName;
			sceneLayerIndex = index;
			base.SetLayer(sortingLayerName, index);
			for (int i = 0; i < Pages.Count; i++)
			{
				Pages[i].SetLayer(sortingLayerName, index);
			}
		}

		private void UpdatePagePosition()
		{
			for (int i = 0; i < Pages.Count; i++)
			{
				Pages[i].SetLayer(sortingLayerName, sceneLayerIndex);
				Pages[i].gameObject.SetActive(pageIndex == i);
				if (pageIndex == i)
				{
					new JoinEffect().Open(Pages[i]);
				}
			}
		}

		private void UpdatePageIndex(int index)
		{
			if (isAnitiom)
			{
				return;
			}
			if (index == pageIndex)
			{
				UpdatePagePosition();
				return;
			}
			float num = 1920f;
			JoinEffect joinEffect;
			JoinEffect joinEffect2;
			if (index > pageIndex)
			{
				num = 0f - num;
				joinEffect = new JoinEffect(JoinEffectDir.Left);
				joinEffect2 = new JoinEffect(JoinEffectDir.Right);
			}
			else
			{
				joinEffect = new JoinEffect(JoinEffectDir.Right);
				joinEffect2 = new JoinEffect(JoinEffectDir.Left);
			}
			index += Pages.Count;
			index %= Pages.Count;
			Pages[index].gameObject.SetActive(value: true);
			joinEffect.Show(Pages[index]);
			isAnitiom = true;
			BaseScene current = Pages[pageIndex];
			joinEffect2.Hide(current, delegate
			{
				isAnitiom = false;
				current.gameObject.SetActive(value: false);
			});
			pageIndex = index;
		}
	}
}
