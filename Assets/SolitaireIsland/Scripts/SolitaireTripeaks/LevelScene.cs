using DG.Tweening;
using Nightingale.Ads;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LevelScene : BaseScene
	{
		public Text CostLabel;

		public Text LevelLabel;

		public Transform ContentTransform;

		public LevelStarGroupUI GameLevelStarGroupUI;

		public LevelStarGroupUI LeaderBoardLevelStarGroupUI;

		public LabelUI FreePlayUI;

		public RemainLabel UnlimitedPlayRemainLabel;

		public Text ExpiredPlayNumberLabel;

		public RemainLabel ExpiredPlayNumberRemainLabel;

		public WatchVideoFreeCoinsUI WatchVideoFreeCoinsUI;

		private LevelRetrunCoinConfig levelRetrunCoinConfig;

		public RectTransform HouseTransform;

		public RectTransform FlowerTransform;

		public RectTransform BottomTransform;

		public Transform ClosedTransform;

		private List<BoosterItemUI> levelSceneBoosters = new List<BoosterItemUI>();

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(LevelScene).Name);
			PackData.Get().GetCommodity(BoosterType.FreePlay).OnChanged.RemoveListener(UpdateFreeNumber);
			PackData.Get().GetCommodity(BoosterType.ExpiredPlay).OnChanged.RemoveListener(UpdateFreeNumber);
		}

		private void UpdateFreeNumber(CommoditySource commoditySource)
		{
			SessionData.Get().ClearBoosters();
			if (AuxiliaryData.Get().IsUnlimitedPlay())
			{
				ExpiredPlayNumberLabel.transform.parent.parent.gameObject.SetActive(value: false);
				FreePlayUI.gameObject.SetActive(value: false);
				CostLabel.text = LocalizationUtility.Get().GetString("Free");
				UnlimitedPlayRemainLabel.SetRemainTime(new DateTime(AuxiliaryData.Get().UnlimitedPlayTicks));
				UnlimitedPlayRemainLabel.OnCompleted.RemoveAllListeners();
				UnlimitedPlayRemainLabel.OnCompleted.AddListener(delegate
				{
					UpdateFreeNumber(CommoditySource.None);
				});
				return;
			}
			if (PackData.Get().GetCommodity(BoosterType.ExpiredPlay).GetTotal() > 0)
			{
				ExpiredPlayNumberLabel.transform.parent.parent.gameObject.SetActive(value: true);
				FreePlayUI.gameObject.SetActive(value: false);
				UnlimitedPlayRemainLabel.gameObject.SetActive(value: false);
				ExpiredPlayNumberLabel.text = $"X{PackData.Get().GetCommodity(BoosterType.ExpiredPlay).GetTotal()}";
				ExpiredPlayNumberRemainLabel.SetRemainTime(new DateTime(AuxiliaryData.Get().ClearBoostersTicks));
				ExpiredPlayNumberRemainLabel.OnCompleted.RemoveAllListeners();
				ExpiredPlayNumberRemainLabel.OnCompleted.AddListener(delegate
				{
					UpdateFreeNumber(CommoditySource.None);
				});
				return;
			}
			ExpiredPlayNumberLabel.transform.parent.parent.gameObject.SetActive(value: false);
			UnlimitedPlayRemainLabel.gameObject.SetActive(value: false);
			int num = (int)PackData.Get().GetCommodity(BoosterType.FreePlay).GetTotal();
			FreePlayUI.gameObject.SetActive(num > 0);
			if (num > 0)
			{
				FreePlayUI.gameObject.SetActive(value: true);
				FreePlayUI.SetString($"X{num}");
				CostLabel.text = LocalizationUtility.Get().GetString("Free");
			}
			else
			{
				CostLabel.text = levelRetrunCoinConfig.LevelTicketCoins.ToString();
			}
		}

		[SerializeField]
		LevelConfig levelConfig = null;
		private void Awake()
		{
			base.IsStay = true;
			PackData.Get().GetCommodity(BoosterType.FreePlay).OnChanged.AddListener(UpdateFreeNumber);
			PackData.Get().GetCommodity(BoosterType.ExpiredPlay).OnChanged.AddListener(UpdateFreeNumber);
			levelRetrunCoinConfig = SingletonClass<AAOConfig>.Get().GetLevelRetrunCoinConfig();
			LevelLabel.text = SingletonClass<AAOConfig>.Get().GetLevelString();
			GameLevelStarGroupUI.SetLevelData(PlayData.Get().GetLevelData(SingletonClass<AAOConfig>.Get().GetPlaySchedule()));
			LeaderBoardLevelStarGroupUI.SetLeaderBoardLevelData(RankCoinData.Get().GetLevelData(SingletonClass<AAOConfig>.Get().GetLevel(SingletonClass<AAOConfig>.Get().GetPlaySchedule())));
            //LevelConfig levelConfig = SingletonClass<AAOConfig>.Get().GetLevelConfig();
            levelConfig = SingletonClass<AAOConfig>.Get().GetLevelConfig();
			GameObject asset = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("UI/Booster");
			foreach (BoosterType outsideBooster in levelConfig.GetOutsideBoosters())
			{
				BoosterItemUI component = UnityEngine.Object.Instantiate(asset).GetComponent<BoosterItemUI>();
				component.transform.SetParent(ContentTransform, worldPositionStays: false);
				component.OnStart(outsideBooster);
				levelSceneBoosters.Add(component);
			}
			OnLoadCompeted();
			UpdateFreeNumber(CommoditySource.None);
			WatchVideoFreeCoinsUI.TryShow(SingletonClass<AAOConfig>.Get().GetLevel(SingletonClass<AAOConfig>.Get().GetPlaySchedule()) + 1);
			OpenAnimator();
		}

		public void OnPlayClick()
		{
			if (AuxiliaryData.Get().IsUnlimitedPlay() || SessionData.Get().UseCommodity(BoosterType.ExpiredPlay, 1L) || SessionData.Get().UseCommodity(BoosterType.FreePlay, 1L) || SessionData.Get().UseCommodity(BoosterType.Coins, levelRetrunCoinConfig.LevelTicketCoins, "Tickets"))
			{
				SetCanvasGraphicRaycaster(enabled: false);
				GlobalLoadingAnimation.Show("Scenes/LoadingGameScene", () => SingletonClass<MySceneManager>.Get().Navigation<PlayScene>("Scenes/PlayScene"));
			}
			else
			{
				long coins = PackData.Get().GetCommodity(BoosterType.Coins).GetTotal();
				StoreScene.ShowOutofCoinsInLevelScene(delegate
				{
					if (coins == PackData.Get().GetCommodity(BoosterType.Coins).GetTotal() && SingletonBehaviour<ThirdPartyAdManager>.Get().IsRewardedVideoAvailable(AuxiliaryData.Get().WatchVideoCount))
					{
						WatchVideoAdTipScene.ShowWatchAdRewardCoins();
					}
				});
			}
		}

		public void OnCloseClick()
		{
			WatchVideoFreeCoinsUI.TryHide();
			foreach (BoosterItemUI levelSceneBooster in levelSceneBoosters)
			{
				levelSceneBooster.ParticleTransform.gameObject.SetActive(value: false);
			}
			SingletonClass<MySceneManager>.Get().Close(new ScaleEffect(), delegate
			{
				if (UnityEngine.Object.FindObjectOfType<ExpertLevelScene>() == null)
				{
					JoinPlayHelper.JoinToIslandDetailScene();
				}
			});
		}

		private void OpenAnimator()
		{
			SetCanvasGraphicRaycaster(enabled: false);
			BottomTransform.localScale = Vector2.zero;
			ClosedTransform.localScale = Vector2.zero;
			HouseTransform.localScale = Vector2.zero;
			FlowerTransform.localScale = Vector2.zero;
			foreach (BoosterItemUI levelSceneBooster in levelSceneBoosters)
			{
				levelSceneBooster.ParticleTransform.gameObject.SetActive(value: false);
				levelSceneBooster.transform.localScale = Vector2.zero;
			}
			Sequence sequence = DOTween.Sequence();
			sequence.Append(HouseTransform.DOScale(1.1f, 0.3f));
			sequence.Append(HouseTransform.DOScale(1f, 0.1f));
			sequence.Append(FlowerTransform.DOScale(1.1f, 0.2f));
			sequence.Append(FlowerTransform.DOScale(1f, 0.1f));
			int num = 0;
			foreach (BoosterItemUI item in levelSceneBoosters)
			{
				sequence.Join(Scale((float)(++num) * 0.08f, item.transform, 0.8f).OnComplete(delegate
				{
					item.ParticleTransform.gameObject.SetActive(value: true);
				}));
			}
			sequence.Join(Scale((float)(++num) * 0.08f, BottomTransform));
			sequence.Join(Scale((float)(++num) * 0.08f, ClosedTransform));
			sequence.OnComplete(delegate
			{
				SetCanvasGraphicRaycaster(enabled: true);
			});
		}

		private Sequence Scale(float delay, Transform transform, float scale = 1f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.PrependInterval(delay);
			sequence.Append(transform.DOScale(scale * 1.1f, 0.3f));
			sequence.Append(transform.DOScale(scale, 0.1f));
			return sequence;
		}

		private Sequence Join(float delay, Transform transform)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.PrependInterval(delay);
			Sequence s = sequence;
			Vector3 localPosition = transform.localPosition;
			s.Append(transform.DOLocalMoveY(localPosition.y + 100f, 0.2f));
			sequence.Append(transform.DOLocalMoveY(-1080f, 0.3f));
			return sequence;
		}
	}
}
