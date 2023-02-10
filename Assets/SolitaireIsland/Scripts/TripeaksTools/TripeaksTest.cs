using Nightingale.Extensions;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using SolitaireTripeaks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TripeaksTools
{
	public class TripeaksTest : DelayBehaviour
	{
		public int replayTotal = 1;

		public float timeScale = 10f;

		public bool JoinToPlayScene;

		public bool IsUseStreak = true;

		public bool IsUseRocket = true;

		private int _index;

		private List<TripeaksHard> _hards = new List<TripeaksHard>();

		private void Awake()
		{
			if (JoinToPlayScene)
			{
				SceneManager.LoadScene("SolitaireTripeaks", LoadSceneMode.Additive);
			}
			AuxiliaryData.Get().FreeHandTotal = 0;
			AuxiliaryData.Get().FreeUndoTotal = 0;
			Time.timeScale = timeScale;
			InvokeRepeating("RepeatingUpdate", 0.5f, 0.5f);
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.A) && UnityEngine.Input.GetKey(KeyCode.LeftShift))
			{
				_hards.ForEach(delegate(TripeaksHard h)
				{
					UnityEngine.Debug.LogFormat("{0}-{1}-{2} 成功次数:{3} 失败次数:{4} 胜率为:{5}", h.schedule.world + 1, h.schedule.chapter + 1, h.schedule.level + 1, h.WonCount, h.FaildCount, (float)h.WonCount / (float)(h.WonCount + h.FaildCount));
				});
			}
		}

		private void PutResluts(ScheduleData schedule, bool success)
		{
			TripeaksHard tripeaksHard = _hards.Find((TripeaksHard e) => e.schedule.Equals(schedule));
			if (tripeaksHard == null)
			{
				TripeaksHard tripeaksHard2 = new TripeaksHard();
				tripeaksHard2.schedule = schedule;
				tripeaksHard = tripeaksHard2;
				_hards.Add(tripeaksHard);
			}
			if (success)
			{
				tripeaksHard.WonCount++;
			}
			else
			{
				tripeaksHard.FaildCount++;
			}
		}

		private void RepeatingUpdate()
		{
			if (Object.FindObjectOfType<GlobalLoadingAnimation>() != null)
			{
				return;
			}
			BaseScene downScene = SingletonClass<MySceneManager>.Get().GetDownScene();
			if (downScene == null)
			{
				return;
			}
			BaseScene topScene = SingletonClass<MySceneManager>.Get().GetTopScene();
			if (downScene is IslandScene)
			{
				if (topScene == null)
				{
					return;
				}
				if (topScene is LevelScene)
				{
					LevelScene levelScene = topScene as LevelScene;
					if (levelScene.gameObject.GetComponentInChildren<GraphicRaycaster>().enabled)
					{
						if (IsUseStreak)
						{
							SessionData.Get().PutCommodity(BoosterType.MultipleStreaks, CommoditySource.Free, 1L);
							Object.FindObjectsOfType<BoosterItemUI>().FirstOrDefault((BoosterItemUI e) => e.boosterType == BoosterType.MultipleStreaks).OnSelect();
						}
						levelScene.OnPlayClick();
					}
				}
				else if (topScene == downScene)
				{
					if (Object.FindObjectsOfType<BaseScene>().Length == 1 && downScene.gameObject.GetComponentInChildren<GraphicRaycaster>().enabled)
					{
						(downScene as IslandScene).JumpTo(SingletonClass<AAOConfig>.Get().GetPlaySchedule());
					}
				}
				else
				{
					SingletonClass<MySceneManager>.Get().Close();
				}
			}
			else if (downScene is PlayScene)
			{
				if (topScene is LeveEndScene)
				{
					LeveEndScene leveEndScene = topScene as LeveEndScene;
					if (leveEndScene.gameObject.GetComponentInChildren<GraphicRaycaster>().enabled)
					{
						_index++;
						PutResluts(SingletonClass<AAOConfig>.Get().GetPlaySchedule(), leveEndScene.levelData.StarComplete);
						if (SingletonClass<OnceGameData>.Get().WonTotalCoins() >= 40000)
						{
							UnityEngine.Debug.LogWarningFormat("-------------------------------------------------{0}获得{1}分。", SingletonClass<AAOConfig>.Get().GetLevel(SingletonClass<AAOConfig>.Get().GetPlaySchedule()), SingletonClass<OnceGameData>.Get().WonTotalCoins() / 100);
						}
						if (_index >= replayTotal && leveEndScene.NextButton.gameObject.activeSelf)
						{
							leveEndScene.NextClick();
							_index = 0;
						}
						else
						{
							leveEndScene.ReplayClick();
						}
					}
				}
				else if (topScene is QuestsScene)
				{
					QuestsScene questsScene = topScene as QuestsScene;
					if (questsScene.gameObject.GetComponentInChildren<GraphicRaycaster>().enabled)
					{
						questsScene.OnCloseClick();
						questsScene.SetCanvasGraphicRaycaster(enabled: false);
					}
				}
				else
				{
					if (!(topScene == downScene))
					{
						return;
					}
					PlayDesk playDesk = Object.FindObjectOfType<PlayDesk>();
					if (!(playDesk != null))
					{
						return;
					}
					if (playDesk.IsGameOver || !playDesk.IsPlaying || Object.FindObjectOfType<RocketBooster>() != null || Object.FindObjectOfType<ColorMatchBooster>() != null)
					{
						return;
					}
					if (Object.FindObjectOfType<Seagull>() != null)
					{
						Object.FindObjectOfType<Seagull>().OnClick();
					}
					else
					{
						if (playDesk.IsBusyHandBusy || playDesk.IsAnimionBusy)
						{
							return;
						}
						BaseCard baseCard2 = playDesk.Uppers.Find((BaseCard e) => e is SeagullCard);
						if ((bool)baseCard2)
						{
							playDesk.OnFindObjects(new Transform[1]
							{
								baseCard2.transform
							});
							return;
						}
						if (IsUseRocket)
						{
							PlayScene.Get().AppendProp<RocketBooster>();
							return;
						}
						BaseCard right = HandCardSystem.Get()._RightHandGroup.GetTop();
						List<TripeaksCard> list = TripeaksDesk.ToCards(playDesk.Uppers);
						List<TripeaksCard> list2 = (from baseCard in list
							where baseCard.baseCard.IsFree() && baseCard.baseCard.CalcClickMatch(right)
							select baseCard).ToList();
						if (list2.Count > 0)
						{
							TripeaksCard bestLink = TripeaksDesk.GetBestLink(list2, list);
							if (bestLink != null)
							{
								playDesk.OnFindObjects(new Transform[1]
								{
									bestLink.baseCard.transform
								});
								return;
							}
						}
						else
						{
							Totem[] array = Object.FindObjectsOfType<Totem>();
							foreach (Totem totem in array)
							{
								if (!totem.IsCompleted && totem.OnClick())
								{
									return;
								}
							}
						}
						if (HandCardSystem.Get()._LeftHandGroup.baseCards.Count > 0)
						{
							HandCardSystem.Get().DoNextCardForce();
						}
						else
						{
							playDesk.GiveUp();
						}
					}
				}
			}
			else
			{
				if (!(downScene is GuessGame))
				{
					return;
				}
				if (topScene == downScene)
				{
					MiniCard[] array2 = Object.FindObjectsOfType<MiniCard>();
					foreach (MiniCard miniCard in array2)
					{
						miniCard.GetComponent<Button>().onClick.Invoke();
					}
				}
				else
				{
					PurchasSuccessPopup purchasSuccessPopup = Object.FindObjectOfType<PurchasSuccessPopup>();
					if (purchasSuccessPopup != null && purchasSuccessPopup.gameObject.GetComponentInChildren<GraphicRaycaster>().enabled)
					{
						purchasSuccessPopup.OKButton.onClick.Invoke();
						purchasSuccessPopup.SetCanvasGraphicRaycaster(enabled: false);
					}
				}
			}
		}
	}
}
