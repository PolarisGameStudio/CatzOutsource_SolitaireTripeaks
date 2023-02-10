using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Rates;
using Nightingale.ScenesManager;
using Nightingale.Toasts;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class IsLandDetails : DelayBehaviour
	{
		[SerializeField]
		private LevelControl[] levelControls;

		private CharacterUtility characterUtility;

		private int playerLevel;

		public Transform[] Points;

		private IslandScene islandScene;

		public int World
		{
			get;
			private set;
		}

		public int Chapter
		{
			get;
			private set;
		}

		private void OnDestroy()
		{
			SingletonBehaviour<TripeaksPlayerHelper>.Get().RemoveListener(UpdateFriendsSchedule);
		}

		private void UpdateFriendsSchedule(List<TripeaksPlayer> players)
		{
			try
			{
				LevelControl[] array = levelControls;
				foreach (LevelControl levelControl in array)
				{
					levelControl.AddFriendSchedule(players);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void ScaleXY(int player)
		{
			DOTween.Kill(GetInstanceID());
			Sequence sequence = DOTween.Sequence();
			levelControls[player].transform.localScale = Vector3.one;
			sequence.Append(levelControls[player].transform.DOScale(1.1f, 0.5f));
			sequence.Append(levelControls[player].transform.DOScale(1f, 0.5f));
			sequence.SetLoops(-1);
			sequence.SetId(GetInstanceID());
		}

		public void OnStart(IslandScene islandScene, int world, int chapter)
		{
            if (transform.Find("SmallLight") != null)
            {
                transform.Find("SmallLight").gameObject.SetActive(false);
            }

#if GET_MAP_DETAIL
			GetComponent<Image>().sprite = GameConfigManager.Instance.MapSprites[0];
#endif

            this.islandScene = islandScene;
			World = world;
			Chapter = chapter;
			AuxiliaryData.Get().PutVisitIsland(world, chapter);
			SingletonBehaviour<GlobalConfig>.Get().PlayBackground();
			(base.transform.parent as RectTransform).sizeDelta = (base.transform as RectTransform).sizeDelta - new Vector2(960f, 540f);
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(IslandScene).Name, "UI/IsLandLevel");
			levelControls = new LevelControl[Points.Length];
			for (int i = 0; i < Points.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(asset);
				gameObject.transform.SetParent(Points[i], worldPositionStays: false);
				gameObject.transform.localPosition = Vector3.zero;
				levelControls[i] = gameObject.GetComponent<LevelControl>();
				levelControls[i].SetInfo(new ScheduleData(world, chapter, i), delegate(ScheduleData schedule)
				{
					JumpTo(schedule, 0f);
				});
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(IslandScene).Name, "Prefabs/Character"));
			characterUtility = gameObject2.GetComponent<CharacterUtility>();
			characterUtility.transform.SetParent(base.transform, worldPositionStays: false);
			characterUtility.transform.SetAsLastSibling();
			characterUtility.CreateInIsland();
			playerLevel = 0;
			ChapterData chapter2 = PlayData.Get().GetChapter(world, chapter);
			if (chapter2 != null)
			{
				playerLevel = chapter2.playLevel;
				if (playerLevel > levelControls.Length - 1)
				{
					playerLevel = levelControls.Length - 1;
				}
			}
			ScaleXY(playerLevel);
			characterUtility.SetPosition(levelControls[playerLevel].transform);
			RectTransformHelper.Center(levelControls[playerLevel].transform as RectTransform, base.transform.parent as RectTransform);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("ui/Boat01UI"));
			gameObject3.transform.SetParent(base.transform.Find("Boat 01"), worldPositionStays: false);
			gameObject3 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("ui/Boat02UI"));
			gameObject3.transform.SetParent(base.transform.Find("Boat 02"), worldPositionStays: false);
			SingletonBehaviour<TripeaksPlayerHelper>.Get().AddListener(UpdateFriendsSchedule);
		}

		public void JumpTo(ScheduleData schedule, float delay)
		{
			if (islandScene == null)
			{
				islandScene = UnityEngine.Object.FindObjectOfType<IslandScene>();
			}
			if (islandScene != null)
			{
				islandScene.SetCanvasGraphicRaycaster(enabled: false);
			}
			UnityAction unityAction = delegate
			{
				ChapterData chapter = PlayData.Get().GetChapter(schedule.world, schedule.chapter);
				if (chapter != null)
				{
					chapter.playLevel = schedule.level;
				}
				if (islandScene != null)
				{
					islandScene.SetCanvasGraphicRaycaster(enabled: true);
				}
				if (playerLevel != schedule.level)
				{
					playerLevel = schedule.level;
					ScaleXY(playerLevel);
				}
				if (!AuxiliaryData.Get().IsFacebookReward && schedule.Equals(new ScheduleData(0, 0, 4)))
				{
					AuxiliaryData.Get().IsFacebookReward = true;
					SingletonBehaviour<GlobalConfig>.Get().ShowLoginFacebook(bonus: true, delegate
					{
						JumpTo(schedule, delay);
					});
				}
				else if (!AuxiliaryData.Get().RewardRowOpen && schedule.Equals(new ScheduleData(0, 0, 5)))
				{
					AuxiliaryData.Get().RewardRowOpen = true;
					SingletonClass<MySceneManager>.Get().Popup<DailyBonusScene>("Scenes/DailyBonusScene").AddClosedListener(delegate
					{
						JumpTo(schedule, delay);
					});
				}
				else if (PokerData.Get().IsPokerThemeOpen() && !PokerData.Get().IsTips)
				{
					MenuUIRight.GetMenu().OpenButtonsWithTips();
					JoinPlayHelper.CreateButtonTips(MenuUIRight.GetMenu().PokerThemeButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("PokerTreasure_Desc"), delegate
					{
						PokerData.Get().OpenPokerTheme();
						PokerThemeScene pokerThemeScene = UnityEngine.Object.FindObjectOfType<PokerThemeScene>();
						if (pokerThemeScene != null)
						{
							pokerThemeScene.AddClosedListener(delegate
							{
								JumpTo(schedule, delay);
							});
						}
					});
				}
				else
				{
					if (!AuxiliaryData.Get().PlayerRated)
					{
						List<ScheduleData> list = new List<ScheduleData>
						{
							new ScheduleData(0, 0, 8),
							new ScheduleData(0, 0, 18),
							new ScheduleData(0, 1, 18),
							new ScheduleData(0, 2, 18)
						};
						if (AuxiliaryData.Get().rates == null)
						{
							AuxiliaryData.Get().rates = new List<ScheduleData>();
						}
						if (list.Contains(schedule) && AuxiliaryData.Get().rates.Count < GameConfig.Get().ShowRateCount && !AuxiliaryData.Get().rates.Contains(schedule))
						{
							AuxiliaryData.Get().rates.Add(schedule);
							BaseRateScene baseRateScene = null;
							baseRateScene = ((!GameConfig.Get().ShowRateCoins) ? ((BaseRateScene)SingletonClass<MySceneManager>.Get().Popup<FeatureRateScene>("FeatureRateScene")) : ((BaseRateScene)SingletonClass<MySceneManager>.Get().Popup<RateScene>("RateScene")));
							baseRateScene.OnStart(delegate(RateType type)
							{
								if (type == RateType.Close)
								{
									JumpTo(schedule, delay);
								}
								else
								{
									AuxiliaryData.Get().PlayerRated = true;
									PurchasingCommodity[] array = new PurchasingCommodity[1]
									{
										new PurchasingCommodity
										{
											boosterType = BoosterType.Coins,
											count = 5000
										}
									};
									PurchasSuccessPopup.ShowPurchasSuccessPopup(array, delegate
									{
										JumpTo(schedule, delay);
										PackData.Get().GetCommodity(BoosterType.Coins).PutChanged(CommoditySource.Free);
									});
									PurchasingCommodity[] array2 = array;
									foreach (PurchasingCommodity purchasingCommodity in array2)
									{
										SessionData.Get().PutCommodity(purchasingCommodity.boosterType, CommoditySource.Free, purchasingCommodity.count, changed: false);
									}
								}
							});
							return;
						}
					}
					if (!QuestData.Get().QuestOpen && QuestsScene.IsOpen())
					{
						MenuUIRight.GetMenu().OpenButtonsWithTips();
						JoinPlayHelper.CreateButtonTips(MenuUIRight.GetMenu().QuestButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("QuestOpen_Desc"), null, -350f);
					}
					else
					{
						if (!AuxiliaryData.Get().LeaderBoardOpen && SingletonBehaviour<LeaderBoardUtility>.Get().IsOepn)
						{
							LeaderboardButtonUI leaderboardButtonUI = UnityEngine.Object.FindObjectOfType<LeaderboardButtonUI>();
							if (leaderboardButtonUI != null)
							{
								AuxiliaryData.Get().LeaderBoardOpen = true;
								JoinPlayHelper.CreateButtonTips(leaderboardButtonUI.Button, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("LeaderBoard_Desc"), delegate
								{
									Func<bool> FinderLeaderBoard = delegate
									{
										LeaderboardScene leaderboardScene = UnityEngine.Object.FindObjectOfType<LeaderboardScene>();
										if (leaderboardScene != null)
										{
											leaderboardScene.AddClosedListener(delegate
											{
												JumpTo(schedule, delay);
											});
											return true;
										}
										return false;
									};
									if (!FinderLeaderBoard())
									{
										LoadingHelper loadingHelper = UnityEngine.Object.FindObjectOfType<LoadingHelper>();
										if (loadingHelper != null && "LeaderboardRewardsScene".Equals(loadingHelper.LoadingPath))
										{
											loadingHelper.AddCloseListener(delegate
											{
												if (!FinderLeaderBoard())
												{
													JumpTo(schedule, delay);
												}
											});
										}
									}
								});
								return;
							}
						}
						if (AuxiliaryData.Get().IsTreasure(schedule))
						{
							AuxiliaryData.Get().CollectTreasure(schedule);
							SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
							SingletonClass<MySceneManager>.Get().Navigation<GuessGame>("MiniGames/Guess/GuessGameScene");
						}
						else
						{
							SingletonClass<AAOConfig>.Get().SetPlaySchedule(schedule);
							SingletonClass<MySceneManager>.Get().Popup<LevelScene>("Scenes/LevelScene", new NavigationEffect());
						}
					}
				}
			};
			if (playerLevel == schedule.level)
			{
				unityAction();
			}
			else if (schedule.level < levelControls.Length)
			{
				DelayDo(new WaitForSeconds(delay), delegate
				{
					characterUtility.Jump(levelControls[schedule.level].transform, unityAction);
				});
			}
		}
	}
}
