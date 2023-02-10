using DG.Tweening;
using Nightingale.Extensions;
using Nightingale.Inputs;
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PlayDesk : DelayBehaviour
	{
		private static PlayDesk _PlayDesk;

		private float BusyTime;

		private float BusyStepTime;

		public List<BaseCard> Uppers = new List<BaseCard>();

		public List<BaseCard> Pokers = new List<BaseCard>();

		public UnityEvent OnDestopChanged = new UnityEvent();

		public UnityEvent OnCardChanged = new UnityEvent();

		public UnityEvent OnPlayChanged = new UnityEvent();

		public UnityEvent OnReadyPlayChanged = new UnityEvent();

		public ClickCardChangedEvent OnClickCardChanged = new ClickCardChangedEvent();

		private LeveEndScene leveEndScene;

		private LevelConfig _LevelConfig;

		public bool IsClickEnable = true;

		[CompilerGenerated]
		private static TaskQueue _003C_003Ef__mg_0024cache0;

		public bool IsPlaying
		{
			get;
			private set;
		}

		public bool IsGameOver
		{
			get;
			private set;
		}

		public bool IsAnimionBusy
		{
			get
			{
				if (!IsPlaying)
				{
					return true;
				}
				if (IsGameOver)
				{
					return true;
				}
				return BusyTime > 0f;
			}
		}

		public bool IsBusyHandBusy
		{
			get
			{
				if (!IsPlaying)
				{
					return true;
				}
				if (IsGameOver)
				{
					return true;
				}
				return BusyStepTime > 0f;
			}
		}

		public static PlayDesk Get()
		{
			return _PlayDesk;
		}

		private void Awake()
		{
			_PlayDesk = this;
			leveEndScene = LeveEndScene.Create(SingletonClass<AAOConfig>.Get().GetLevel());
			base.gameObject.AddComponent<SampleCardTutorial>();
			_LevelConfig = SingletonClass<AAOConfig>.Get().GetLevelConfig();
			if (_LevelConfig.Scale < 0.1f)
			{
				_LevelConfig.Scale = 1f;
			}
			base.transform.localScale = Vector3.one * _LevelConfig.Scale;
			float num = 1.77777779f * (float)Screen.height;
			if (num > (float)Screen.width)
			{
				float num2 = (float)Screen.width / num;
				base.transform.localScale *= num2;
				HandCardSystem.Get().transform.localScale = num2 * 0.95f * Vector3.one;
				Transform transform = HandCardSystem.Get().transform;
				float x = Mathf.Lerp(0f, 2.5f, 1f - num2);
				Vector3 localPosition = HandCardSystem.Get().transform.localPosition;
				transform.localPosition = new Vector3(x, Mathf.Lerp(localPosition.y, -5.4f, 1f - num2), 0f);
			}
			List<BaseCard> list = new List<BaseCard>();
			foreach (CardConfig card in _LevelConfig.Cards)
			{
				card.Init();
				BaseCard baseCard = SolitaireVariationExtensions.GetBaseCard(card, !SingletonClass<OnceGameData>.Get().IsRandom());
				baseCard.transform.SetParent(base.transform, worldPositionStays: false);
				baseCard.transform.localPosition = baseCard.Config.GetPosition();
				baseCard.transform.eulerAngles = Vector3.forward * baseCard.Config.EulerAngles;
				baseCard.ExtraInitialized();
				list.Add(baseCard);
			}
			List<List<BaseCard>> list2 = new List<List<BaseCard>>();
			List<BaseCard> list3 = CalcDown(list);
			while (list3.Count > 0)
			{
				list3 = list3.OrderBy(delegate(BaseCard e)
				{
					Vector3 position = e.transform.position;
					return position.x;
				}).ToList();
				list2.Add(list3);
				foreach (BaseCard item in list3)
				{
					item.gameObject.SetActive(value: false);
					list.Remove(item);
				}
				list3 = CalcDown(list);
			}
			if (!SingletonClass<OnceGameData>.Get().IsTutorial())
			{
				List<BaseCard> list4 = (from e in list2[0]
					where e.GetType() == typeof(NumberCard) && e.GetExtra<BaseExtra>() == null
					select e).ToList();
				if (list4.Count > 0)
				{
					List<CardProbability> list5 = HandConfig.GetNormal().DeskCardProbabilitys.ToList();
					foreach (CardProbability item2 in list5)
					{
						if (MathUtility.Probability(item2.probability))
						{
							BaseCard baseCard2 = list4[UnityEngine.Random.Range(0, list4.Count)];
							list4.Remove(baseCard2);
							list2[0].Remove(baseCard2);
							CardConfig cardConfig = default(CardConfig);
							cardConfig.CardType = item2.GetCardType();
							cardConfig.EulerAngles = baseCard2.Config.EulerAngles;
							cardConfig.Position = baseCard2.Config.Position;
							cardConfig.zIndex = baseCard2.Config.zIndex;
							cardConfig.Index = baseCard2.Config.Index;
							CardConfig config = cardConfig;
							BaseCard baseCard3 = SolitaireVariationExtensions.GetBaseCard(config, random: false);
							baseCard3.transform.SetParent(base.transform, worldPositionStays: false);
							list2[0].Add(baseCard3);
							UnityEngine.Object.Destroy(baseCard2.gameObject);
							if (list4.Count <= 0)
							{
								break;
							}
						}
					}
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				Pokers.AddRange(list2[i]);
			}
			Sequence sequence = null;
			for (int j = 0; j < Pokers.Count; j++)
			{
				Pokers[j].Config.zIndex = j * 10;
				Pokers[j].UpdateOrderLayer(Pokers[j].Config.zIndex);
				Pokers[j].gameObject.SetActive(value: true);
				float duration = 0.7f;
				float num3 = 0.08f;
				Pokers[j].transform.position = new Vector3(-9.6f, 7f, -0.1f);
				Pokers[j].transform.eulerAngles = new Vector3(90f, 90f, 0f);
				Sequence sequence2 = DOTween.Sequence();
				sequence2.PrependInterval(0.5f);
				sequence2.Join(Pokers[j].transform.DORotate(new Vector3(0f, 0f, Pokers[j].Config.EulerAngles), duration));
				sequence = DOTween.Sequence();
				sequence.PrependInterval((float)j * num3);
				sequence.Append(Pokers[j].transform.DOLocalMove(Pokers[j].Config.GetPosition(), duration));
				sequence.Join(sequence2);
				sequence.SetEase(Ease.Linear);
			}
			sequence.OnComplete(delegate
			{
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				if (playSchedule.Equals(new ScheduleData(0, 0, 0)) && !PlayData.Get().HasLevelData(playSchedule))
				{
					base.gameObject.AddComponent<Sample01Tutorial>();
				}
				else if (playSchedule.Equals(new ScheduleData(0, 0, 1)) && !PlayData.Get().HasLevelData(playSchedule))
				{
					base.gameObject.AddComponent<Sample02Tutorial>();
				}
				HandCardSystem.Get().OnStart(playSchedule, _LevelConfig.HandCount, DelayStart);
				Totem.Create(_LevelConfig.Objects);
			});
			AudioUtility.GetSound().Play("Audios/Pokering.mp3");
			SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/Character");
		}

		public bool IsTop(BaseCard baseCard, List<BaseCard> pokers)
		{
			Collider[] array = Physics.OverlapBox(baseCard.transform.position, baseCard.GetHalfExtents(), baseCard.transform.rotation);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				BaseCard component = collider.gameObject.GetComponent<BaseCard>();
				if (component != null && collider.gameObject.activeSelf && collider.gameObject != baseCard.gameObject && pokers.Contains(component) && component.Config.zIndex > baseCard.Config.zIndex)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsDwon(BaseCard baseCard, List<BaseCard> pokers)
		{
			Collider[] array = Physics.OverlapBox(baseCard.transform.position, baseCard.GetHalfExtents(), baseCard.transform.rotation);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (collider.gameObject.activeSelf)
				{
					BaseCard component = collider.gameObject.GetComponent<BaseCard>();
					if (!(component == null) && !(collider.gameObject == baseCard.gameObject) && pokers.Contains(component) && component.Config.zIndex < baseCard.Config.zIndex)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool CalcTopCard(List<BaseCard> opening = null)
		{
			List<BaseCard> uppers = Uppers.ToList();
			Uppers.Clear();
			List<BaseCard> list = Pokers.ToList();
			foreach (BaseCard item in list)
			{
				if (IsTop(item, list))
				{
					Uppers.Add(item);
					if (item.TryOpenCard())
					{
						opening?.Add(item);
					}
				}
			}
			if ((from e in Uppers
				where !uppers.Contains(e)
				select e).ToList().Find((BaseCard e) => !(e is NumberCard) || (bool)e.GetExtra<BaseExtra>()) != null)
			{
				OperatingHelper.Get().ClearStep();
				return true;
			}
			return false;
		}

		public List<T> GetExtras<T>() where T : BaseExtra
		{
			List<T> list = (from e in Uppers
				select e.GetExtra<T>()).ToList();
			list.RemoveAll((T e) => (UnityEngine.Object)e == (UnityEngine.Object)null);
			return list;
		}

		public List<T> GetAllExtras<T>() where T : BaseExtra
		{
			List<T> list = (from e in Pokers
				select e.GetExtra<T>()).ToList();
			list.RemoveAll((T e) => (UnityEngine.Object)e == (UnityEngine.Object)null);
			return list;
		}

		public void DestopChanged()
		{
			DelayDo(delegate
			{
				OnDestopChanged.Invoke();
			});
		}

		public bool RemoveCard(BaseCard baseCard)
		{
			if (Pokers.Contains(baseCard))
			{
				if (baseCard is NumberCard)
				{
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectColorCard, baseCard.GetColor());
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectNumberCard, baseCard.GetNumber());
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectSuitCard, baseCard.GetSuit());
					AchievementData.Get().DoAchievement(AchievementType.ClearSuitCard, baseCard.GetSuit());
				}
				else if (baseCard is SnakeCard)
				{
					SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearSanke);
				}
				Pokers.Remove(baseCard);
				if (Uppers.Contains(baseCard))
				{
					Uppers.Remove(baseCard);
				}
				CheckOver();
				return true;
			}
			return false;
		}

		public void ClearCard()
		{
			foreach (BaseCard poker in Pokers)
			{
				poker.ClearCard();
			}
			OnCardChanged.Invoke();
		}

		public void LinkOnce(Vector3 position)
		{
			ScoringSystem.Get().CreateScoreUIByLink(position, OperatingHelper.Get().GetLink(), null);
			PlayStreaksSystem.Get().StreaksOnce();
		}

		public void GiveUp()
		{
			if (!IsGameOver)
			{
				AudioUtility.GetSound().Play("Audios/Level_Faild.wav");
				PlayAdditional.Get().Over();
				Component component = null;
				component = base.gameObject.GetComponent<Sample01Tutorial>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				component = base.gameObject.GetComponent<Sample02Tutorial>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				component = base.gameObject.GetComponent<SampleCardTutorial>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 0f;
				PlayScene.Get().SetCanvasGraphicRaycaster(enabled: false);
				PlayStreaksSystem.Get().DoHideAnimtor();
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				SingletonClass<OnceGameData>.Get().UploadFaild(playSchedule);
				GlobalBoosterUtility.Get().Destory();
				PlayScene.Get().SetButtonLayerVisable(visable: false);
				PlayScene.Get().SetOverButtons(visable: false);
				FindObjectsWithClick.Get().Clear();
				CommonGuideUtility[] componentsInChildren = PlayScene.Get().transform.Find("Canvas").GetComponentsInChildren<CommonGuideUtility>();
				CommonGuideUtility[] array = componentsInChildren;
				foreach (CommonGuideUtility commonGuideUtility in array)
				{
					UnityEngine.Object.Destroy(commonGuideUtility.gameObject);
				}
				leveEndScene.OnStart(new LevelData());
				if (IsPlaying)
				{
					HandCardSystem.Get().StorageHand.HasVaule(delegate(IStorageHandGroup e)
					{
						e.Over();
					});
					TaskQueueUtility taskQueueUtility = new TaskQueueUtility();
					TaskQueueUtility taskQueueUtility2 = taskQueueUtility;
					LeftHandGroup leftHandGroup = HandCardSystem.Get()._LeftHandGroup;
					taskQueueUtility2.AddTask(((HandGroup)leftHandGroup).DestoryWhenFaild);
					TaskQueueUtility taskQueueUtility3 = taskQueueUtility;
					RightHandGroup rightHandGroup = HandCardSystem.Get()._RightHandGroup;
					taskQueueUtility3.AddTask(((HandGroup)rightHandGroup).DestoryWhenFaild);
					taskQueueUtility.AddTask(delegate(UnityAction unityAction)
					{
						PlayDesk playDesk = this;
						SingletonClass<QuestHelper>.Get().DoQuest(QuestType.WinRow, 0);
						Sequence sequence = null;
						Totem.PlayFailed(null);
						for (int j = 0; j < Pokers.Count; j++)
						{
							BaseCard baseCard = Pokers[j];
							baseCard.SetPokerFaceAllShow();
							Vector3 position = baseCard.transform.position;
							position.y += 11.3f;
							baseCard.transform.DORotate(UnityEngine.Random.insideUnitSphere * 360f * 10f, 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
							sequence = DOTween.Sequence();
							sequence.PrependInterval((float)j * 0.02f);
							sequence.Append(baseCard.transform.DOMove(position, 2f));
							sequence.SetEase(Ease.Linear);
						}
						if (unityAction != null)
						{
							if (sequence == null)
							{
								unityAction();
							}
							sequence.OnComplete(delegate
							{
								unityAction();
							});
						}
					});
					taskQueueUtility.Run(null);
					DelayDo(new WaitForSeconds(1.5f), delegate
					{
						_PlayDesk = null;
						SingletonClass<MySceneManager>.Get().Popup<LeveEndScene>(leveEndScene.gameObject, new NavigationEffect());
						leveEndScene.OpenAnimator();
						UnityEngine.Object.Destroy(base.gameObject);
						UnityEngine.Object.Destroy(HandCardSystem.Get().gameObject, 0.5f);
						FindObjectsWithClick.Get().Remove(OnFindObjects);
					});
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Popup<LeveEndScene>(leveEndScene.gameObject, new NavigationEffect());
					leveEndScene.OpenAnimator();
					UnityEngine.Object.Destroy(base.gameObject);
					UnityEngine.Object.Destroy(HandCardSystem.Get().gameObject, 0.5f);
					FindObjectsWithClick.Get().Remove(OnFindObjects);
				}
				IsPlaying = false;
				IsGameOver = true;
			}
		}

		public void AppendBusyTime(float busy)
		{
			if (BusyTime < busy)
			{
				BusyTime = busy;
			}
		}

		public void AppendStepBusyTime(float busy)
		{
			if (BusyStepTime < busy)
			{
				BusyStepTime = busy;
			}
		}

		public void LevelCompleted()
		{
			if (!IsGameOver)
			{
				GlobalBoosterUtility.Get().Destory();
				ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
				IsPlaying = false;
				IsGameOver = true;
				SingletonBehaviour<GlobalConfig>.Get().TimeScale = 0f;
				PlayScene.Get().SetCanvasGraphicRaycaster(enabled: false);
				PlayScene.Get().SetButtonLayerVisable(visable: false);
				PlayScene.Get().SetOverButtons(visable: false);
				FindObjectsWithClick.Get().Clear();
				Transform CanvasTransform = PlayScene.Get().transform.Find("Canvas");
				SingletonBehaviour<SpecialActivityUtility>.Get().CreateSpecialActivityInLevelCompleted(playSchedule, CanvasTransform);
				PlayAdditional.Get().Over();
				Sequence sequence = DOTween.Sequence();
				if (Pokers.Count > 0)
				{
					foreach (BaseCard baseCard in from e in Pokers
						orderby e.Config.zIndex descending
						select e)
					{
						sequence.AppendCallback(delegate
						{
							LinkOnce(baseCard.transform.position);
							RemoveCard(baseCard);
							baseCard.HideAndRemoveAllExtra();
							HandCardSystem.Get().FromDeskToRightHandCard(baseCard);
							if (baseCard is NumberCard)
							{
								SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectColorCard, baseCard.GetColor());
								SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectNumberCard, baseCard.GetNumber());
								SingletonClass<QuestHelper>.Get().DoQuest(QuestType.CollectSuitCard, baseCard.GetSuit());
								AchievementData.Get().DoAchievement(AchievementType.ClearSuitCard, baseCard.GetSuit());
							}
							else if (baseCard is SnakeCard)
							{
								SingletonClass<QuestHelper>.Get().DoQuest(QuestType.ClearSanke);
							}
						});
						sequence.AppendInterval(0.4f);
					}
				}
				sequence.OnComplete(delegate
				{
					GameObject confettiBlastRainbow = SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Particles/ConfettiBlastRainbow");
					List<Vector3> source = new List<Vector3>
					{
						new Vector3(0f, 1.73f, 0f),
						new Vector3(-6.42f, 1.73f, 0f),
						new Vector3(4.33f, 1.73f, 0f),
						new Vector3(0f, -0.21f, 0f),
						new Vector3(5.9f, -0.51f, 0f)
					};
					Sequence s = DOTween.Sequence();
					source = (from p in source
						orderby Guid.NewGuid()
						select p).ToList();
					foreach (Vector3 position in source)
					{
						s.AppendCallback(delegate
						{
							AudioUtility.GetSound().Play("Audios/ConfettiBomb.wav");
							GameObject gameObject = UnityEngine.Object.Instantiate(confettiBlastRainbow);
							gameObject.transform.SetParent(PlayScene.Get().transform, worldPositionStays: false);
							gameObject.transform.position = position + UnityEngine.Random.insideUnitSphere;
						});
						s.AppendInterval(0.5f);
					}
					try
					{
						CharacterUtility component = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "Prefabs/Character")).GetComponent<CharacterUtility>();
						component.transform.SetParent(CanvasTransform, worldPositionStays: false);
						component.transform.SetAsLastSibling();
						component.CreateInVictory();
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.Log(ex.Message);
					}
					HandCardSystem.Get().StorageHand.HasVaule(delegate(IStorageHandGroup e)
					{
						e.Over();
					});
					TaskQueueOnByoneUtility taskQueueOnByoneUtility = new TaskQueueOnByoneUtility();
					taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
					{
						DelayDo(new WaitForSeconds(0.5f), unityAction);
					});
					taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
					{
						leveEndScene.OnStart(new LevelData
						{
							StarComplete = true,
							StarTime = PlayStreaksSystem.Get().GetFirstStar(),
							StarSteaks = PlayStreaksSystem.Get().GetSecondStar()
						});
						unityAction?.Invoke();
					});
					TaskQueueOnByoneUtility taskQueueOnByoneUtility2 = taskQueueOnByoneUtility;
					LeftHandGroup leftHandGroup = HandCardSystem.Get()._LeftHandGroup;
					taskQueueOnByoneUtility2.AddTask(((HandGroup)leftHandGroup).DestoryWhenSuccess);
					taskQueueOnByoneUtility.AddTask(Totem.CompletedAnitamion);
					TaskQueueOnByoneUtility taskQueueOnByoneUtility3 = taskQueueOnByoneUtility;
					RightHandGroup rightHandGroup = HandCardSystem.Get()._RightHandGroup;
					taskQueueOnByoneUtility3.AddTask(((HandGroup)rightHandGroup).DestoryWhenSuccess);
					taskQueueOnByoneUtility.AddTask(PlayStreaksSystem.Get().DoHideAnimtor);
					taskQueueOnByoneUtility.AddTask(ScoringSystem.Get().CreateTimeBonusCoinUI);
					taskQueueOnByoneUtility.AddTask(ScoringSystem.Get().CreateCompletionCoinUI);
					taskQueueOnByoneUtility.AddTask(ScoringSystem.Get().CreateClanBonusCoinUI);
					if (CoinBankData.Get().IsBankRunning())
					{
						taskQueueOnByoneUtility.AddTask(delegate(UnityAction unityAction)
						{
							GameObject g = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(PlayScene).Name, "UI/BankInOver"));
							g.transform.SetParent(CanvasTransform, worldPositionStays: false);
							Text label = g.transform.GetComponentInChildren<Text>();
							UnityAction<int> updateNumber = delegate(int number)
							{
								if (CoinBankData.Get().IsFull(number))
								{
									label.text = LocalizationUtility.Get("Localization_bank.json").GetString("Full");
								}
								else
								{
									label.text = $"{number:N0}";
								}
							};
							int current = CoinBankData.Get().CoinsInBank;
							CoinBankData.Get().PutCoinsInBank(SingletonClass<OnceGameData>.Get().WonTotalCoins());
							int coinsInBank = CoinBankData.Get().CoinsInBank;
							updateNumber(current);
							g.transform.localScale = Vector3.zero;
							Sequence sequence2 = DOTween.Sequence();
							sequence2.Append(g.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.3f));
							sequence2.Append(g.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.15f));
							if (!CoinBankData.Get().IsFull(current))
							{
								AudioUtility.GetSound().PlayLoop("Audios/loop_coin.mp3", 1f);
								sequence2.Join(DOTween.To(() => current, delegate(int vaule)
								{
									updateNumber(vaule);
								}, coinsInBank, 1f));
							}
							sequence2.AppendInterval(0.5f);
							sequence2.Append(g.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.15f));
							sequence2.Append(g.transform.DOScale(Vector3.zero, 0.3f));
							sequence2.OnComplete(delegate
							{
								UnityEngine.Object.Destroy(g);
								if (unityAction != null)
								{
									unityAction();
								}
							});
						});
					}
					taskQueueOnByoneUtility.Run(delegate
					{
						_PlayDesk = null;
						SingletonClass<MySceneManager>.Get().Popup<LeveEndScene>(leveEndScene.gameObject, new NavigationEffect());
						leveEndScene.OpenAnimator();
						UnityEngine.Object.Destroy(base.gameObject);
						UnityEngine.Object.Destroy(HandCardSystem.Get().gameObject, 0.5f);
					});
				});
			}
		}

		private List<BaseCard> CalcTop(List<BaseCard> tops)
		{
			List<BaseCard> list = new List<BaseCard>();
			foreach (BaseCard top in tops)
			{
				if (IsTop(top, tops))
				{
					list.Add(top);
				}
			}
			return list;
		}

		private List<BaseCard> CalcDown(List<BaseCard> all)
		{
			List<BaseCard> list = new List<BaseCard>();
			foreach (BaseCard item in all)
			{
				if (IsDwon(item, all))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private void DelayStart()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			BoosterType unLockBooster = AppearNodeConfig.Get().GetUnLockBooster(playSchedule);
			if (unLockBooster != BoosterType.None && unLockBooster != BoosterType.BurnRope && !PlayData.Get().HasLevelData(playSchedule) && !AuxiliaryData.Get().HasView($"{unLockBooster}_Reward_NO"))
			{
				AuxiliaryData.Get().PutView($"{unLockBooster}_Reward_NO");
				SessionData.Get().PutCommodity(unLockBooster, CommoditySource.Free, 1L);
			}
			FindObjectsWithClick.Get().IsRunning = false;
			PlayScene.Get().SetButtonLayerVisable(visable: true);
			FindObjectsWithClick.Get().Append(OnFindObjects);
			DelayDo(new WaitForSeconds(1f), delegate
			{
				GlobalBoosterUtility.Get().OnStart(PackData.Get().UseBoosters(_LevelConfig.GetBoosters()), delegate
				{
					CalcTopCard();
					OnReadyPlayChanged.Invoke();
					HandCardSystem.Get().CheckRightHandCard(delegate
					{
						FindObjectsWithClick.Get().IsRunning = true;
						IsPlaying = true;
						OnPlayChanged.Invoke();
						if (PlayScene.Get().RocketButton.gameObject.activeSelf && !SampleCardTutorial.IsRunning() && (!AuxiliaryData.Get().RocketOpen || !AuxiliaryData.Get().HasView("_RocketTips")))
						{
							if (AuxiliaryData.Get().RocketOpen)
							{
								SessionData.Get().PutCommodity(BoosterType.Rocket, CommoditySource.Free, 1L);
								JoinPlayHelper.CreateButtonTips(PlayScene.Get().RocketButton.GetComponent<Button>(), JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_rocket_2"), delegate
								{
									PackData.Get().GetCommodity(BoosterType.Rocket).ForUseBack(CommoditySource.Free);
								}, -400f);
							}
							else
							{
								SessionData.Get().PutCommodity(BoosterType.Rocket, CommoditySource.Free, 2L);
								JoinPlayHelper.CreateButtonTips(PlayScene.Get().RocketButton.GetComponent<Button>(), JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("help_rocket"), delegate
								{
									PackData.Get().GetCommodity(BoosterType.Rocket).ForUseBack(CommoditySource.Free);
								}, -400f);
							}
							AuxiliaryData.Get().RocketOpen = true;
							AuxiliaryData.Get().PutView("_RocketTips");
						}
						else if (!AuxiliaryData.Get().RopeTips && playSchedule.Equals(new ScheduleData(0, 0, 10)) && !PlayData.Get().HasLevelData(playSchedule))
						{
							SessionData.Get().PutCommodity(BoosterType.BurnRope, CommoditySource.Free, 2L);
							BoosterItemUI boosterItemUI = PlayScene.Get().BoosterItems.Find((BoosterItemUI e) => e.boosterType == BoosterType.BurnRope);
							if (boosterItemUI == null)
							{
								return;
							}
							JoinPlayHelper.CreateButtonTips(boosterItemUI.GetComponentInChildren<Button>(), JoinEffectDir.Top, LocalizationUtility.Get("Localization_help.json").GetString("NoRope_Desc"), delegate
							{
								PackData.Get().GetCommodity(BoosterType.BurnRope).ForUseBack(CommoditySource.Free);
							}, -250f);
							AuxiliaryData.Get().RopeTips = true;
						}
						MenuUITopLeft.GetMenu().SettingButton.gameObject.AddComponent<EscapeButtonControler>();
					});
				});
			});
		}

		public bool OnFindObjects(Transform[] transforms)
		{
			if (IsAnimionBusy)
			{
				return false;
			}
			if (IsClickEnable)
			{
				BaseCard top = HandCardSystem.Get()._RightHandGroup.GetTop();
				if (top == null)
				{
					return false;
				}
				foreach (Transform transform in transforms)
				{
					BaseCard component = transform.gameObject.GetComponent<BaseCard>();
					if ((bool)component && Uppers.Contains(component))
					{
						if (component.IsBusy())
						{
							return true;
						}
						if (component.IsApplyGloden() && top is GoldenCard)
						{
							component.DestoryByGolden();
							OnClickCardChanged.Invoke(arg0: true);
						}
						else if (component.IsApplyGloden() && component.IsFree() && top is WildCard)
						{
							component.DestoryByMatch(top);
							OnClickCardChanged.Invoke(arg0: true);
						}
						else if (component.IsFree() && component.CalcClickMatch(top))
						{
							component.DestoryByMatch(top);
							OnClickCardChanged.Invoke(arg0: true);
						}
						else
						{
							component.MatchedError();
							AudioUtility.GetSound().Play("Audios/Click_Error.mp3");
						}
						return true;
					}
				}
			}
			return false;
		}

		private void CheckOver()
		{
			if (IsPlaying && Pokers.Count <= 0)
			{
				LevelCompleted();
			}
		}

		private void Update()
		{
			if (BusyStepTime > 0f)
			{
				BusyStepTime -= Time.deltaTime;
			}
			if (BusyTime > 0f)
			{
				BusyTime -= Time.deltaTime;
			}
			if (IsPlaying)
			{
				SingletonClass<OnceGameData>.Get().PlayTime += NightingaleTime.DeltaTime * SingletonBehaviour<GlobalConfig>.Get().TimeScale;
			}
		}

		private void OnDestroy()
		{
			OnPlayChanged.RemoveAllListeners();
			OnDestopChanged.RemoveAllListeners();
			OnClickCardChanged.RemoveAllListeners();
			OnCardChanged.RemoveAllListeners();
			OnReadyPlayChanged.RemoveAllListeners();
		}
	}
}
