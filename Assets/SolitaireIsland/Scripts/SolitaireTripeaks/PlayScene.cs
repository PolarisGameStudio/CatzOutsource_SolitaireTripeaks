using DG.Tweening;
using Nightingale.Ads;
using Nightingale.Inputs;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	[RequireComponent(typeof(OperatingHelper))]
	public class PlayScene : BaseScene
	{
		public Image backgroundImage;

		private static PlayScene playScene;

		public Button VideoStepButton;

		public Button BuyStepButton;

		public Button CloseButton;

		public Button UndoButton;

		public BoosterStoreItem WildButton;

		public BoosterStoreItem RocketButton;

		public List<BoosterItemUI> BoosterItems;

		public Transform BoosterParentTransform;

		public RectTransform OverRectTransform;

		public CashText UndoLabel;

		public bool HasMissCard;

		private bool GuideTiping;

		private bool FreeUnDoing;

		private bool watchCompleted;

		private MenuUITopLeft _MenuUITopLeft;

		private Canvas canvas;

		public static PlayScene Get()
		{
			if (playScene == null)
			{
				playScene = UnityEngine.Object.FindObjectOfType<PlayScene>();
			}
			return playScene;
		}

		private void WatchAdComeleted(bool completed)
		{
			if (completed && HandCardSystem.Get() != null && PlayDesk.Get() != null && !PlayDesk.Get().IsGameOver)
			{
				watchCompleted = true;
				HandCardSystem.Get().AppendLeftCards(3);
				AuxiliaryData.Get().WatchVideoCount++;
				AuxiliaryData.Get().PutDailyOnce("WatchAdAddCards");
				OperatingHelper.Get().ClearStep();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(PlayScene).Name);
			SingletonBehaviour<ThirdPartyAdManager>.Get().compeleted.RemoveListener(WatchAdComeleted);
		}

		private void Start()
		{
			PlayLogic();
			CreatorDesk();
			CreatorBackgroundAndPlayMusic();
			OnLoadCompeted();
			AddListener();
			CreatorMenuUI();
			CreatorBoosters();
		}

		private void PlayLogic()
		{
			try
			{
				playScene = this;
				SingletonBehaviour<ThirdPartyAdManager>.Get().compeleted.AddListener(WatchAdComeleted);
				SingletonClass<OnceGameData>.Get().Rest();
				AuxiliaryData.Get().PlayNumber++;
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 1f;
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.Play);
				AchievementData.Get().DoAchievement(AchievementType.Play);
				canvas = base.transform.Find("Canvas").GetComponent<Canvas>();
				ScoringSystem.Get().SetLevel();
				UndoLabel.SetSmallCash(ScoringSystem.Get().UndoCoins);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void CreatorDesk()
		{
			try
			{
				GameObject gameObject = new GameObject("Card Desk");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				gameObject.transform.localPosition = new Vector3(0f, 0.1f, 0f);
				gameObject.AddComponent<PlayDesk>();
				gameObject.AddComponent<PlayAdditional>();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void CreatorBackgroundAndPlayMusic()
		{
			try
			{
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				AssetBundle detailsAssetBundle = SingletonClass<AAOConfig>.Get().GetChapterConfig(playSchedule.world, playSchedule.chapter).GetDetailsAssetBundle();
				if (detailsAssetBundle == null)
				{
					detailsAssetBundle = UniverseConfig.Get().GetChapterConfig().GetDetailsAssetBundle();
				}
				if (!(detailsAssetBundle == null))
				{
					SingletonBehaviour<GlobalConfig>.Get().PlayBackground(detailsAssetBundle.LoadAsset<AudioClip>("music.mp3"));
					GameObject gameObject = detailsAssetBundle.LoadAsset<GameObject>("playAnimation");
					if (gameObject == null)
					{
						backgroundImage.sprite = detailsAssetBundle.LoadAsset<Sprite>("play.png");
					}
					else
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, backgroundImage.transform.parent);
						float num = 1.77777779f * (float)Screen.height;
						if (num < (float)Screen.width)
						{
							float num2 = (float)(200 * Screen.width) / num;
							gameObject2.transform.localScale = new Vector3(num2, num2, num2);
						}
						backgroundImage.gameObject.SetActive(value: false);
					}
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void AddListener()
		{
			try
			{
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				RocketButton.gameObject.SetActive(SingletonClass<AAOConfig>.Get().GetLevel(playSchedule) >= 19 || AuxiliaryData.Get().RocketOpen);
				UndoButton.onClick.AddListener(delegate
				{
					if (PlayDesk.Get() != null && PlayDesk.Get().IsPlaying)
					{
						if (FreeUnDoing || PackData.Get().GetCommodity(BoosterType.Coins).GetTotal() >= ScoringSystem.Get().UndoCoins)
						{
							if (OperatingHelper.Get().UndoStep())
							{
								AudioUtility.GetSound().Play("Audios/button.mp3");
								if (!FreeUnDoing)
								{
									SingletonClass<OnceGameData>.Get().Use(BoosterType.Undo);
									SingletonClass<OnceGameData>.Get().UndoCoins += ScoringSystem.Get().UndoCoins;
									SessionData.Get().UseCommodity(BoosterType.Coins, ScoringSystem.Get().UndoCoins, "UndoStep");
									ScoringSystem.Get().UndoOnce();
								}
								else
								{
									AuxiliaryData.Get().FreeUndoTotal--;
								}
								UndoLabel.SetSmallCash(ScoringSystem.Get().UndoCoins);
								GlobalBoosterUtility.Get().OpenPoker();
							}
						}
						else
						{
							StoreScene.ShowOutofCoins();
						}
					}
				});
				VideoStepButton.onClick.AddListener(delegate
				{
					SingletonBehaviour<ThirdPartyAdManager>.Get().ShowRewardedVideoAd();
				});
				CloseButton.onClick.AddListener(delegate
				{
					if (PlayDesk.Get() != null && PlayDesk.Get().IsPlaying)
					{
						PlayDesk.Get().GiveUp();
					}
				});
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void CreatorMenuUI()
		{
			try
			{
				_MenuUITopLeft = MenuUITopLeft.CreateMenuUITopLeft(base.transform, hasEsc: false);
				_MenuUITopLeft.transform.localPosition = new Vector3(0f, 300f, 0f);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		private void CreatorBoosters()
		{
			try
			{
				BoosterItems = new List<BoosterItemUI>();
				GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/Booster");
				float num = -85f;
				foreach (BoosterType insideBooster in SingletonClass<AAOConfig>.Get().GetLevelConfig().GetInsideBoosters())
				{
					BoosterItemUI component = UnityEngine.Object.Instantiate(asset).GetComponent<BoosterItemUI>();
					component.OnStart(insideBooster, delegate(BoosterType boosterType)
					{
						GlobalBoosterUtility.Get().PutBooster(boosterType);
					});
					component.transform.SetParent(BoosterParentTransform, worldPositionStays: false);
					component.transform.localPosition = new Vector3(num, 300f, 0f);
					BoosterItems.Add(component);
					num -= 140f;
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.Message);
			}
		}

		public void CalcBoosterEffect()
		{
			if (!(PlayDesk.Get() == null))
			{
				foreach (BoosterItemUI item in BoosterItems)
				{
					if (item.boosterType == BoosterType.SnakeEliminate)
					{
						item.Breathe(PlayDesk.Get().Uppers.Count((BaseCard e) => e is ForkCard || e is SnakeCard) > 0);
					}
					else
					{
						ExtraTypeNodeConfig finder = AppearNodeConfig.Get().ExtraTypeNodeConfigs.ToList().Find((ExtraTypeNodeConfig e) => e.booster == item.boosterType);
						if (finder != null)
						{
							item.Breathe(PlayDesk.Get().Uppers.Find((BaseCard poker) => poker.HasExtras(finder.extraType)) != null);
						}
					}
				}
			}
		}

		public void AppendProp<T>(int number = 0) where T : NormalBooster
		{
			GameObject gameObject = new GameObject(typeof(T).Name);
			gameObject.transform.SetParent(canvas.transform, worldPositionStays: false);
			T val = gameObject.AddComponent<T>();
			val.OnStart(number);
		}

		public override void OnSceneStateChanged(SceneState state)
		{
			switch (state)
			{
			case SceneState.Opened:
			case SceneState.Showed:
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 1f;
				break;
			case SceneState.Opening:
			case SceneState.Hiding:
			case SceneState.Hided:
			case SceneState.Showing:
			case SceneState.Closing:
			case SceneState.Closed:
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 0f;
				break;
			}
		}

		public void SetUndoButtonVisable(bool visable)
		{
			float endValue = (!visable) ? (-157) : 157;
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey("DelayUndoButton");
			UndoButton.DOKill();
			UndoButton.transform.DOLocalMoveY(endValue, 0.5f).OnComplete(delegate
			{
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("DelayUndoButton");
				if (!GuideTiping)
				{
					if (UndoButton.gameObject.activeSelf && visable && HasMissCard && OperatingHelper.Get().HasSteps() && AuxiliaryData.Get().FreeUndoTotal > 0)
					{
						FreeUnDoing = true;
						GuideTiping = true;
						UndoLabel.SetCash(0L);
						JoinPlayHelper.CreateButtonTips(UndoButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_FreeUndo"), delegate
						{
							GuideTiping = false;
							FreeUnDoing = false;
						}, -350f);
					}
					HasMissCard = false;
				}
			});
		}

		public void SetOverButtons(bool visable)
		{
			float endValue = (!visable) ? (-143) : 143;
			VideoStepButton.interactable = false;
			if (visable)
			{
				if (!watchCompleted && (StatisticsData.Get().IsLowPlayer() || AuxiliaryData.Get().GetDailyNumber("WatchAdAddCards") < 1) && SingletonBehaviour<ThirdPartyAdManager>.Get().IsRewardedVideoAvailable(AuxiliaryData.Get().WatchVideoCount) && PlayData.Get().HasThanLevelData(0, 0, 6))
				{
					VideoStepButton.transform.DOLocalMoveY(endValue, 0.5f).OnComplete(delegate
					{
						VideoStepButton.interactable = true;
					});
				}
			}
			else
			{
				VideoStepButton.transform.DOLocalMoveY(endValue, 0.5f);
			}
			BuyStepButton.interactable = false;
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey("DelayBuyStepButton");
			BuyStepButton.DOKill();
			BuyStepButton.transform.DOLocalMoveY(endValue, 0.5f).OnComplete(delegate
			{
				BuyStepButton.interactable = visable;
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("DelayBuyStepButton");
				if (!GuideTiping && AuxiliaryData.Get().FreeHandTotal > 0 && visable)
				{
					GuideTiping = true;
					JoinPlayHelper.CreateButtonTips(BuyStepButton, JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_FreeHand"), delegate
					{
						GuideTiping = false;
					}, -350f);
				}
			});
			CloseButton.transform.DOLocalMoveY((!visable) ? (-200) : 122, 0.5f);
		}

		public void SetButtonLayerVisable(bool visable)
		{
			float endValue = (!visable) ? (-200) : 130;
			int num = 0;
			VideoStepButton.interactable = visable;
			if (!visable)
			{
				VideoStepButton.transform.DOLocalMoveY(endValue, 0.5f);
				CloseButton.transform.DOLocalMoveY(endValue, 0.7f).SetDelay(0.07f * (float)num++);
				BuyStepButton.transform.DOLocalMoveY(endValue, 0.7f).SetDelay(0.07f * (float)num++);
				UndoButton.transform.DOLocalMoveY(endValue, 0.7f).SetDelay(0.07f * (float)num++);
			}
			RocketButton.transform.DOLocalMoveY(endValue, 0.7f).SetDelay(0.07f * (float)num++);
			WildButton.transform.DOLocalMoveY(endValue, 0.7f).SetDelay(0.07f * (float)num++);
			num = 0;
			if (visable)
			{
				_MenuUITopLeft.transform.DOLocalMoveY((!visable) ? 300 : 0, 0.7f).SetDelay(0.07f * (float)num++);
			}
			if (visable)
			{
				PlayStreaksSystem.Get().transform.DOLocalMoveY((!visable) ? 120 : (-90), 0.7f).SetDelay(0.07f * (float)num++);
			}
			foreach (BoosterItemUI boosterItem in BoosterItems)
			{
				boosterItem.transform.DOLocalMoveY((!visable) ? 300 : (-80), 0.7f).SetDelay(0.07f * (float)num++);
			}
		}
	}
}
