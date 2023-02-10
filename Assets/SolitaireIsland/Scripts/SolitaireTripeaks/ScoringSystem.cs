using Nightingale.Extensions;
using Nightingale.Localization;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class ScoringSystem : MonoBehaviour
	{
		private static ScoringSystem scoringSystem;

		private int undoCoins;

		private int buyCoins;

		private int baseCoins;

		private int buyCount;

		public LevelRetrunCoinConfig Config
		{
			get;
			set;
		}

		public int UndoCoins => undoCoins;

		public int BuyStepCoins
		{
			get
			{
				if (AuxiliaryData.Get().FreeHandTotal > 0)
				{
					return 0;
				}
				return buyCoins;
			}
		}

		public static ScoringSystem Get()
		{
			if (scoringSystem == null)
			{
				scoringSystem = UnityEngine.Object.FindObjectOfType<ScoringSystem>();
			}
			return scoringSystem;
		}

		private void Awake()
		{
			scoringSystem = this;
		}

		private void OnDestroy()
		{
			scoringSystem = null;
		}

		public void SetLevel()
		{
			Config = SingletonClass<AAOConfig>.Get().GetLevelRetrunCoinConfig();
			undoCoins = Config.LevelTicketCoins / 2;
			baseCoins = ((!SingletonClass<AAOConfig>.Get().IsLowBuyStepCoins()) ? (Config.LevelTicketCoins * 2) : Config.LevelTicketCoins);
			buyCoins = baseCoins;
		}

		public void UndoOnce()
		{
			undoCoins += Config.LevelTicketCoins / 2;
		}

		public void BuyStep()
		{
			if (AuxiliaryData.Get().FreeHandTotal > 0)
			{
				AuxiliaryData.Get().FreeHandTotal--;
				return;
			}
			buyCount++;
			buyCoins = baseCoins + baseCoins / 2 * buyCount;
		}

		public void CreateCompletionCoinUI(UnityAction unityAction)
		{
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, Config.CompletionCoin);
			SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(Vector3.zero, LocalizationUtility.Get().GetString("COMPLETION"), Config.CompletionCoin.ToString(), unityAction).transform.localScale = new Vector3(2f, 2f, 2f);
			AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
			SingletonClass<OnceGameData>.Get().CompletionCoins += Config.CompletionCoin;
		}

		public void CreateTimeBonusCoinUI(UnityAction unityAction)
		{
			int a = Mathf.FloorToInt((float)Config.TimeRewardReferenceCoin * (1f - SingletonClass<OnceGameData>.Get().PlayTime / (float)Config.LimitTime));
			a = Mathf.Min(a, Config.TimeRewardMaxLimitCoin);
			a = Mathf.Max(a, 0);
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, a);
			SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(Vector3.zero, LocalizationUtility.Get().GetString("TIMEBONUS"), a.ToString(), unityAction).transform.localScale = new Vector3(2f, 2f, 2f);
			AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
			SingletonClass<OnceGameData>.Get().CompletionCoins += a;
		}

		public void CreateClanBonusCoinUI(UnityAction unityAction)
		{
			SingletonClass<OnceGameData>.Get().CompletionMoreCoins = SingletonBehaviour<ClubSystemHelper>.Get().LevelCompletedCoins(SingletonClass<OnceGameData>.Get().StreaksCoins + SingletonClass<OnceGameData>.Get().CompletionCoins);
			if (SingletonClass<OnceGameData>.Get().CompletionMoreCoins > 0)
			{
				SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, SingletonClass<OnceGameData>.Get().CompletionMoreCoins);
				SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(Vector3.zero, LocalizationUtility.Get().GetString("CLANBONUS"), SingletonClass<OnceGameData>.Get().CompletionMoreCoins.ToString(), unityAction).transform.localScale = new Vector3(2f, 2f, 2f);
				AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
			}
			else
			{
				unityAction?.Invoke();
			}
		}

		public void CreateScoreUIByLink(Vector3 position, int link, UnityAction unityAction)
		{
			int number = GetNumber(link - 1);
			SingletonClass<OnceGameData>.Get().StreaksCoins += number;
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, number);
			if (link >= 5)
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(position + new Vector3(0f, 1.5f, 0f), string.Format(LocalizationUtility.Get().GetString("{0} Streaks"), link), number.ToString(), unityAction);
				GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("Particles/PS_effect_coin_1"));
				gameObject.transform.position = position + new Vector3(0f, 1.1f, 0f);
				UnityEngine.Object.Destroy(gameObject, 1f);
			}
			else
			{
				SingletonBehaviour<Effect2DUtility>.Get().CreateScoreUI(position + new Vector3(0f, 1.5f, 0f), number, unityAction);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("Particles/PS_effect_coin_1"));
				gameObject2.transform.position = position + new Vector3(0f, 0.7f, 0f);
				UnityEngine.Object.Destroy(gameObject2, 1f);
			}
		}

		public void CreateScoreUIByHand(Vector3 position, int link, UnityAction unityAction)
		{
			int number = GetNumber(link - 1);
			SingletonClass<OnceGameData>.Get().CompletionCoins += number;
			SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, number);
			SingletonBehaviour<Effect2DUtility>.Get().CreateScoreUI(position, number, unityAction).transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}

		public void CreateScoreUIByStreaksNode(Vector3 position, int link, UnityAction unityAction, bool music = true)
		{
			if (link <= Config.StreaksNodeRewardCoin.Count)
			{
				int coins = Config.StreaksNodeRewardCoin[link - 1];
				SingletonClass<OnceGameData>.Get().StreaksCoins += coins;
				SessionData.Get().PutCommodity(BoosterType.Coins, CommoditySource.Free, coins);
				this.DelayDo(new WaitForSeconds(1f), delegate
				{
					if (music)
					{
						AudioUtility.GetSound().Play("Audios/GetCoins.mp3");
					}
					SingletonBehaviour<Effect2DUtility>.Get().CreateTitleIconLabelUI(position, LocalizationUtility.Get().GetString("Streak Bonus!"), coins.ToString(), unityAction).transform.localScale = new Vector3(2f, 2f, 2f);
					GameObject gameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("Particles/PS_effect_coin_2"));
					gameObject.transform.position = position - new Vector3(0f, 0.7f, 0f);
					UnityEngine.Object.Destroy(gameObject, 1f);
					if (link == 3)
					{
						this.DelayDo(new WaitForSeconds(1f), delegate
						{
							if (music)
							{
								AudioUtility.GetSound().Play("Audios/Booster.mp3");
							}
							SingletonBehaviour<Effect2DUtility>.Get().CreateTextTipsUI(LocalizationUtility.Get().GetString("Streak Star Tips"));
						});
					}
				});
			}
		}

		public void UndoScoreUI(int link)
		{
			if (link > 0)
			{
				int number = GetNumber(link - 1);
				BoosterCommodity commodity = PackData.Get().GetCommodity(BoosterType.Coins);
				commodity.ForceUse(number);
				SingletonClass<OnceGameData>.Get().StreaksCoins -= number;
			}
		}

		public int GetNumber(int link)
		{
			int num = 0;
			if (link < Config.StreaksRewardCoin.Count)
			{
				return Config.StreaksRewardCoin[link];
			}
			int num2 = Config.StreaksFormulaParameters[0];
			int num3 = Config.StreaksFormulaParameters[1];
			int num4 = Config.StreaksFormulaParameters[2];
			int num5 = Config.StreaksFormulaParameters[3];
			return GetNumber(link - 1) + (link - num2 + 1) / 2 * num3 + (link - 1) * num4 + num5;
		}
	}
}
