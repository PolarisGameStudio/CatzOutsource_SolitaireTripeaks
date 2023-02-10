using DG.Tweening;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class LeveEndScene : BaseScene
	{
		public Button ReplayButton;

		public Button NextButton;

		public LevelStarGroupUI GameLevelStarGroupUI;

		public LevelStarGroupUI LeaderBoardLevelStarGroupUI;

		public Text StreaksCoinsLabel;

		public Text CompletionCoinsLabel;

		public LocalizationLabel ClanMoreLabel;

		public LocalizationLabel ClanPointsLabel;

		public Text WonCoinsLabel;

		public Text LevelLabel;

		public Transform ContentTransform;

		public MiniLeaderBoardUI MiniLeaderBoardUI;

		private bool isDestory;

		private bool content;

		public Transform HouseTransform;

		public Transform LightTransform;

		public Transform FlowerTransform;

		public Transform CloseTransform;

		public OverInvitePop overInvitePop;

		public LevelData blueStarLevelData;

		public LevelData levelData;

		public static LeveEndScene Create(int level)
		{
			LeveEndScene component = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeveEndScene).Name, "Scenes/LevelEndScene")).GetComponent<LeveEndScene>();
			component.gameObject.SetActive(value: false);
			component.OnAwake(level);
			return component;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(LeveEndScene).Name);
			isDestory = true;
		}

		private void OnAwake(int level)
		{
			content = RankCoinData.Get().HasTreasure(level);
			SetCanvasGraphicRaycaster(enabled: false);
		}

		public void OnStart(LevelData levelData)
		{
			base.IsStay = true;
			this.levelData = levelData;
			blueStarLevelData = RankCoinData.Get().GetLevelData(SingletonClass<AAOConfig>.Get().GetLevel(), content);
			LevelLabel.text = SingletonClass<AAOConfig>.Get().GetLevelString();
			ReplayButton.gameObject.SetActive(value: false);
			NextButton.gameObject.SetActive(value: false);
			CloseTransform.gameObject.SetActive(value: false);
			if (levelData.StarComplete)
			{
				int stars = PlayData.Get().GetStars();
				RecordDataType recordDataType = RankCoinData.Get().PutLevelData(SingletonClass<AAOConfig>.Get().GetLevel(), levelData, content);
				if (recordDataType == RecordDataType.FirstRecord || recordDataType == RecordDataType.NewRecord)
				{
					if (!StatisticsData.Get().IsCheatPlayer())
					{
						SingletonBehaviour<LeaderBoardUtility>.Get().UploadScore(RankCoinData.Get().GetRankCoinNumbers(), AuxiliaryData.Get().AvaterFileName, delegate
						{
							if (!isDestory)
							{
								MiniLeaderBoardUI.OnLeaderBoardStart(delay: true);
							}
						});
					}
				}
				else
				{
					MiniLeaderBoardUI.OnLeaderBoardStart(delay: false);
				}
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				if (PlayData.Get().PutLevelData(playSchedule, levelData) == RecordDataType.FirstRecord && playSchedule.world != -1)
				{
					SingletonBehaviour<TripeaksPlayerHelper>.Get().UploadTripeaksPlayer();
				}
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.WinRow);
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.WinGame);
				AchievementData.Get().DoAchievement(AchievementType.Win);
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.WinGameInScene, playSchedule);
				AchievementData.Get().PutAchievement(AchievementType.WinIn, playSchedule);
				int stars2 = PlayData.Get().GetStars();
				SingletonClass<QuestHelper>.Get().DoQuest(QuestType.GetNewStar, stars2 - stars);
				AchievementData.Get().CalcAchievement(playSchedule.world, playSchedule.chapter);
				AchievementData.Get().PutAchievement(AchievementType.CompeletedLevel, playSchedule);
				SingletonClass<OnceGameData>.Get().UploadSuccess(playSchedule);
				AudioUtility.GetSound().Play("Audios/Level_Completed.mp3");
				Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(LeveEndScene).Name, "Particles/Ribbons"), base.transform);
			}
			else
			{
				MiniLeaderBoardUI.OnLeaderBoardStart(delay: false);
			}
		}

		public void OpenAnimator()
		{
			base.gameObject.SetActive(value: true);
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (!StatisticsData.Get().IsCheatPlayer())
			{
				SingletonBehaviour<ClubSystemHelper>.Get().Contribute(Mathf.CeilToInt((float)SingletonClass<OnceGameData>.Get().WonTotalCoins() / 100f));
			}
			SingletonClass<QuestHelper>.Get().DoQuest(QuestType.WinCoins, SingletonClass<OnceGameData>.Get().WonTotalCoins());
			GameLevelStarGroupUI.SetAnimationLevelData(levelData);
			LeaderBoardLevelStarGroupUI.SetLeaderBoardAnimationLevelData(blueStarLevelData, levelData);
			TaskQueueOnByoneUtility taskQueueOnByoneUtility = new TaskQueueOnByoneUtility();
			float duration = 1f;
			taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
			{
				int coins3 = 0;
				duration = (float)SingletonClass<OnceGameData>.Get().CompletionCoins / 10f;
				duration = Mathf.Min(duration, 0.5f);
				AudioUtility.GetSound().PlayLoop("Audios/loop_coin.mp3", duration);
				DOTween.To(() => coins3, delegate(int coin)
				{
					CompletionCoinsLabel.text = $"{coin}";
				}, SingletonClass<OnceGameData>.Get().CompletionCoins, duration).OnComplete(delegate
				{
					if (unityAction != null)
					{
						unityAction();
					}
				});
			});
			taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
			{
				int coins2 = 0;
				duration = (float)SingletonClass<OnceGameData>.Get().StreaksCoins / 10f;
				duration = Mathf.Min(duration, 0.5f);
				AudioUtility.GetSound().PlayLoop("Audios/loop_coin.mp3", duration);
				DOTween.To(() => coins2, delegate(int coin)
				{
					StreaksCoinsLabel.text = $"{coin}";
				}, SingletonClass<OnceGameData>.Get().StreaksCoins, duration).OnComplete(delegate
				{
					if (unityAction != null)
					{
						unityAction();
					}
				});
			});
			if (SingletonClass<OnceGameData>.Get().CompletionMoreCoins > 0)
			{
				ClanMoreLabel.gameObject.SetActive(value: true);
				ClanMoreLabel.SetText(SingletonClass<OnceGameData>.Get().CompletionMoreCoins);
			}
			if (!string.IsNullOrEmpty(SingletonBehaviour<ClubSystemHelper>.Get().GetClubIdentifier()) && SingletonBehaviour<ClubSystemHelper>.Get().GetRankType() == RankType.Upload)
			{
				ClanPointsLabel.gameObject.SetActive(value: true);
				ClanPointsLabel.SetText(Mathf.CeilToInt((float)SingletonClass<OnceGameData>.Get().WonTotalCoins() / 100f));
			}
			taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
			{
				int coins = 0;
				int num = SingletonClass<OnceGameData>.Get().WonCoins();
				duration = (float)num / 10f;
				duration = Mathf.Min(duration, 1f);
				AudioUtility.GetSound().PlayLoop("Audios/loop_coin.mp3", duration);
				DOTween.To(() => coins, delegate(int coin)
				{
					WonCoinsLabel.text = $"{coin}";
				}, num, 1f).OnComplete(delegate
				{
					if (unityAction != null)
					{
						unityAction();
					}
				});
			});
			taskQueueOnByoneUtility.Run(delegate
			{
				ScheduleData nextSchedule = SingletonClass<AAOConfig>.Get().GetNextSchedule(playSchedule);
				CloseTransform.gameObject.SetActive(value: true);
				ReplayButton.gameObject.SetActive(value: true);
				NextButton.gameObject.SetActive(PlayData.Get().HasLevelData(playSchedule) && !nextSchedule.IsEmpty());
				if (playSchedule.world == 0 && playSchedule.chapter == 0 && (playSchedule.level == 0 || playSchedule.level == 1) && levelData.Star >= 3)
				{
					ReplayButton.gameObject.SetActive(value: false);
				}
				Scale(0f, NextButton.transform);
				Scale(0f, ReplayButton.transform);
				Scale(0f, CloseTransform, delegate
				{
					SetCanvasGraphicRaycaster(enabled: true);
				});
			});
			CloseTransform.localScale = Vector3.zero;
			HouseTransform.localScale = Vector3.zero;
			Sequence s = DOTween.Sequence();
			s.Append(Scale(0f, HouseTransform));
			s.Append(Scale(0f, FlowerTransform));
			s.Join(Scale(0f, LightTransform));
		}

		public void NextClick()
		{
			SetCanvasGraphicRaycaster(enabled: false);
			CloseAnimator(delegate
			{
				JoinPlayHelper.JoinPlayByGameEnd(SingletonClass<AAOConfig>.Get().GetNextSchedule(SingletonClass<AAOConfig>.Get().GetPlaySchedule()));
			});
		}

		public void ReplayClick()
		{
			SetCanvasGraphicRaycaster(enabled: false);
			CloseAnimator(delegate
			{
				JoinPlayHelper.JoinPlayByGameEnd(SingletonClass<AAOConfig>.Get().GetPlaySchedule());
			});
		}

		public void ClosedClick()
		{
			SetCanvasGraphicRaycaster(enabled: false);
			CloseAnimator(delegate
			{
				JoinPlayHelper.JoinIslandByGameEnd(SingletonClass<AAOConfig>.Get().GetPlaySchedule());
			});
		}

		public void CloseAnimator(TweenCallback tweenCallback)
		{
			overInvitePop.CloseAnimator();
			SetCanvasGraphicRaycaster(enabled: false);
			CloseTransform.DOScale(0f, 0.2f);
			NextButton.transform.DOScale(0f, 0.2f);
			ReplayButton.transform.DOScale(0f, 0.2f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(FlowerTransform.DOScale(0f, 0.2f));
			sequence.Join(LightTransform.DOScale(0f, 0.2f));
			sequence.Append(HouseTransform.DOScale(0f, 0.2f));
			sequence.OnComplete(delegate
			{
				SingletonClass<MySceneManager>.Get().Close(new NavigationEffect());
				if (tweenCallback != null)
				{
					tweenCallback();
				}
			});
		}

		private Sequence Scale(float delay, Transform transform, TweenCallback tweenCallback = null)
		{
			transform.localScale = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.PrependInterval(delay);
			sequence.Append(transform.DOScale(1.1f, 0.3f));
			sequence.Append(transform.DOScale(1f, 0.1f));
			if (tweenCallback != null)
			{
				sequence.OnComplete(tweenCallback);
			}
			return sequence;
		}
	}
}
