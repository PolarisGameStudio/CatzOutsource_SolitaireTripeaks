using Nightingale.Extensions;
using Nightingale.Inputs;
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
	public class JoinPlayHelper
	{
		public static void JoinPlay()
		{
			SingletonBehaviour<SynchronizeUtility>.Get().Init();
			SolitaireTripeaksData.Get().OnOpenApplication();
			SingletonBehaviour<TripeaksLogUtility>.Get().Init();
			ScheduleData schedule = PlayData.Get().GetPlayScheduleData();
			SingletonBehaviour<EscapeInputManager>.Get().ClearKey();
			if (!PlayData.Get().HasLevelData(0, 0, 0))
			{
				PurchaseListener();
				SingletonBehaviour<UnityPurchasingHelper>.Get().StartInitialize();
				schedule = default(ScheduleData);
				SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
				SingletonClass<MySceneManager>.Get().Navigation<PlayScene>("Scenes/PlayScene");
			}
			else
			{
				GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
				{
					UnityAction<bool> unityAction = delegate(bool success)
					{
						if (!success)
						{
							try
							{
								TryDownload();
								PurchaseListener();
								SingletonBehaviour<UnityPurchasingHelper>.Get().StartInitialize();
								LaunchToJoinIslandDetailScene();
							}
							catch (Exception ex)
							{
								UnityEngine.Debug.Log(ex.Message);
							}
						}
					};
					SingletonClass<ExpertLevelConfigGroup>.Get().ReadAssetBundle();
					ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(schedule.world, schedule.chapter);
					if (chapterConfig != null && chapterConfig.IsSupportVersion())
					{
						if (UniverseConfig.Get().HasThumbnailInWorld(schedule.world) && HasWorldTips())
						{
							return JoinSelectionIslandScene(0, unityAction);
						}
						if (chapterConfig.IsDetailsAtLocalPath())
						{
							IslandScene islandScene = IslandScene.Create(schedule.world);
							islandScene.AddLoadListener(unityAction);
							islandScene.OnStart(schedule.world, schedule.chapter);
							return islandScene;
						}
					}
					return JoinSelectionIslandScene(schedule.world, unityAction);
				});
			}
		}

		public static BaseScene JoinSelectionIslandScene(int world, UnityAction<bool> unityAction = null)
		{
			if ((UnityEngine.Object.FindObjectOfType<WorldScene>() != null 
				|| UnityEngine.Object.FindObjectOfType<IslandScene>() != null) 
				&& !UniverseConfig.Get().HasThumbnailInWorld(world))
			{
				TipPopupNoIconScene.ShowDataDownloading("JoinSelectionIslandScene not HasThumbnailInWorld");
				return null;
			}

			Func<BaseScene> CreatorSelectionIslandScene = delegate
			{
				if (UniverseConfig.Get().HasThumbnailInWorld(world))
				{
					SelectionIslandScene selectionIslandScene = SelectionIslandScene.Create(world);
					selectionIslandScene.AddLoadListener(unityAction);
					selectionIslandScene.OnStart(world);
					return selectionIslandScene;
				}
				WorldScene worldScene = SingletonClass<MySceneManager>.Get().Navigation<WorldScene>("Scenes/WorldScene");
				worldScene.AddLoadListener(delegate(bool success)
				{
					TipPopupNoIconScene.ShowDataDownloading("JoinSelectionIslandScene WorldScene");

					unityAction?.Invoke(success);
                });
				worldScene.OnStart(world);
				return worldScene;
			};
			if (UnityEngine.Object.FindObjectOfType<GlobalLoadingAnimation>() == null)
			{
				GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", () => CreatorSelectionIslandScene());
				return null;
			}
			return CreatorSelectionIslandScene();
		}

		public static void TryDownload(bool force = false)
		{
			if (force || SaleData.Get().IsOlderPlayer())
			{
				UniverseConfig.Get().Download();
				SaleConfig.GetNormalSale().Download();
			}
		}

		public static void JoinPlayByQuest(ScheduleData schedule)
		{
			ScheduleData playScheduleData = PlayData.Get().GetPlayScheduleData();
			IsLandDetails isLandDetails = UnityEngine.Object.FindObjectOfType<IsLandDetails>();
			if (isLandDetails != null && isLandDetails.World == schedule.world && isLandDetails.Chapter == schedule.chapter)
			{
				SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
				isLandDetails.JumpTo(schedule, 0.5f);
				return;
			}
			ChapterConfig chapterConfig = UniverseConfig.Get().GetChapterConfig(schedule.world, schedule.chapter);
			if (chapterConfig != null)
			{
				if (chapterConfig.IsSupportVersion() && chapterConfig.IsDetailsAtLocalPath())
				{
					SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
					GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
					{
						IslandScene islandScene = IslandScene.Create(schedule.world);
						islandScene.OnStart(schedule.world, schedule.chapter);
						return islandScene;
					});
				}
				else if (UniverseConfig.Get().HasThumbnailInWorld(schedule.world))
				{
					GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", () => JoinSelectionIslandScene(schedule.world, delegate(bool success)
					{
						if (success)
						{
							if (chapterConfig.IsSupportVersion())
							{
								TipPopupNoIconScene.ShowDataDownloading("JoinPlayByQuest HasThumbnailInWorld");
							}
#if ENABLE_UPDATE_VERSION
							else
							{
								TipPopupNoIconScene.ShowNeedUpdateVersion();
							}
#endif
						}
					}));
				}
				else
				{
					TipPopupNoIconScene.ShowDataDownloading("JoinPlayByQuest none");
				}
			}
		}

		public static void JoinPlayByGameEnd(ScheduleData schedule)
		{
			QuestsScene.TryShow(delegate
			{
				if (schedule.world == -1)
				{
					ScheduleData playScheduleData = PlayData.Get().GetPlayScheduleData();
					JoinSelectionIslandScene(playScheduleData.world, delegate(bool success)
					{
						if (success)
						{
							SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
							SingletonClass<MySceneManager>.Get().Popup<LevelScene>("Scenes/LevelScene", new NavigationEffect());
						}
					});
				}
				else
				{
					JoinPlayLevel(schedule, hasTips: true);
				}
			});
		}

		public static void JoinIslandByGameEnd(ScheduleData schedule)
		{
			QuestsScene.TryShow(delegate
			{
				if (schedule.world == -1)
				{
					ScheduleData playScheduleData = PlayData.Get().GetPlayScheduleData();
					JoinSelectionIslandScene(playScheduleData.world);
				}
				else
				{
					GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
					{
						if (HasWorldTips())
						{
							return JoinSelectionIslandScene(schedule.world, delegate(bool success)
							{
								if (success)
								{
									JoinToIslandDetailScene();
								}
							});
						}
						IslandScene islandScene = IslandScene.Create(schedule.world);
						islandScene.OnStart(schedule.world, schedule.chapter);
						islandScene.AddLoadListener(delegate(bool success)
						{
							if (!success)
							{
								JoinToIslandDetailScene();
								TryDownload();
							}
						});
						return islandScene;
					});
				}
			});
		}

		public static void JoinPlayLevel(ScheduleData schedule, bool hasTips = false)
		{
			ScheduleData playSchedule = PlayData.Get().GetPlayScheduleData();
			GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", delegate
			{
				UnityAction joinCompleted = delegate
				{
					TryDownload();
				};
				if (HasMapTips() && hasTips)
				{
					IslandScene islandScene2 = IslandScene.Create(playSchedule.world);
					islandScene2.OnStart(playSchedule.world, playSchedule.chapter);
					islandScene2.AddLoadListener(delegate(bool success)
					{
						if (!success)
						{
							JoinToIslandDetailScene();
						}
					});
					return islandScene2;
				}
				if (HasWorldTips() && hasTips)
				{
					return JoinSelectionIslandScene(playSchedule.world, delegate(bool success)
					{
						if (success)
						{
							JoinToIslandDetailScene();
						}
					});
				}
				if (schedule.world == playSchedule.world && schedule.chapter == playSchedule.chapter)
				{
					IslandScene scene = IslandScene.Create(schedule.world);
					scene.OnStart(schedule.world, schedule.chapter);
					scene.AddLoadListener(delegate(bool success)
					{
						if (!success)
						{
							JoinToIslandDetailScene(delegate
							{
								scene.JumpTo(schedule);
								joinCompleted();
							});
						}
					});
					return scene;
				}
				if (schedule.world == playSchedule.world && schedule.chapter != playSchedule.chapter && UniverseConfig.Get().HasThumbnailInWorld(schedule.world))
				{
					return JoinSelectionIslandScene(schedule.world, delegate(bool success)
					{
						if (success)
						{
							AudioUtility.GetSound().Play("Audios/selectIsland.mp3");
							try
							{
								UnityEngine.Object.FindObjectOfType<SelectionIslandScene>().JumpTo(schedule.chapter, delegate(IslandScene islandScene)
								{
									islandScene.JumpTo(schedule);
									joinCompleted();
								});
							}
							catch (Exception ex)
							{
								UnityEngine.Debug.Log(ex.Message);
							}
						}
					});
				}
				WorldScene scene2 = SingletonClass<MySceneManager>.Get().Navigation<WorldScene>("Scenes/WorldScene");
				scene2.OnStart(playSchedule.world);
				scene2.AddLoadListener(delegate(bool success)
				{
					if (!success)
					{
						scene2.JumpTo(schedule.world, delegate(SelectionIslandScene selectIsLandScene)
						{
							if (!(selectIsLandScene == null))
							{
								selectIsLandScene.JumpTo(schedule.chapter, delegate(IslandScene islandScene)
								{
									islandScene.JumpTo(schedule);
									joinCompleted();
								});
							}
						});
					}
				});
				return scene2;
			});
		}

		public static bool HasMapTips()
		{
			return PlayData.Get().HasLevelData(0, 0, 19) && !PlayData.Get().HasLevelData(0, 1, 0) && !AuxiliaryData.Get().HasVisitIsland(0, 1);
		}

		public static bool HasWorldTips()
		{
			return PlayData.Get().HasCompleted(0) && UniverseConfig.Get().GetWorldConfig(1) != null && !PlayData.Get().HasLevelData(1, 0, 0) && !AuxiliaryData.Get().HasVisitIsland(1, 0);
		}

		public static bool ClickMapTip()
		{
			IslandScene islandScene = UnityEngine.Object.FindObjectOfType<IslandScene>();
			if (HasMapTips() && islandScene != null && islandScene.menuUIRight != null && islandScene.menuUIRight._MapButton.gameObject.activeSelf)
			{
				if (islandScene.menuUIRight._MapButton.gameObject.activeSelf)
				{
					islandScene.SetCanvasGraphicRaycaster(enabled: true);
					islandScene.menuUIRight._MapButton.onClick.RemoveAllListeners();
					CreateButtonTips(islandScene.menuUIRight._MapButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_click_map"), delegate
					{
						JoinPlayLevel(new ScheduleData(0, 1, 0));
					});
				}
				return true;
			}
			return false;
		}

		public static bool ClickWorldTip()
		{
			MenuUIRight menuUIRight = FindRight();
			if (HasWorldTips() && menuUIRight != null && menuUIRight._WorldButton.gameObject.activeSelf)
			{
				menuUIRight._WorldButton.onClick.RemoveAllListeners();
				CreateButtonTips(menuUIRight._WorldButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("unlockWorld_Tip"), delegate
				{
					JoinPlayLevel(new ScheduleData(1, 0, 0));
				});
				return true;
			}
			return false;
		}

		public static bool LeaderBoardTips()
		{
			if (SingletonBehaviour<LeaderBoardUtility>.Get().IsUploadEnable && !RankCoinData.Get().IsTips)
			{
				RankCoinData.Get().IsTips = true;
				SingletonClass<MySceneManager>.Get().Popup<LeaderboarGuidePopup>("Scenes/Pops/LeaderboarGuidePopup").OnStart(isClan: false, RankCoinData.Get().Staged, SingletonBehaviour<LeaderBoardUtility>.Get().GetRewards())
					.AddClosedListener(delegate
					{
						LeaderboardButtonUI.TryShowLeaderboard();
					});
				return true;
			}
			return false;
		}

		public static bool RewardRowTips(UnityAction unityAction = null)
		{
			if (AuxiliaryData.Get().RewardRowOpen && !AuxiliaryData.Get().RewardRowDay && SaleData.Get().openDays >= 2)
			{
				SingletonClass<MySceneManager>.Get().Popup<DailyBonusScene>("Scenes/DailyBonusScene").AddClosedListener(unityAction);
				return true;
			}
			return false;
		}

		public static bool ShowExpertLocking()
		{
			MenuUIRight menuUIRight = FindRight();
			if (menuUIRight == null)
			{
				return false;
			}
			if (!menuUIRight.ExportButton.gameObject.activeSelf)
			{
				return false;
			}
			if (!AuxiliaryData.Get().HasView("_ExpertLocking"))
			{
				CreateButtonTips(menuUIRight.ExportButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("Expert_UnLock_desc"), delegate
				{
					AuxiliaryData.Get().PutView("_ExpertLocking");
				});
				return true;
			}
			return false;
		}

		public static bool ShowBoosterCurrencyTipPopup()
		{
			if (!AuxiliaryData.Get().HasView("_BoosterCurrencyTipPopup"))
			{
				AuxiliaryData.Get().PutView("_BoosterCurrencyTipPopup");
				SingletonClass<MySceneManager>.Get().Popup<CommonPopScene>("Scenes/Pops/BoosterCurrencyTipPopup").OnStart(delegate
				{
					SingletonClass<MySceneManager>.Get().Close();
				});
				return true;
			}
			return false;
		}

		public static bool InboxTips()
		{
			MenuUIRight menuUIRight = FindRight();
			if (PlayData.Get().HasThanLevelData(0, 0, 0) && AuxiliaryData.Get().DailyBonusRewards > 0 && menuUIRight != null && menuUIRight.InBoxButton.gameObject.activeSelf && !AuxiliaryData.Get().IsInboxOpen)
			{
				try
				{
					CreateButtonTips(menuUIRight.InBoxButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("InboxOpen_Desc"), delegate
					{
						AuxiliaryData.Get().IsInboxOpen = true;
					});
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log(ex.Message);
				}
				return true;
			}
			return false;
		}

		public static bool FreeCoinsTips()
		{
			if (AuxiliaryData.Get().FreeCoinsCount == 0 && !AuxiliaryData.Get().FreeCoinsTips && AuxiliaryData.Get().IsCollect())
			{
				MenuUIRight menuUIRight = FindRight();
				if (menuUIRight != null && menuUIRight.FreeCoinsButton.gameObject.activeSelf)
				{
					CreateButtonTips(menuUIRight.FreeCoinsButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_click_freeCoins"), delegate
					{
						AuxiliaryData.Get().FreeCoinsTips = true;
					});
					return true;
				}
			}
			return false;
		}

		public static bool SpecialActiveTips(UnityAction unityAction)
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().IsActive() && !AuxiliaryData.Get().HasView("Special_Active") && AuxiliaryData.Get().IsDailyActive("Special_Active") && SingletonBehaviour<SpecialActivityUtility>.Get().ShowSpecialActivity(unityAction))
			{
				AuxiliaryData.Get().PutDailyCompleted("Special_Active");
				return true;
			}
			return false;
		}

		public static bool SpecialSaleTips(UnityAction unityAction)
		{
			if (SingletonBehaviour<SpecialActivityUtility>.Get().IsActive() && AuxiliaryData.Get().IsDailyActive("Special_Sale"))
			{
				AuxiliaryData.Get().PutDailyCompleted("Special_Sale");
				SingletonBehaviour<SpecialActivityUtility>.Get().ShowSpecialActivitySale(unityAction);
				return true;
			}
			return false;
		}

		public static bool LeaderBoardInfo(UnityAction unityAction = null)
		{
			if (AuxiliaryData.Get().IsDailyActive("DailyLeaderBoardMini") && SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType() == RankType.Upload && SingletonBehaviour<LeaderBoardUtility>.Get().RewardRequest == RequestState.Success && SingletonData<RankCache>.Get().Rank > 0 && SingletonData<RankCache>.Get().RankId.Equals(RankCoinData.Get().NewRankId))
			{
				AuxiliaryData.Get().PutDailyCompleted("DailyLeaderBoardMini");
				SingletonClass<MySceneManager>.Get().Popup<LeaderBoardTipScene>("Scenes/LeaderBoardTipScene").AddClosedListener(unityAction);
				return true;
			}
			return false;
		}

		public static bool CheatShowing(UnityAction unityAction)
		{
			if (VerificationUtility.Get().IsCheatShowing())
			{
				VerificationUtility.Get().SetViewCheat();
				TipPopupNoIconScene.ShowCheatUserTips(unityAction);
				return true;
			}
			return false;
		}

		private static bool ShowBankTips(UnityAction unityAction = null)
		{
			if (CoinBankData.Get().IsBankOpening())
			{
				if (CoinBankData.Get().IsOpened)
				{
					return false;
				}
				CoinBankData.Get().IsOpened = true;
				BankButtonUI.UUUUpdateUI();
				SingletonClass<MySceneManager>.Get().Popup<BankCoinsScene>("Scenes/BankCoinsScene").AddClosedListener(unityAction);
				return true;
			}
			return false;
		}

		public static bool ShowClubOpenTips()
		{
			if (AuxiliaryData.Get().HasView("ClubOpenTips"))
			{
				return false;
			}
			if (SingletonBehaviour<ClubSystemHelper>.Get().IsActive())
			{
				MenuUIRight menuUIRight = FindRight();
				if (menuUIRight != null && menuUIRight.ClubButton.gameObject.activeSelf)
				{
					CreateButtonTips(menuUIRight.ClubButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_click_club"), delegate
					{
						AuxiliaryData.Get().PutView("ClubOpenTips");
					});
					return true;
				}
				return true;
			}
			return false;
		}

		public static bool LaunchToJoinIslandDetailScene()
		{
			SaleData.Get().RemoveInvalidSale();
			if (ClickMapTip())
			{
				return true;
			}
			if (ClickWorldTip())
			{
				return true;
			}
			if (ShowExpertLocking())
			{
				return true;
			}
			if (LeaderBoardTips())
			{
				return true;
			}
			if (LeaderBoardInfo())
			{
				return true;
			}
			if (ShowClubOpenTips())
			{
				return true;
			}
			if (ShowBankTips())
			{
				return true;
			}
			if (ShowBoosterCurrencyTipPopup())
			{
				return true;
			}
			return false;
		}

		public static void JoinToIslandDetailScene(UnityAction unityAction = null)
		{
			SaleData.Get().RemoveInvalidSale();
			if (CheatShowing(delegate
			{
				JoinToIslandDetailScene(unityAction);
			}) || ClickMapTip() || ClickWorldTip() || ShowBankTips(unityAction))
			{
				return;
			}
			if (unityAction == null)
			{
				if (AuxiliaryData.Get().GetCoinBack() > AuxiliaryData.Get().DailyBonusRewards)
				{
					if (FreeCoinsTips() || InboxTips())
					{
						return;
					}
				}
				else if (InboxTips() || FreeCoinsTips())
				{
					return;
				}
			}
			if (!SpecialActiveTips(unityAction) && !SpecialSaleTips(unityAction) && (unityAction != null || (!ShowBoosterCurrencyTipPopup() && !LeaderBoardTips() && !ShowClubOpenTips() && !ShowExpertLocking())) && !LeaderBoardInfo(unityAction) && !SaleData.Get().PutHightScore(unityAction) && !SaleData.Get().CalcInvalidSale(unityAction) && !SingletonClass<OptimizationSystem>.Get().Analysis(unityAction) && !RewardRowTips(unityAction) && unityAction != null)
			{
				unityAction();
			}
		}

		public static void CreateButtonTips(Button button, JoinEffectDir dir, string des, UnityAction unityAction = null, float position = -450f)
		{
			string key = $"ButtonTips_{button.gameObject.name}_{button.GetInstanceID()}";
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey(key);
			Canvas canvas = button.GetComponent<Graphic>().canvas;
			CommonGuideUtility common = CommonGuideUtility.CreateCommonGuideUtilityBy(canvas.transform);
			(common.transform as RectTransform).anchoredPosition = new Vector3(0f, position, 0f);
			common.CreateGuide(des, 0f, null);
			SingletonBehaviour<GlobalConfig>.Get().CreateLightArrow(button, dir, delegate
			{
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey(key);
				common.CloseGuide();
				if (unityAction != null)
				{
					unityAction();
				}
			});
			common.transform.SetAsLastSibling();
		}

		public static void CreateTips(RectTransform rectTransform, JoinEffectDir dir, string des, string full, UnityAction unityAction = null, float position = -450f)
		{
			string key = $"Tips_{rectTransform.gameObject.name}_{rectTransform.GetInstanceID()}";
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey(key);
			Canvas canvas = rectTransform.gameObject.GetComponentInChildren<Graphic>().canvas;
			CommonGuideUtility common = CommonGuideUtility.CreateCommonGuideUtilityBy(canvas.transform);
			(common.transform as RectTransform).anchoredPosition = new Vector3(0f, position, 0f);
			common.CreateGuide(des, 0f, null);
			GameObject MaskObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("FullSceneGuide"));
			FullScreenGuide component = MaskObject.GetComponent<FullScreenGuide>();
			component.Target = rectTransform;
			component.Canvas = canvas;
			component.transform.SetParent(canvas.transform, worldPositionStays: false);
			component.OnStart(full, delegate
			{
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey(key);
				common.CloseGuide();
				UnityEngine.Object.Destroy(MaskObject);
				if (unityAction != null)
				{
					unityAction();
				}
			});
			common.transform.SetAsLastSibling();
		}

		public static MenuUIRight FindRight()
		{
			IslandScene islandScene = UnityEngine.Object.FindObjectOfType<IslandScene>();
			if (islandScene != null)
			{
				return islandScene.menuUIRight;
			}
			return MenuUIRight.GetMenu();
		}

		private static void PurchaseListener()
		{
			SingletonBehaviour<UnityPurchasingHelper>.Get().Clear();
			SingletonBehaviour<UnityPurchasingHelper>.Get().Append(delegate(string transactionID, PurchasingPackage package)
			{
				if (package != null)
				{
					StatisticsData.Get().PutPurchasingPackage(transactionID, package);
					PurchasingCommodity[] commoditys = package.commoditys;
					foreach (PurchasingCommodity purchasingCommodity in commoditys)
					{
						if (purchasingCommodity.boosterType == BoosterType.Poker)
						{
							PokerData.Get().PutPoker(purchasingCommodity.count);
						}
						else if (purchasingCommodity.boosterType == BoosterType.DoubleStar)
						{
							RankCoinData.Get().AppendDoubleStarByThreeHours(purchasingCommodity.count);
							MenuUITopLeft.UpdateDoubleStarRemianUI();
						}
						else if (purchasingCommodity.boosterType == BoosterType.UnlimitedPlay)
						{
							purchasingCommodity.count = Mathf.Max(purchasingCommodity.count * 30, 30);
							AuxiliaryData.Get().AppendUnlimitedByMinutes(purchasingCommodity.count);
							MenuUITopLeft.UpdateUnlimitedPlayRemianUI();
						}
						else if (purchasingCommodity.boosterType == BoosterType.World)
						{
							PlayData.Get().PutWorldData(purchasingCommodity.count);
						}
						else if (purchasingCommodity.boosterType == BoosterType.ClubStore)
						{
							SingletonBehaviour<ClubSystemHelper>.Get().SendGift(purchasingCommodity.count);
						}
						else if (purchasingCommodity.boosterType != BoosterType.CoinBank)
						{
							if (purchasingCommodity.boosterType == BoosterType.GuessGame)
							{
								UnityEngine.Object.FindObjectOfType<GuessGame>().HasVaule(delegate(GuessGame e)
								{
									e.CheckOver();
								});
							}
							else
							{
								SessionData.Get().PutCommodity(purchasingCommodity.boosterType, CommoditySource.Buy, purchasingCommodity.count, purchasingCommodity.boosterType != BoosterType.Coins);
							}
						}
					}
					SolitaireTripeaksData.Get().FlushData();
					SingletonData<PurchasingData>.Get().PutBuySuccessPackage(package);
					SingletonBehaviour<TripeaksLogUtility>.Get().UploadPurchasingPackage(transactionID, package);
					SingletonBehaviour<SynchronizeUtility>.Get().UploadGameData();
					List<PurchasingCommodity> list = package.commoditys.ToList();
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.World);
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.UnlimitedPlay);
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.DoubleStar);
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.Poker);
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.WildEvent);
					list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.GuessGame);
					if (list.Count == 1)
					{
						list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.Wild);
						list.RemoveAll((PurchasingCommodity e) => e.boosterType == BoosterType.Rocket);
					}
					if (list.Find((PurchasingCommodity e) => e.boosterType == BoosterType.ClubStore) != null)
					{
						LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_popup.json");
						TipPopupNoIconScene.ShowTitleDescription(localizationUtility.GetString("ClubStore_Buy_Title"), localizationUtility.GetString("ClubStore_Buy_Description"));
					}
					else if (list.Find((PurchasingCommodity e) => e.boosterType == BoosterType.CoinBank) != null)
					{
						SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<BankCoinsScene>());
						PurchasingCommodity purchasingCommodity2 = list.Find((PurchasingCommodity e) => e.boosterType == BoosterType.CoinBank);
						SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Buy, purchasingCommodity2.count, changed: false);
						PurchasSuccessPopup.ShowPurchasSuccessPopup(new PurchasingCommodity[1]
						{
							new PurchasingCommodity
							{
								boosterType = BoosterType.Coins,
								count = purchasingCommodity2.count
							}
						}, delegate
						{
							SingletonClass<MySceneManager>.Get().Popup<BankCoinsScene>("Scenes/BankCoinsScene");
						});
						CoinBankData.Get().RestBank();
					}
					else if (list.Count > 0)
					{
						PurchasSuccessPopup.ShowPurchasSuccessPopup(list.ToArray(), delegate
						{
							SaleData.Get().PutStoreSale(package);
							WorldScene.PurchasingSuccess(package);
						});
					}
					else if (package.HasBoosters(BoosterType.UnlimitedPlay) && package.HasBoosters(BoosterType.DoubleStar))
					{
						SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<SaleScene>());
						SingletonClass<MySceneManager>.Get().Close(UnityEngine.Object.FindObjectOfType<SaleGroupScene>());
					}
				}
			});
		}
	}
}
