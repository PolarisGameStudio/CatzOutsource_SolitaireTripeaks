using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SelectionIslandScene : BaseScene
	{
		public List<IsLandThumbnailController> Points;

		public RectTransform contentTransform;

		private List<IsLandThumUI> IsLandThumUIs = new List<IsLandThumUI>();

		private GameObject selfUIGameObject;

		public int World
		{
			get;
			private set;
		}

		public int PlayerChapterIndex
		{
			get;
			private set;
		}

		public static SelectionIslandScene Create(int world)
		{
			string empty = string.Empty;
			switch (world)
			{
			default:
				empty = "Scenes/WorldBellaScene";
				break;
			case 1:
				empty = "Scenes/WorldParadiseScene";
				break;
			case 2:
				empty = "Scenes/WorldFantasticScene";
				break;
			}
			return SingletonClass<MySceneManager>.Get().Navigation<SelectionIslandScene>(empty);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<TripeaksPlayerHelper>.Get().RemoveListener(UpdateFriendsSchedule);
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(SelectionIslandScene).Name);
		}

		public void OnStart(int worldIndex)
		{
			SingletonBehaviour<SpecialActivityUtility>.Get().CheckSpecialActivity();
			UniverseConfig.Get().DestoryDetails();
			SingletonBehaviour<GlobalConfig>.Get().PlayBackground();
			MenuUITopLeft.CreateMenuUITopLeft(base.transform);
			MenuUIRight.CreateMenuUIRight(base.transform).CreateInChapterSelect(worldIndex);
			World = worldIndex;
			WorldConfig worldConfig = UniverseConfig.Get().GetWorldConfig(worldIndex);
			WorldData worldData = PlayData.Get().GetWorldData(World);
			if (worldData != null)
			{
				PlayerChapterIndex = worldData.playChapter;
			}
			for (int i = 0; i < worldConfig.chapters.Count && i <= Points.Count - 1; i++)
			{
				ChapterConfig chapterConfig = worldConfig.chapters[i];
				if (!chapterConfig.IsThumbnailAtLocalPath())
				{
					break;
				}
				AssetBundle thumbnailAssetBundle = chapterConfig.GetThumbnailAssetBundle();
				if (thumbnailAssetBundle == null)
				{
					break;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(thumbnailAssetBundle.LoadAsset<GameObject>("IsLandThumbnail"));
				gameObject.transform.SetParent(Points[i].transform, worldPositionStays: false);
				gameObject.transform.localPosition = Vector3.zero;
				Points[i].SetLock(PlayData.Get().IsLock(worldIndex, i));
				IsLandThumUI component = gameObject.GetComponent<IsLandThumUI>();
				component.SetInfo(worldIndex, worldConfig.chapters.IndexOf(chapterConfig), delegate(int cha)
				{
					JumpTo(cha, delegate
					{
						JoinPlayHelper.JoinToIslandDetailScene();
					}, self: true);
					AudioUtility.GetSound().Play("Audios/selectIsland.mp3");
				});
				IsLandThumUIs.Add(component);
			}
			IsLandThumUIs = (from e in IsLandThumUIs
				orderby e.Chapter
				select e).ToList();
			if (PlayerChapterIndex > IsLandThumUIs.Count - 1)
			{
				PlayerChapterIndex = IsLandThumUIs.Count - 1;
			}
			CheckIslandLock();
			OnLoadCompeted();
			IEnumerable<IsLandThumbnailController> source = from e in Points
				where e.IsVisable
				select e;
			contentTransform.sizeDelta = new Vector2(Mathf.Max(Mathf.Abs(source.Min(delegate(IsLandThumbnailController p)
			{
				Vector3 localPosition4 = p.transform.localPosition;
				return localPosition4.x;
			})), Mathf.Abs(source.Max(delegate(IsLandThumbnailController p)
			{
				Vector3 localPosition3 = p.transform.localPosition;
				return localPosition3.x;
			}))) * 2f + 4140f, Mathf.Max(Mathf.Abs(source.Min(delegate(IsLandThumbnailController p)
			{
				Vector3 localPosition2 = p.transform.localPosition;
				return localPosition2.y;
			})), Mathf.Abs(source.Max(delegate(IsLandThumbnailController p)
			{
				Vector3 localPosition = p.transform.localPosition;
				return localPosition.y;
			}))) * 2f + 3240f);
			selfUIGameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SelectionIslandScene).Name, "UI/SelfAvaterUI"));
			selfUIGameObject.AddComponent<LayoutElement>().ignoreLayout = true;
			selfUIGameObject.transform.SetParent(contentTransform, worldPositionStays: false);
			selfUIGameObject.transform.localPosition = selfUIGameObject.transform.parent.InverseTransformPoint(IsLandThumUIs[PlayerChapterIndex]._JumpTransform.position);
			selfUIGameObject.transform.SetAsLastSibling();
			SingletonBehaviour<TripeaksPlayerHelper>.Get().AddListener(UpdateFriendsSchedule);
		}

		public void CheckIslandLock()
		{
			for (int i = 0; i < IsLandThumUIs.Count; i++)
			{
				int chapter = IsLandThumUIs[i].Chapter;
				if (!PlayData.Get().IsLock(World, chapter) && !AuxiliaryData.Get().UnLocks.Contains(new ChapterScheduleData(World, chapter)))
				{
					SetCanvasGraphicRaycaster(enabled: false);
					RectTransformHelper.Center(Points[i].transform as RectTransform);
					IsLandThumUIs[i].ShowUnlockAnmtion(delegate
					{
						OnLoadCompeted(completed: true, delegate
						{
							SetCanvasGraphicRaycaster(enabled: true);
						});
						AuxiliaryData.Get().UnLocks.Add(new ChapterScheduleData(World, chapter));
					});
					return;
				}
			}
			RectTransformHelper.Center(Points[PlayerChapterIndex].transform as RectTransform);
			OnLoadCompeted(completed: true);
		}

		public void JumpTo(int chapter, UnityAction<IslandScene> unityAction = null, bool self = false)
		{
			try
			{
				if (chapter > IsLandThumUIs.Count - 1)
				{
					TipPopupNoIconScene.ShowDataDownloading("JumpTo");
				}
				else
				{
					ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(World, chapter);
					LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
#if ENABLE_UPDATE_VERSION
                    if (!chapterConfig.IsSupportVersion())
					{
						TipPopupNoIconScene.ShowNeedUpdateVersion();
					}
					else if (!chapterConfig.IsDetailsAtLocalPath())
					{
						TipPopupNoIconScene.ShowDownloading();
					}
					else 
#endif
                    if (PlayData.Get().IsLock(World, chapter))
					{
						ScheduleData preScheduleData = UniverseConfig.Get().GetPreScheduleData(new ScheduleData(World, chapter, 0));
						ChapterConfig chapterConfig2 = UniverseConfig.Get().GetChapterConfig(preScheduleData.world, preScheduleData.chapter);
						if (chapterConfig2 != null)
						{
							TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("UnLock Chapter Tips Title"), string.Format(localizationUtility.GetString("UnLock Chapter Tips Description"), chapterConfig2.name, chapterConfig.name));
						}
					}
					else
					{
						SetCanvasGraphicRaycaster(enabled: false);
						TweenCallback tweenCallback = delegate
						{
							GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
							{
								UniverseConfig.Get().DestoryThumbnails();
								IslandScene scene = IslandScene.Create(World);
								scene.OnStart(World, chapter);
								scene.AddLoadListener(delegate(bool success)
								{
									if (!success && unityAction != null)
									{
										unityAction(scene);
									}
								});
								return scene;
							});
						};
						if (PlayerChapterIndex == chapter && self)
						{
							tweenCallback();
						}
						else if (!(selfUIGameObject == null))
						{
							WorldData worldData = PlayData.Get().GetWorldData(World);
							if (worldData != null)
							{
								worldData.playChapter = chapter;
							}
							PlayerChapterIndex = chapter;
							Vector3 endValue = selfUIGameObject.transform.parent.InverseTransformPoint(IsLandThumUIs[chapter]._JumpTransform.position);
							selfUIGameObject.transform.DOLocalJump(endValue, 500f, 1, 1f).OnComplete(tweenCallback);
						}
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void UpdateFriendsSchedule(List<TripeaksPlayer> players)
		{
			try
			{
				if (players != null)
				{
					foreach (IsLandThumUI isLandThumUI in IsLandThumUIs)
					{
						isLandThumUI.AddFriendSchedule(players);
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}
	}
}
