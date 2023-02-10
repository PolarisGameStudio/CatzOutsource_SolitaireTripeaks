using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class DailyBonusScene : SoundScene
	{
		[Header("奖励父控件")]
		public Transform RewardsContentTransform;

		[Header("收集按钮")]
		public Button CollectButton;

		[Header("奖励信息")]
		public PurchasingCommodity[] commodities;

		[Header("奖励Items")]
		public DailyBonusItemUI[] RewardItems;

		private void Awake()
		{
			base.IsStay = true;
			int RewardRowDays = AuxiliaryData.Get().RewardRowDays - 1;
			RewardRowDays = Mathf.Max(0, Mathf.Min(RewardRowDays, commodities.Length));
			for (int i = 0; i < RewardItems.Length; i++)
			{
				DailyRewardState state = DailyRewardState.Rewarded;
				if (i == RewardRowDays)
				{
					state = DailyRewardState.Rewarding;
				}
				else if (i > RewardRowDays)
				{
					state = DailyRewardState.Rewardinging;
				}
				RewardItems[i].SetInfo(i, state);
			}
			if (RewardRowDays > 2)
			{
				Transform rewardsContentTransform = RewardsContentTransform;
				Vector3 position = RewardsContentTransform.position;
				float y = position.y;
				Vector3 position2 = RewardsContentTransform.position;
				rewardsContentTransform.position = new Vector3(-710f, y, position2.z);
			}
			CollectButton.onClick.AddListener(delegate
			{
				CollectButton.onClick.RemoveAllListeners();
				AuxiliaryData.Get().RewardRowDay = true;
				List<PurchasingCommodity> list = new List<PurchasingCommodity>();
				if (RewardRowDays >= commodities.Length)
				{
					AuxiliaryData.Get().RewardRowDays = 0;
					list.Add(new PurchasingCommodity
					{
						boosterType = BoosterType.DoubleStar,
						count = 1
					});
					list.Add(new PurchasingCommodity
					{
						boosterType = BoosterType.ExpiredPlay,
						count = 3
					});
					list.Add(new PurchasingCommodity
					{
						boosterType = BoosterType.FreeSlotsPlay,
						count = 1
					});
					list.Add(new PurchasingCommodity
					{
						boosterType = AppearNodeConfig.Get().GetRandomBooster(),
						count = 2
					});
					foreach (PurchasingCommodity item in list)
					{
						SessionData.Get().PutCommodity(item.boosterType, CommoditySource.Free, item.count, changed: false);
					}
					PurchasSuccessPopup.ShowPurchasSuccessPopup(list.ToArray(), delegate
					{
						SingletonClass<MySceneManager>.Get().Close(new JoinEffect(JoinEffectDir.Bottom));
					});
				}
				else
				{
					PurchasingCommodity purchasingCommodity = commodities[RewardRowDays];
					list.Add(new PurchasingCommodity
					{
						boosterType = purchasingCommodity.boosterType,
						count = purchasingCommodity.count
					});
					SingletonBehaviour<EffectUtility>.Get().CreateBoosterType(purchasingCommodity.boosterType, CollectButton.transform.position);
					SessionData.Get().PutCommodity(purchasingCommodity.boosterType, CommoditySource.Free, purchasingCommodity.count);
					SingletonClass<MySceneManager>.Get().Close();
					if (purchasingCommodity.boosterType == BoosterType.UnlimitedPlay)
					{
						MenuUITopLeft.UpdateUnlimitedPlayRemianUI();
					}
				}
			});
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(DailyBonusScene).Name);
		}
	}
}
