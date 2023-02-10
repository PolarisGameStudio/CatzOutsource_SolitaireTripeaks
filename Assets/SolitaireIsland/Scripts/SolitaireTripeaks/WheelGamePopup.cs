using I2.MiniGames;

using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class WheelGamePopup : BaseScene
	{
		public PrizeWheel PrizeWheel;

		public Transform RewardTransform;

		public Button Btn_Spin;

		public Button Btn_Closed;

		public NumberUI CostLabelUI;

		public NumberUI FreeLabelUI;

		private WheelData[] WheelDatas = new WheelData[8]
		{
			new WheelData
			{
				boosterType = BoosterType.Wild,
				Rewards = 1,
				Probality = 3
			},
			new WheelData
			{
				boosterType = BoosterType.Coins,
				Rewards = 2000,
				Probality = 22
			},
			new WheelData
			{
				boosterType = BoosterType.FreePlay,
				Rewards = 1,
				Probality = 20
			},
			new WheelData
			{
				boosterType = BoosterType.FreeSlotsPlay,
				Rewards = 1,
				Probality = 22
			},
			new WheelData
			{
				boosterType = BoosterType.RandomBooster,
				Rewards = 1,
				Probality = 15
			},
			new WheelData
			{
				boosterType = BoosterType.Coins,
				Rewards = 2000,
				Probality = 22
			},
			new WheelData
			{
				boosterType = BoosterType.FreePlay,
				Rewards = 1,
				Probality = 20
			},
			new WheelData
			{
				boosterType = BoosterType.FreeWheelPlay,
				Rewards = 1,
				Probality = 20
			}
		};

		private AudioSource loopWheelRotate;

		private int doubleRewards = 1;

		private void Start()
		{
			base.IsStay = true;
			WheelRewardElement[] componentsInChildren = RewardTransform.GetComponentsInChildren<WheelRewardElement>();
			for (int i = 0; i < WheelDatas.Length; i++)
			{
				componentsInChildren[i].SetInfo(WheelDatas[i]);
			}
			UpdateCostUI();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(WheelGamePopup).Name);
		}

		private void UpdateCostUI()
		{
			long total = PackData.Get().GetCommodity(BoosterType.FreeWheelPlay).GetTotal();
			CostLabelUI.gameObject.SetActive(total == 0);
			FreeLabelUI.SetNumber("x{0}", total);
		}

		private void DoSpin()
		{
			UnityAction<int> RewardsSpin = delegate(int doubleRewards)
			{
				this.doubleRewards = doubleRewards;
				AuxiliaryData.Get().PlayWheelNumber++;
				PrizeWheel.StartSpinning();
				Btn_Spin.interactable = false;
				Btn_Closed.interactable = false;
				loopWheelRotate = AudioUtility.GetSound().PlayLoop("Audios/wheel_rotate");
				UpdateCostUI();
			};
			if (IronSourceManager.Instance.IsRewardedVideoAvailable() && AuxiliaryData.Get().GetDailyNumber("RewardOnceWheelSpin") < ((!StatisticsData.Get().IsLowPlayer()) ? 1 : 2))
			{
				WatchVideoAdTipScene.ShowWatchAdRewardDoubleBooster(delegate(bool success)
				{
					AuxiliaryData.Get().PutDailyOnce("RewardOnceWheelSpin");
					RewardsSpin((!success) ? 1 : 2);
				});
			}
			else
			{
				RewardsSpin(1);
			}
		}

		public void WheelGameSpinFinished(int index)
		{
			loopWheelRotate.Stop();
			DelayDo(new WaitForSeconds(0.5f), delegate
			{
				BoosterType boosterType = WheelDatas[index].boosterType;
				SessionData.Get().PutCommodity(boosterType, CommoditySource.Wheel, WheelDatas[index].Rewards * doubleRewards, changed: false);
				TipPopupIconNumberScene.ShowPurchasingCommodity(new PurchasingCommodity
				{
					boosterType = boosterType,
					count = WheelDatas[index].Rewards
				}, doubleRewards == 2);
				Btn_Spin.interactable = true;
				Btn_Closed.interactable = true;
				AudioUtility.GetSound().Play("Audios/wheel_win");
				UpdateCostUI();
			});
		}

		public void Btn_DoSpin()
		{
			if (SessionData.Get().UseCommodity(BoosterType.FreeWheelPlay, 1L))
			{
				DoSpin();
			}
			else if (SessionData.Get().UseCommodity(BoosterType.Coins, 2500L, "WheelPlay"))
			{
				DoSpin();
			}
			else
			{
				StoreScene.ShowOutofCoins();
			}
		}

		public void Btn_Close()
		{
			SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
		}
	}
}
