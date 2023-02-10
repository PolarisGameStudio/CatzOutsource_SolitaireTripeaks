using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.UIExtensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using TriPeaks.ProtoData.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeaderboardScene : SoundScene
	{
		public LoopScrollRect loopScrollRect;

		public LoopScrollRect _TopPlayerScroll;

		public LeaderboardUI _PlayerLeaderboardUI;

		public GameObject LoadGameObject;

		public GameObject LeaderBoardCdGameObject;

		public GameObject LeaderBoardSettleGameObject;

		public GameObject LeaderBoardUploadGameObject;

		public GameObject LoadRankGameObject;

		public GameObject LoadTopPlayerGameObject;

		public Image StageImage;

		public Text StageLabel;

		public Text UploadRemainTimeLabel;

		public LocalizationLabel RewardRemainTimeLabel;

		public LocalizationLabel SettleRemainTimeLabel;

		public Button DoubleButton;

		public Text DoubleRemainLabel;

		public TabGroup TabButtonGroup;

		private void Start()
		{
			base.IsStay = true;
			AuxiliaryData.Get().LeaderBoardOpen = true;
			StageImage.gameObject.SetActive(value: false);
			StageImage.sprite = SingletonBehaviour<StageIconHelper>.Get().GetSprite((int)RankCoinData.Get().Staged);
			StageLabel.text = LocalizationUtility.Get("Localization_LeaderBoard.json").GetString($"Stage_{(int)RankCoinData.Get().Staged}").ToUpper();
			ChangeRank(SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType());
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.AddListener(ChangeRank);
			SingletonBehaviour<GlobalConfig>.Get().CreateNumber(DoubleButton.gameObject, 1f, (int)PackData.Get().GetCommodity(BoosterType.DoubleStar).GetTotal(), left: false, -10f, -10f);
			DoubleButton.onClick.AddListener(delegate
			{
				if (SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType() == RankType.Upload)
				{
					SingletonBehaviour<GlobalConfig>.Get().BuyDoubleBooster();
					SingletonBehaviour<GlobalConfig>.Get().CreateNumber(DoubleButton.gameObject, 1f, (int)PackData.Get().GetCommodity(BoosterType.DoubleStar).GetTotal(), left: false, -10f, -10f);
					RepeatingUpdate();
				}
			});
			InvokeRepeating("RepeatingUpdate", 0f, 1f);
		}

		public override void OnSceneStateChanged(SceneState state)
		{
			base.OnSceneStateChanged(state);
			if (state == SceneState.Opened)
			{
				StageImage.transform.localScale = new Vector3(5f, 5f, 5f);
				StageImage.gameObject.SetActive(value: true);
				Vector3 localPosition = StageImage.transform.localPosition;
				float y = localPosition.y;
				Sequence s = DOTween.Sequence();
				s.Append(StageImage.transform.DOScale(1f, 0.2f));
			}
		}

		private void RepeatingUpdate()
		{
			switch (SingletonBehaviour<LeaderBoardUtility>.Get().GetRankType())
			{
			case RankType.Upload:
				UploadRemainTimeLabel.text = SingletonBehaviour<LeaderBoardUtility>.Get().GetUploadRemainTime().TOString();
				break;
			case RankType.Settle:
				SettleRemainTimeLabel.SetText(SingletonBehaviour<LeaderBoardUtility>.Get().GetSettleRemainTime().TOString());
				break;
			case RankType.Reward:
				RewardRemainTimeLabel.SetText(SingletonBehaviour<LeaderBoardUtility>.Get().GetRewardRemainTime().TOString());
				break;
			}
			if (RankCoinData.Get().IsDouble())
			{
				DoubleRemainLabel.text = RankCoinData.Get().GetRemainTimeString();
			}
			else
			{
				DoubleRemainLabel.text = LocalizationUtility.Get("Localization_popup.json").GetString("btn_buyDouble");
			}
		}

		private void RankLoadCompleted(LeaderboardListResponse response)
		{
			LoadRankGameObject.SetActive(value: false);
			LeaderBoardUploadGameObject.SetActive(value: true);
			List<RankUser> arrays = (from e in response.TopPlayers.ToList()
				select new RankUser(e, response.Stage, response.UpgradePosition, response.DowngradePosition)).ToList();
			loopScrollRect.objectsToFill = arrays.ToArray();
			loopScrollRect.totalCount = arrays.Count;
			loopScrollRect.RefreshCells();
			loopScrollRect.onValueChanged.RemoveAllListeners();
			if (response.TopPlayers.Count <= 7)
			{
				_PlayerLeaderboardUI.gameObject.SetActive(value: false);
			}
			else
			{
				RankUser finder = arrays.Find((RankUser e) => SolitaireTripeaksData.Get().GetPlayerId().Equals(e.PlayerId));
				if (finder != null)
				{
					_PlayerLeaderboardUI.ScrollCellContent(finder);
					loopScrollRect.onValueChanged.AddListener(delegate
					{
						_PlayerLeaderboardUI.gameObject.SetActive(value: false);
						List<LeaderboardUI> list = Object.FindObjectsOfType<LeaderboardUI>().ToList();
						list.Remove(_PlayerLeaderboardUI);
						LeaderboardUI leaderboardUI = list.Find((LeaderboardUI e) => e.RankIndex == finder.Position);
						if (leaderboardUI != null)
						{
							Vector3 vector = leaderboardUI.transform.parent.InverseTransformVector(leaderboardUI.transform.position);
							if (vector.y > 360f)
							{
								_PlayerLeaderboardUI.gameObject.SetActive(value: true);
								(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 766f);
							}
							else if (vector.y < -360f)
							{
								_PlayerLeaderboardUI.gameObject.SetActive(value: true);
								(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 110f);
							}
						}
						else if (list.Find((LeaderboardUI e) => e.RankIndex > finder.Position) == null)
						{
							_PlayerLeaderboardUI.gameObject.SetActive(value: true);
							(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 110f);
						}
						else if (list.Find((LeaderboardUI e) => e.RankIndex < finder.Position) == null)
						{
							_PlayerLeaderboardUI.gameObject.SetActive(value: true);
							(_PlayerLeaderboardUI.transform as RectTransform).anchoredPosition = new Vector2(0f, 766f);
						}
					});
				}
			}
			LoopDelayDo(delegate
			{
				if (loopScrollRect.gameObject.activeInHierarchy)
				{
					int num = arrays.FindIndex((RankUser e) => SolitaireTripeaksData.Get().GetPlayerId().Equals(e.PlayerId)) - 2;
					if (num > arrays.Count - 6)
					{
						loopScrollRect.RefillCellsFromEnd();
						return false;
					}
					if (num < 0)
					{
						num = 0;
					}
					loopScrollRect.RefillCells(num);
					return false;
				}
				return true;
			}, null);
		}

		private void TopPlayerCompleted(List<TopPlayer> players)
		{
			LoadTopPlayerGameObject.SetActive(value: false);
			_TopPlayerScroll.objectsToFill = players.ToArray();
			_TopPlayerScroll.totalCount = players.Count;
			_TopPlayerScroll.RefreshCells();
			LoopDelayDo(delegate
			{
				if (_TopPlayerScroll.gameObject.activeSelf)
				{
					_TopPlayerScroll.RefillCells();
					return false;
				}
				return true;
			}, null);
		}

		private void ChangeRank(RankType type)
		{
			switch (type)
			{
			case RankType.None:
				LoadGameObject.SetActive(value: true);
				TabButtonGroup.SetTabIndex(0);
				TabButtonGroup.SetVisable(visable: false);
				LoadRankGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				_TopPlayerScroll.ClearCells();
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				SingletonBehaviour<LeaderBoardUtility>.Get().TopPlayerEvent.RemoveListener(TopPlayerCompleted);
				SingletonBehaviour<LeaderBoardUtility>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				UploadRemainTimeLabel.text = "--:--:--";
				DoubleButton.interactable = false;
				break;
			default:
				UploadRemainTimeLabel.text = SingletonBehaviour<LeaderBoardUtility>.Get().GetUploadRemainTime().TOString();
				TabButtonGroup.SetVisable(visable: true);
				LoadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: true);
				LoadTopPlayerGameObject.SetActive(value: true);
				LeaderBoardCdGameObject.SetActive(value: false);
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				SingletonBehaviour<LeaderBoardUtility>.Get().RankEvent.AddListener(RankLoadCompleted);
				_TopPlayerScroll.ClearCells();
				SingletonBehaviour<LeaderBoardUtility>.Get().GetRank();
				SingletonBehaviour<LeaderBoardUtility>.Get().TopPlayerEvent.AddListener(TopPlayerCompleted);
				SingletonBehaviour<LeaderBoardUtility>.Get().GetTopPlayers();
				DoubleButton.interactable = true;
				break;
			case RankType.Settle:
				_PlayerLeaderboardUI.gameObject.SetActive(value: false);
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				_TopPlayerScroll.ClearCells();
				TabButtonGroup.SetTabIndex(0);
				TabButtonGroup.SetVisable(visable: false);
				LoadGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: false);
				LeaderBoardSettleGameObject.SetActive(value: true);
				LeaderBoardUploadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: false);
				SingletonBehaviour<LeaderBoardUtility>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				SingletonBehaviour<LeaderBoardUtility>.Get().TopPlayerEvent.RemoveListener(TopPlayerCompleted);
				SettleRemainTimeLabel.SetText(SingletonBehaviour<LeaderBoardUtility>.Get().GetSettleRemainTime().TOString());
				UploadRemainTimeLabel.text = "--:--:--";
				DoubleButton.interactable = false;
				break;
			case RankType.Reward:
				loopScrollRect.onValueChanged.RemoveAllListeners();
				loopScrollRect.ClearCells();
				_TopPlayerScroll.ClearCells();
				TabButtonGroup.SetTabIndex(0);
				TabButtonGroup.SetVisable(visable: false);
				LoadGameObject.SetActive(value: false);
				LeaderBoardCdGameObject.SetActive(value: true);
				LeaderBoardSettleGameObject.SetActive(value: false);
				LeaderBoardUploadGameObject.SetActive(value: false);
				LoadRankGameObject.SetActive(value: false);
				SingletonBehaviour<LeaderBoardUtility>.Get().RankEvent.RemoveListener(RankLoadCompleted);
				SingletonBehaviour<LeaderBoardUtility>.Get().TopPlayerEvent.RemoveListener(TopPlayerCompleted);
				RewardRemainTimeLabel.SetText(SingletonBehaviour<LeaderBoardUtility>.Get().GetRewardRemainTime().TOString());
				UploadRemainTimeLabel.text = "--:--:--";
				DoubleButton.interactable = false;
				break;
			}
			RepeatingUpdate();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(LeaderboardScene).Name);
			SingletonBehaviour<LeaderBoardUtility>.Get().RankEvent.RemoveListener(RankLoadCompleted);
			SingletonBehaviour<LeaderBoardUtility>.Get().RankChanged.RemoveListener(ChangeRank);
		}

		public void OnClickInfo()
		{
			SingletonClass<MySceneManager>.Get().Popup<LeaderboarGuidePopup>("Scenes/Pops/LeaderboarGuidePopup").OnStart(isClan: false, RankCoinData.Get().Staged, SingletonBehaviour<LeaderBoardUtility>.Get().GetRewards());
		}
	}
}
