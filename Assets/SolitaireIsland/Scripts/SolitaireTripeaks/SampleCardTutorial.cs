using Nightingale.Extensions;
using Nightingale.Inputs;
using Nightingale.Localization;
using Nightingale.Utilitys;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SampleCardTutorial : MonoBehaviour
	{
		private static ScheduleData TotemScheduleData = new ScheduleData(1, 5, 0);

		private static ScheduleData ScarecrowScheduleData = new ScheduleData(1, 9, 0);

		private bool isDestory;

		public static bool IsRunning()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (playSchedule.Equals(TotemScheduleData) || playSchedule.Equals(ScarecrowScheduleData) || AppearNodeConfig.Get().HasCardType(playSchedule) || AppearNodeConfig.Get().HasExtraType(playSchedule))
			{
				return true;
			}
			return false;
		}

		private void Awake()
		{
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey("SampleCardTutorial");
			PlayDesk.Get().OnReadyPlayChanged.AddListener(OnReadyPlayChanged);
		}

		private void OnDestroy()
		{
			TipPokerSystem.Get().HideTip();
		}

		private void OnReadyPlayChanged()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (playSchedule.Equals(ScarecrowScheduleData))
			{
				PlayDesk.Get().OnReadyPlayChanged.RemoveListener(OnReadyPlayChanged);
				BaseCard finder = PlayDesk.Get().Pokers.Find((BaseCard e) => e is ScarecrowCard);
				if (finder != null)
				{
					finder.FrontPoker.UpdateColor(white: true);
					finder.FrontPoker.UpdateLayer("TopLayer");
					finder.BackgroundPoker.UpdateLayer("TopLayer");
					SpriteRenderer componentInChildren = TipPokerSystem.Get().OpenTargetTip(finder.transform).GetComponentInChildren<SpriteRenderer>();
					componentInChildren.sortingLayerName = "TopLayer";
					GameObject ButtonObject = new GameObject("Button", typeof(Image), typeof(Button));
					ButtonObject.transform.SetParent(PlayScene.Get().transform.Find("Canvas"), worldPositionStays: false);
					RectTransform component = ButtonObject.GetComponent<RectTransform>();
					ButtonObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);
					component.anchorMin = Vector2.zero;
					component.anchorMax = new Vector2(1f, 1f);
					CommonGuideUtility common = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
					RectTransform obj = common.transform as RectTransform;
					Vector2 anchoredPosition = (common.transform as RectTransform).anchoredPosition;
					obj.anchoredPosition = new Vector2(anchoredPosition.x, -210f);
					common.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("ScarecrowCardHelper"), 2f, delegate
					{
						ButtonObject.GetComponent<Button>().onClick.AddListener(delegate
						{
							finder.FrontPoker.UpdateColor(white: false);
							finder.FrontPoker.UpdateLayer();
							finder.BackgroundPoker.UpdateLayer();
							TipPokerSystem.Get().CloseTargetTip();
							common.CloseGuide();
							UnityEngine.Object.Destroy(ButtonObject);
							SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
							StorageHelper();
						});
					});
				}
			}
			else if (playSchedule.Equals(TotemScheduleData))
			{
				Totem[] arrays = UnityEngine.Object.FindObjectsOfType<Totem>();
				if (arrays.Length > 0)
				{
					PlayDesk.Get().OnReadyPlayChanged.RemoveListener(OnReadyPlayChanged);
					PlayDesk.Get().IsClickEnable = false;
					HandCardSystem.Get().IsClickEnable = false;
					HandCardSystem.Get().IsStorageHand = false;
					for (int i = 0; i < arrays.Length; i++)
					{
						HandCardSystem.Get().AppendLeftCard(arrays[i].ColorType * 13 + 3);
					}
					CommonGuideUtility common2 = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
					RectTransform obj2 = common2.transform as RectTransform;
					Vector2 anchoredPosition2 = (common2.transform as RectTransform).anchoredPosition;
					obj2.anchoredPosition = new Vector2(anchoredPosition2.x, -210f);
					common2.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("TotemHelper"), 1f, null);
					UnityAction OnPlayChanged = null;
					OnPlayChanged = delegate
					{
						PlayDesk.Get().OnPlayChanged.RemoveListener(OnPlayChanged);
						UnityAction unityAction = delegate
						{
							BaseCard _Top = HandCardSystem.Get()._RightHandGroup.GetTop();
							if (_Top != null)
							{
								Totem totem = arrays.ToList().Find((Totem e) => e.ColorType == _Top.GetSuit() && !e.IsCompleted);
								if (totem != null)
								{
									TipPokerSystem.Get().OpenTargetTip(totem.transform, 0.5f);
								}
								else
								{
									PlayDesk.Get().IsClickEnable = true;
									HandCardSystem.Get().IsClickEnable = true;
									HandCardSystem.Get().IsStorageHand = true;
									PlayDesk.Get().OnCardChanged.RemoveAllListeners();
									TipPokerSystem.Get().CloseTargetTip();
									common2.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("TotemCompleted"), 2f, delegate
									{
										StorageHelper();
										common2.CloseGuide();
										SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
									});
								}
							}
						};
						PlayDesk.Get().OnCardChanged.AddListener(unityAction);
						unityAction();
					};
					PlayDesk.Get().OnPlayChanged.AddListener(OnPlayChanged);
				}
			}
			else if (AppearNodeConfig.Get().HasCardType(playSchedule))
			{
				CardTypeNodeConfig config = AppearNodeConfig.Get().GetCardType(playSchedule);
				BaseCard finder2 = PlayDesk.Get().Uppers.Find((BaseCard e) => e.Config.CardType == config.cardType);
				if (finder2 != null)
				{
					PlayDesk.Get().OnReadyPlayChanged.RemoveListener(OnReadyPlayChanged);
					HandCardSystem.Get().IsClickEnable = false;
					HandCardSystem.Get().IsStorageHand = false;
					TipPokerSystem.Get().OpenTargetTip(finder2.transform);
					GlobalBoosterUtility.Get().IsRunning = false;
					CommonGuideUtility common3 = null;
					if (finder2.Config.CardType == CardType.Snake || finder2.Config.CardType == CardType.Fork)
					{
						common3 = CommonGuideUtility.CreateCommonGuideUtilitySnake(PlayScene.Get().transform);
					}
					else
					{
						common3 = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
					}
					RectTransform obj3 = common3.transform as RectTransform;
					Vector2 anchoredPosition3 = (common3.transform as RectTransform).anchoredPosition;
					obj3.anchoredPosition = new Vector2(anchoredPosition3.x, -210f);
					common3.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString(config.guide), 1f, null);
					PlayDesk.Get().OnCardChanged.AddListener(delegate
					{
						if (!PlayDesk.Get().Uppers.Contains(finder2))
						{
							HandCardSystem.Get().IsClickEnable = true;
							HandCardSystem.Get().IsStorageHand = true;
							common3.CloseGuide();
							PlayDesk.Get().OnCardChanged.RemoveAllListeners();
							TipPokerSystem.Get().CloseTargetTip();
							GlobalBoosterUtility.Get().IsRunning = true;
							GlobalBoosterUtility.Get().OpenPoker();
							SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
							StorageHelper();
						}
					});
				}
			}
			else if (AppearNodeConfig.Get().HasExtraType(playSchedule))
			{
				PlayDesk.Get().OnReadyPlayChanged.RemoveListener(OnReadyPlayChanged);
				ExtraTypeNodeConfig config2 = AppearNodeConfig.Get().GetExtraConfig(playSchedule);
				BaseCard finder3 = PlayDesk.Get().Uppers.Find((BaseCard e) => e.Config.HasExtraType(new ExtraConfig
				{
					ClassType = config2.extraType,
					Index = config2.Index
				}));
				if (finder3 != null)
				{
					int num = finder3.Config.Index + 14;
					num = (num - 1) % 52 + 1;
					HandCardSystem.Get().AppendLeftCard(num);
					if (config2.extraType == ExtraType.Rope || config2.extraType == ExtraType.Vine)
					{
						HandCardSystem.Get().AppendLeftCard(num);
					}
					HandCardSystem.Get().IsClickEnable = false;
					HandCardSystem.Get().IsStorageHand = false;
					TipPokerSystem.Get().OpenTargetTip(finder3.transform);
					GlobalBoosterUtility.Get().IsRunning = false;
					CommonGuideUtility common4 = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
					RectTransform obj4 = common4.transform as RectTransform;
					Vector2 anchoredPosition4 = (common4.transform as RectTransform).anchoredPosition;
					obj4.anchoredPosition = new Vector2(anchoredPosition4.x, -210f);
					common4.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString(config2.guide), 1f, null);
					PlayDesk.Get().OnCardChanged.AddListener(delegate
					{
						if (config2.extraType == ExtraType.Color)
						{
							finder3 = PlayDesk.Get().Uppers.Find((BaseCard e) => e.Config.HasExtraType(new ExtraConfig
							{
								ClassType = config2.extraType,
								Index = config2.Index
							}));
							if (finder3 != null)
							{
								SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
								common4.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("SameColor_Tips2"));
								TipPokerSystem.Get().OpenTargetTip(finder3.transform);
								return;
							}
						}
						if (!PlayDesk.Get().Uppers.Contains(finder3))
						{
							SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
							HandCardSystem.Get().IsClickEnable = true;
							HandCardSystem.Get().IsStorageHand = true;
							common4.CloseGuide();
							PlayDesk.Get().OnCardChanged.RemoveAllListeners();
							TipPokerSystem.Get().CloseTargetTip();
							GlobalBoosterUtility.Get().IsRunning = true;
							GlobalBoosterUtility.Get().OpenPoker();
							StorageHelper();
						}
					});
				}
			}
			else
			{
				StorageHelper();
			}
		}

		private void StorageHelper()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (playSchedule.world == 1 && playSchedule.chapter == 0 && playSchedule.level == 0 && UnityEngine.Object.FindObjectOfType<StorageHandGroup>() != null)
			{
				HandCardSystem.Get().IsClickEnable = false;
				HandCardSystem.Get().IsStorageHand = false;
				GlobalBoosterUtility.Get().IsRunning = false;
				PlayDesk.Get().IsClickEnable = false;
				CommonGuideUtility common = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
				RectTransform obj = common.transform as RectTransform;
				Vector2 anchoredPosition = (common.transform as RectTransform).anchoredPosition;
				obj.anchoredPosition = new Vector2(anchoredPosition.x, -210f);
				TipPokerSystem.Get().OpenTargetTip(HandCardSystem.Get().StorageHand.GetTransform());
				common.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("Storage_helper"), 1f, delegate
				{
					HandCardSystem.Get().IsStorageHand = true;
					HandCardSystem.Get().StorageHand.HasVaule(delegate(IStorageHandGroup e)
					{
						e.Changed().AddListener(delegate
						{
							DestoryStorageHelper(common);
						});
					});
				});
			}
			else if (playSchedule.world == 2 && playSchedule.chapter == 0 && playSchedule.level == 0 && UnityEngine.Object.FindObjectOfType<NumberUpStorageHandGroup>() != null)
			{
				HandCardSystem.Get().IsClickEnable = false;
				HandCardSystem.Get().IsStorageHand = false;
				GlobalBoosterUtility.Get().IsRunning = false;
				PlayDesk.Get().IsClickEnable = false;
				CommonGuideUtility common2 = CommonGuideUtility.CreateCommonGuideUtility(PlayScene.Get().transform);
				RectTransform obj2 = common2.transform as RectTransform;
				Vector2 anchoredPosition2 = (common2.transform as RectTransform).anchoredPosition;
				obj2.anchoredPosition = new Vector2(anchoredPosition2.x, -210f);
				TipPokerSystem.Get().OpenTargetTip(HandCardSystem.Get().StorageHand.GetTransform());
				common2.CreateGuide(LocalizationUtility.Get("Localization_help.json").GetString("Number_up_Storage_helper_1"), 1f, delegate
				{
					HandCardSystem.Get().IsStorageHand = true;
					HandCardSystem.Get().StorageHand.HasVaule(delegate(IStorageHandGroup e)
					{
						e.Changed().AddListener(delegate
						{
							e.Changed().RemoveAllListeners();
							HandCardSystem.Get().IsClickEnable = true;
							HandCardSystem.Get().IsStorageHand = false;
							TipPokerSystem.Get().CloseTargetTip();
							common2.ChangeGuide(LocalizationUtility.Get("Localization_help.json").GetString("Number_up_Storage_helper_2"), 1f, delegate
							{
								TipPokerSystem.Get().OpenTargetTip(HandCardSystem.Get()._LeftHandGroup.GetTop().transform);
								PlayDesk.Get().OnDestopChanged.AddListener(delegate
								{
									PlayDesk.Get().OnDestopChanged.RemoveAllListeners();
									DestoryStorageHelper(common2);
								});
							});
						});
					});
				});
			}
			else
			{
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
				UnityEngine.Object.Destroy(this);
			}
		}

		private void DestoryStorageHelper(CommonGuideUtility common)
		{
			if (!isDestory)
			{
				isDestory = true;
				HandCardSystem.Get().IsClickEnable = true;
				HandCardSystem.Get().IsStorageHand = true;
				GlobalBoosterUtility.Get().IsRunning = true;
				PlayDesk.Get().IsClickEnable = true;
				TipPokerSystem.Get().CloseTargetTip();
				UnityEngine.Object.Destroy(this);
				common.CloseGuide();
				SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("SampleCardTutorial");
			}
		}
	}
}
