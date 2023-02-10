using com.F4A.MobileThird;
using Nightingale.Ads;
using Nightingale.Localization;
using Nightingale.Notifications;
using Nightingale.Socials;
using Nightingale.U2D;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class NewsConfig
	{
		public string identifier;

		public bool HidingInNumber;

		public Sprite icon;

		public List<ButtonLabel> buttons;

		public List<PrefabLabel> prefabs;

		public string description;

		public string title;

		public Sprite rewardSprite;

		public int rewardCount;

		public DateTime Order;

		public string Tag;

		public UnityAction<InboxNewsUI, int> RunAction;

		public Sprite GetIcon()
		{
			if (icon == null)
			{
				return SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite((UnityEngine.Random.Range(0, 100) % 2 != 0) ? "inbox_fox" : "inbox_bella");
			}
			return icon;
		}

		public void Run(InboxNewsUI inboxNewsUI, int index)
		{
			if (RunAction != null)
			{
				RunAction(inboxNewsUI, index);
			}
		}

		public static List<NewsConfig> CreateDailyBonusConfig()
		{
			List<NewsConfig> list = new List<NewsConfig>();
			LocalizationUtility localizationUtility = LocalizationUtility.Get("Localization_inbox.json");
			if (SystemTime.IsConnect)
			{
				if (AuxiliaryData.Get().DailyBonusRewards > 0)
				{
					list.Add(new NewsConfig
					{
						title = localizationUtility.GetString("rewards_daily_bonus_title"),
						description = localizationUtility.GetString("rewards_daily_bonus_desc"),
						icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("shop_coin_2"),
						prefabs = new List<PrefabLabel>
						{
							new PrefabLabel(new Vector3(0f, 0f), "UI/Inboxs/DailyReward")
						},
						buttons = new List<ButtonLabel>
						{
							new ButtonLabel(localizationUtility.GetString("btn_collect"))
						},
						RunAction = delegate(InboxNewsUI ui, int dex)
						{
							ui.Config.buttons = null;
							ui.Config.prefabs = new List<PrefabLabel>
							{
								new PrefabLabel(new Vector3(446.3f, 0f), "UI/Inboxs/Collected")
							};
							ui.UpdateUI();
							List<PurchasingCommodity> list2 = new List<PurchasingCommodity>
							{
								new PurchasingCommodity
								{
									boosterType = BoosterType.Coins,
									count = 3000
								},
								new PurchasingCommodity
								{
									boosterType = BoosterType.ExpiredPlay,
									count = 2
								}
							};
							foreach (PurchasingCommodity item in list2)
							{
								SessionData.Get().PutCommodity(item.boosterType, CommoditySource.Free, item.count, changed: false);
							}
							PurchasSuccessPopup.ShowPurchasSuccessPopup(list2.ToArray());
							SingletonClass<InboxUtility>.Get().CollectDailyBonus();
						}
					});
				}
				else
				{
					list.Add(new NewsConfig
					{
						title = localizationUtility.GetString("rewards_daily_bonus_title"),
						description = localizationUtility.GetString("rewards_daily_bonus_desc"),
						icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("shop_coin_2"),
						prefabs = new List<PrefabLabel>
						{
							new PrefabLabel(new Vector3(446.3f, 0f), "UI/Inboxs/Collected")
						},
						HidingInNumber = true
					});
				}
			}
			if (AuxiliaryData.Get().IsDailyActive("InboxWatchVideoAd") && SingletonBehaviour<ThirdPartyAdManager>.Get().IsRewardedVideoAvailable(AuxiliaryData.Get().WatchVideoCount))
			{
				UnityAction<bool> unityAction = delegate
				{
					AuxiliaryData.Get().PutDailyCompleted("InboxWatchVideoAd");
					AuxiliaryData.Get().WatchVideoCount++;
					AuxiliaryData.Get().WatchVideoTotal++;
					SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Video, 1000L, changed: false);
					TipPopupIconNumberScene.ShowVideoRewardCoins(1000);
					SingletonClass<InboxUtility>.Get().UpdateNumber();
				};
				list.Add(new NewsConfig
				{
					title = localizationUtility.GetString("rewards_VIDEO_bonus_title"),
					description = localizationUtility.GetString("rewards_VIDEO_bonus_desc"),
					icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("shop_coin_2"),
					prefabs = new List<PrefabLabel>
					{
						new PrefabLabel(new Vector3(0f, 0f), "UI/Inboxs/VideoReward")
					},
					buttons = new List<ButtonLabel>
					{
						new ButtonLabel(localizationUtility.GetString("btn_NextTime"), "ui/Inboxs/inbox_redButton"),
						new ButtonLabel(localizationUtility.GetString("btn_watch"))
					},
					RunAction = delegate(InboxNewsUI ui, int dex)
					{
						if (dex == 0)
						{
							ui.DestroyUI();
							AuxiliaryData.Get().PutDailyCompleted("InboxWatchVideoAd");
							SingletonClass<InboxUtility>.Get().UpdateNumber();
						}
						else
						{
							ui.ShowWatchVideo();
						}
					}
				});
			}
			list.Add(new NewsConfig
			{
				identifier = "Like_Facebook_Page_Login",
				title = LocalizationUtility.Get("Localization_inbox.json").GetString("news_facebook_page_title"),
				description = LocalizationUtility.Get("Localization_inbox.json").GetString("news_facebook_page_desc"),
				buttons = new List<ButtonLabel>
				{
					new ButtonLabel(LocalizationUtility.Get("Localization_inbox.json").GetString("btn_view"))
				},
				icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_fb"),
				Order = DateTime.MaxValue,
				RunAction = delegate
				{
					SocialManager.Instance.OpenLinkFacebookPage();
				}
			});
			if (!SingletonBehaviour<FacebookMananger>.Get().IsLogin() && AuxiliaryData.Get().IsFacebookReward)
			{
				list.Add(new NewsConfig
				{
					identifier = "Facebook_Login",
					title = LocalizationUtility.Get("Localization_inbox.json").GetString("news_facebook_login_title"),
					description = LocalizationUtility.Get("Localization_inbox.json").GetString("news_facebook_login_desc"),
					buttons = new List<ButtonLabel>
					{
						new ButtonLabel(LocalizationUtility.Get("Localization_inbox.json").GetString("btn_view"))
					},
					icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_fb"),
					Order = DateTime.MaxValue,
					RunAction = delegate(InboxNewsUI inboxNewsUI, int ndx)
					{
						SingletonBehaviour<GlobalConfig>.Get().ShowLoginFacebook(AuxiliaryData.Get().IsFacebookReward, delegate
						{
							if (SingletonBehaviour<FacebookMananger>.Get().IsLogin())
							{
								inboxNewsUI.DestroyUI();
							}
						});
					}
				});
			}
#if ENABLE_LOCAL_NOTIFICATION
			if (!Nightingale.Notifications.LocalNotification.isNotificationsEnabled())
#endif
			{
				list.Add(new NewsConfig
				{
					identifier = "Notification",
					title = LocalizationUtility.Get("Localization_inbox.json").GetString("news_notifications_title"),
					description = LocalizationUtility.Get("Localization_inbox.json").GetString("news_notifications_desc"),
					buttons = new List<ButtonLabel>
					{
						new ButtonLabel(LocalizationUtility.Get("Localization_inbox.json").GetString("btn_enable"))
					},
					icon = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>("Sprites/IconSprite").GetSprite("inbox_fox"),
					Order = DateTime.MaxValue,
					RunAction = delegate(InboxNewsUI inboxNewsUI, int ndx)
					{
#if ENABLE_LOCAL_NOTIFICATION
                        Nightingale.Notifications.LocalNotification.RegisterForNotifications();
#endif
						inboxNewsUI.DestroyUI();
					}
				});
			}
			return list;
		}
	}
}
