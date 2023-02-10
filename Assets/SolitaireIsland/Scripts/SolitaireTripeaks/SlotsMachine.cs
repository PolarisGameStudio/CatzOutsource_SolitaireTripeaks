
using Nightingale.Localization;
using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class SlotsMachine : BaseScene
	{
		public Sprite[] Sprites;

		public Transform SlotItems;

		public Button Btn_Spin;

		public Button Btn_Close;

		public Button Btn_PayTable;

		public Text WonLabel;

		public JackpotUI JackpotUI;

		public NumberUI CostLabelUI;

		public NumberUI FreeLabelUI;

		public GameObject SpiningGameObject;

		public GameObject SpinedGameObject;

		public SlotsLightUI SlotsLightUI;

		private int TotalRewards;

		private int DoubleRewards;

		private bool isJackpot;

		private float speed = 3000f;

		private float spinDurationTime = 2f;

		private float spinColumbAddTime = 0.4f;

		private bool m_Spinning;

		private SLotGameItemTransform[] items;

		private int[][] ItemProbabilitys = new int[3][]
		{
			new int[6]
			{
				2,
				10,
				8,
				12,
				16,
				25
			},
			new int[6]
			{
				5,
				5,
				8,
				15,
				18,
				22
			},
			new int[6]
			{
				2,
				5,
				5,
				10,
				10,
				25
			}
		};

		private int[][] ItemPrizeConfig = new int[6][]
		{
			new int[3]
			{
				0,
				0,
				1000000
			},
			new int[3]
			{
				0,
				0,
				100000
			},
			new int[3]
			{
				0,
				0,
				50000
			},
			new int[3]
			{
				0,
				0,
				20000
			},
			new int[3]
			{
				0,
				0,
				10000
			},
			new int[3]
			{
				2000,
				3000,
				6000
			}
		};

		private AudioSource LoopAudioSource;

		private GameObject RibbonsGameObject;

		private const float single = 200f;

		public override void OnSceneStateChanged(SceneState state)
		{
			if (state == SceneState.Closing)
			{
				JackpotUI.OnClose();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PackData.Get().GetCommodity(BoosterType.FreeSlotsPlay).OnChanged.RemoveListener(OnFreeSlotsChanged);
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(SlotsMachine).Name);
		}

		private void Start()
		{
			base.IsStay = true;
			items = new SLotGameItemTransform[SlotItems.childCount];
			for (int i = 0; i < items.Length; i++)
			{
				SLotGameItemTransform sLotGameItemTransform = new SLotGameItemTransform();
				sLotGameItemTransform.ItemTransform = SlotItems.GetChild(i);
				SLotGameItemTransform sLotGameItemTransform2 = sLotGameItemTransform;
				sLotGameItemTransform2.SpritesTransform = new Transform[sLotGameItemTransform2.ItemTransform.childCount];
				for (int j = 0; j < sLotGameItemTransform2.SpritesTransform.Length; j++)
				{
					sLotGameItemTransform2.SpritesTransform[j] = sLotGameItemTransform2.ItemTransform.GetChild(j);
					sLotGameItemTransform2.SpritesTransform[j].localPosition = new Vector3(0f, 200f * (float)(j - 1), 0f);
					sLotGameItemTransform2.SpritesTransform[j].GetComponent<Image>().sprite = Sprites[j];
				}
				sLotGameItemTransform2.RotateTime = spinDurationTime + (float)i * spinColumbAddTime;
				items[i] = sLotGameItemTransform2;
			}
			Btn_Spin.interactable = true;
			Btn_Close.interactable = true;
			Btn_PayTable.interactable = true;
			SpiningGameObject.SetActive(value: false);
			SpinedGameObject.SetActive(value: true);
			UpdateCostUI();
			UpdateWonCoinsUI(0);
			PackData.Get().GetCommodity(BoosterType.FreeSlotsPlay).OnChanged.AddListener(OnFreeSlotsChanged);
		}

		private void OnFreeSlotsChanged(CommoditySource source)
		{
			UpdateCostUI();
		}

		private void Update()
		{
			if (!m_Spinning)
			{
				return;
			}
			int num = 0;
			SLotGameItemTransform[] array = items;
			foreach (SLotGameItemTransform sLotGameItemTransform in array)
			{
				if (sLotGameItemTransform.RotateTime > 0f)
				{
					sLotGameItemTransform.RotateTime -= Time.deltaTime;
				}
				else
				{
					sLotGameItemTransform.ReadyToStop = true;
				}
				for (int j = 0; j < sLotGameItemTransform.SpritesTransform.Length; j++)
				{
					if (sLotGameItemTransform.DoingStop)
					{
						for (int k = 0; k < sLotGameItemTransform.SpritesTransform.Length; k++)
						{
							Transform obj = sLotGameItemTransform.SpritesTransform[k];
							Vector3 localPosition = sLotGameItemTransform.SpritesTransform[k].localPosition;
							float x = localPosition.x;
							float y = (float)(k - 1) * 200f;
							Vector3 localPosition2 = sLotGameItemTransform.SpritesTransform[k].localPosition;
							obj.localPosition = new Vector3(x, y, localPosition2.z);
						}
						if (num == items.Length - 1)
						{
							m_Spinning = false;
							SpinFinished();
							break;
						}
						continue;
					}
					sLotGameItemTransform.SpritesTransform[j].localPosition += Vector3.down * speed * Time.deltaTime;
					Vector3 localPosition3 = sLotGameItemTransform.SpritesTransform[j].localPosition;
					if (!(localPosition3.y <= -400f))
					{
						continue;
					}
					Transform obj2 = sLotGameItemTransform.SpritesTransform[j];
					Vector3 localPosition4 = sLotGameItemTransform.SpritesTransform[j].localPosition;
					float x2 = localPosition4.x;
					Vector3 localPosition5 = sLotGameItemTransform.SpritesTransform.OrderByDescending(delegate(Transform p)
					{
						Vector3 localPosition7 = p.localPosition;
						return localPosition7.y;
					}).First().localPosition;
					float y2 = localPosition5.y + 200f;
					Vector3 localPosition6 = sLotGameItemTransform.SpritesTransform[j].localPosition;
					obj2.localPosition = new Vector3(x2, y2, localPosition6.z);
					if (!sLotGameItemTransform.ReadyToStop)
					{
						continue;
					}
					sLotGameItemTransform.DoingStop = true;
					for (int l = 0; l < sLotGameItemTransform.SpritesTransform.Length; l++)
					{
						if (l == 1)
						{
							sLotGameItemTransform.SpritesTransform[l].GetComponent<Image>().sprite = Sprites[sLotGameItemTransform.Reslut];
						}
						else
						{
							sLotGameItemTransform.SpritesTransform[l].GetComponent<Image>().sprite = Sprites[RandomResult(num)];
						}
					}
					ColumnSpinFinished();
				}
				num++;
			}
		}

		private int RandomResult(int column)
		{
			int num = 0;
			int[] array = ItemProbabilitys[column];
			foreach (int num2 in array)
			{
				num += num2;
			}
			int num3 = Random.Range(0, num + 1);
			for (int j = 0; j < ItemProbabilitys[column].Length; j++)
			{
				num3 -= ItemProbabilitys[column][j];
				if (num3 <= 0)
				{
					return j;
				}
			}
			return 0;
		}

		private void UpdateWonCoinsUI(int coins)
		{
			if (DoubleRewards > 1 && coins > 0)
			{
				WonLabel.text = string.Format(LocalizationUtility.Get().GetString("Won: {0} Coins"), $"{coins} x{DoubleRewards}");
			}
			else
			{
				WonLabel.text = string.Format(LocalizationUtility.Get().GetString("Won: {0} Coins"), coins);
			}
		}

		private void SpinFinished()
		{
			LoopAudioSource.Stop();
			DelayDo(new WaitForSeconds(0.5f), delegate
			{
				Btn_Spin.interactable = true;
				Btn_Close.interactable = true;
				Btn_PayTable.interactable = true;
				SpiningGameObject.SetActive(value: false);
				SpinedGameObject.SetActive(value: true);
				UpdateWonCoinsUI(TotalRewards);
				if (TotalRewards > 0)
				{
					AudioUtility.GetSound().Play("Audios/wheel_win.wav");
					SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Wheel, TotalRewards * DoubleRewards);
					SingletonBehaviour<EffectUtility>.Get().CreateCoinEffect(WonLabel.transform.position + new Vector3(0f, 1f, 0f));
					if (isJackpot)
					{
						RibbonsGameObject = Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(typeof(SlotsMachine).Name, "Particles/Ribbons"), base.transform);
						SingletonBehaviour<JackpotUtility>.Get().UploadJackpot();
					}
					SlotsLightUI.WinLight();
				}
				else
				{
					SlotsLightUI.StopLight();
				}
			});
		}

		private void ColumnSpinFinished()
		{
			AudioUtility.GetSound().Play("Audios/slots_spin_stop.wav");
		}

		private void UpdateCostUI()
		{
			long total = PackData.Get().GetCommodity(BoosterType.FreeSlotsPlay).GetTotal();
			CostLabelUI.gameObject.SetActive(total == 0);
			FreeLabelUI.SetNumber("x{0}", total);
		}

		private void DoSpin()
		{
			UnityAction<int> RewardsSpin = delegate(int doubleRewards)
			{
				if (RibbonsGameObject != null)
				{
					UnityEngine.Object.Destroy(RibbonsGameObject);
					RibbonsGameObject = null;
				}
				UpdateWonCoinsUI(0);
				AuxiliaryData.Get().PlaySlotsNumber++;
				SpiningGameObject.SetActive(value: true);
				SpinedGameObject.SetActive(value: false);
				m_Spinning = true;
				if (items != null)
				{
					int num = 0;
					SLotGameItemTransform[] array = items;
					foreach (SLotGameItemTransform sLotGameItemTransform in array)
					{
						sLotGameItemTransform.RotateTime = spinDurationTime + (float)num * spinColumbAddTime;
						sLotGameItemTransform.ReadyToStop = false;
						sLotGameItemTransform.DoingStop = false;
						sLotGameItemTransform.Reslut = 0;
						num++;
					}
				}
				List<int> list = new List<int>();
				int num2 = 0;
				SLotGameItemTransform[] array2 = items;
				foreach (SLotGameItemTransform sLotGameItemTransform2 in array2)
				{
					sLotGameItemTransform2.Reslut = RandomResult(num2);
					list.Add(sLotGameItemTransform2.Reslut);
					num2++;
				}
				TotalRewards = 0;
				DoubleRewards = doubleRewards;
				isJackpot = false;
				int i;
				for (i = 0; i < Sprites.Length; i++)
				{
					int num3 = list.Count((int p) => p == i);
					if (num3 > 0)
					{
						if (i == 0 && num3 >= 3)
						{
							isJackpot = true;
						}
						TotalRewards += ItemPrizeConfig[i][num3 - 1];
					}
				}
				Btn_Spin.interactable = false;
				Btn_Close.interactable = false;
				Btn_PayTable.interactable = false;
				LoopAudioSource = AudioUtility.GetSound().PlayLoop("Audios/wheel_rotate.wav");
				SlotsLightUI.StartLight();
			};
			if (IronSourceManager.Instance.IsRewardedVideoAvailable() && AuxiliaryData.Get().GetDailyNumber("RewardOnceSlotsSpin") < ((!StatisticsData.Get().IsLowPlayer()) ? 1 : 2))
			{
				WatchVideoAdTipScene.ShowWatchAdRewardDoubleBooster(delegate(bool success)
				{
					AuxiliaryData.Get().PutDailyOnce("RewardOnceSlotsSpin");
					RewardsSpin((!success) ? 1 : 2);
				});
			}
			else
			{
				RewardsSpin(1);
			}
		}

		public void Btn_Do_Spin()
		{
			if (SessionData.Get().UseCommodity(BoosterType.FreeSlotsPlay, 1L))
			{
				DoSpin();
			}
			else if (SessionData.Get().UseCommodity(BoosterType.Coins, 2500L, "SlotsPlay"))
			{
				DoSpin();
			}
			else
			{
				StoreScene.ShowOutofCoins();
			}
		}

		public void Btn_Close_Click()
		{
			SingletonClass<MySceneManager>.Get().Close(new ScaleEffect());
		}

		public void Btn_PayTable_Click()
		{
			SingletonClass<MySceneManager>.Get().Popup<PayTableScene>("MiniGames/Slots/PayTableScene");
		}
	}
}
