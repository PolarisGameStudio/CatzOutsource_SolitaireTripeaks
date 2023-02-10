using DG.Tweening;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class WorldScene : BaseScene
	{
		public Button _ExitButton;

		public Transform contentTransform;

		public WorldThumbnailUI[] WorldThumbnailUIs;

		private int playWorld;

		private GameObject selfUIGameObject;

		public void OnStart(int world)
		{
			SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig();
			UniverseConfig.Get().DestoryDetails();
			UniverseConfig.Get().DestoryThumbnails();
			playWorld = world;
			if (playWorld > WorldThumbnailUIs.Length)
			{
				playWorld = WorldThumbnailUIs.Length - 1;
			}
			MenuUITopLeft.CreateMenuUITopLeft(base.transform, hasEsc: false);
			_ExitButton.onClick.AddListener(delegate
			{
				JoinPlayHelper.TryDownload(force: true);
				JoinPlayHelper.JoinSelectionIslandScene(world);
			});
			DelayDo(delegate
			{
				CenterToSelected(WorldThumbnailUIs[playWorld].gameObject);
				selfUIGameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(WorldScene).Name, "UI/SelfAvaterUI"));
				selfUIGameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				selfUIGameObject.transform.SetParent(contentTransform, worldPositionStays: false);
				selfUIGameObject.transform.localPosition = WorldThumbnailUIs[playWorld].transform.localPosition;
				OnLoadCompeted();
			});
		}

		public static void PurchasingSuccess(PurchasingPackage package)
		{
			WorldScene worldScene = Object.FindObjectOfType<WorldScene>();
			PurchasingCommodity[] commoditys = package.commoditys;
			int num = 0;
			PurchasingCommodity purchasingCommodity;
			while (true)
			{
				if (num >= commoditys.Length)
				{
					return;
				}
				purchasingCommodity = commoditys[num];
				if (purchasingCommodity.boosterType == BoosterType.World)
				{
					WorldThumbnailUI[] worldThumbnailUIs = worldScene.WorldThumbnailUIs;
					foreach (WorldThumbnailUI worldThumbnailUI in worldThumbnailUIs)
					{
						worldThumbnailUI.UpdateUI();
					}
					if (purchasingCommodity.count < worldScene.WorldThumbnailUIs.Length)
					{
						break;
					}
				}
				num++;
			}
			worldScene.JumpTo(purchasingCommodity.count);
		}

		private void CenterToSelected(GameObject selected)
		{
			RectTransform component = selected.GetComponent<RectTransform>();
			RectTransform rectTransform = contentTransform.parent.parent.transform as RectTransform;
			Vector3 a = rectTransform.position + (Vector3)rectTransform.rect.center;
			Vector3 position = component.position;
			Vector3 b = a - position;
			b.z = 0f;
			Vector3 position2 = contentTransform.position + b;
			contentTransform.position = position2;
		}

		public void CenterToWorld(int index)
		{
			playWorld = index;
			if (playWorld > WorldThumbnailUIs.Length)
			{
				playWorld = WorldThumbnailUIs.Length - 1;
			}
			CenterToSelected(WorldThumbnailUIs[playWorld].gameObject);
			selfUIGameObject.transform.localPosition = WorldThumbnailUIs[playWorld].transform.localPosition;
		}

		public void JumpTo(int world, UnityAction<SelectionIslandScene> unityAction = null)
		{
			if (world > WorldThumbnailUIs.Length || !UniverseConfig.Get().HasIslandConfigInWorld(world))
			{
				TipPopupNoIconScene.ShowDownloading();
				return;
			}
			if (PlayData.Get().IsLock(world))
			{
				TipPopupNoIconScene.ShowUnlockWorldScene(world);
				return;
			}
			SetCanvasGraphicRaycaster(enabled: false);
			TweenCallback tweenCallback = delegate
			{
				SetCanvasGraphicRaycaster(enabled: true);
				JoinPlayHelper.JoinSelectionIslandScene(world, delegate(bool success)
				{
					if (success)
					{
						AudioUtility.GetSound().Play("Audios/selectIsland.mp3");
						if (unityAction != null)
						{
							unityAction(Object.FindObjectOfType<SelectionIslandScene>());
						}
					}
				});
			};
			if (playWorld == world)
			{
				tweenCallback();
				return;
			}
			playWorld = world;
			selfUIGameObject.transform.DOLocalJump(WorldThumbnailUIs[playWorld].transform.localPosition, 500f, 1, 1f).OnComplete(tweenCallback);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(WorldScene).Name);
		}
	}
}
