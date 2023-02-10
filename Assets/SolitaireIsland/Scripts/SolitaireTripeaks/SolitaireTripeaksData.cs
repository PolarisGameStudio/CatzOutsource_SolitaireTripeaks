using Nightingale.Utilitys;
using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class SolitaireTripeaksData
	{
		public string PlayerId;

		public PlayData Play;

		public QuestData Quest;

		public AchievementData Achievement;

		public PackData Pack;

		public SaleData Promotions;

		public AuxiliaryData Auxiliary;

		public PokerData PokerData;

		public RankCoinData RankCoin;

		public SpecialActivityData SpecialActivity;

		public StatisticsData Statistics;

		public ClubSystemData ClubSystem;

		public CoinBankData CoinBank;

		public long _LastStartTicks;

		private static SolitaireTripeaksData _instance;

		private bool IsInited;

		public static SolitaireTripeaksData Get()
		{
			if (_instance == null)
			{
				_instance = GetData(PlayerPrefs.GetString(typeof(SolitaireTripeaksData).Name));
				if (_instance == null)
				{
					SolitaireTripeaksData solitaireTripeaksData = new SolitaireTripeaksData();
					solitaireTripeaksData.Pack = new PackData();
					solitaireTripeaksData.Play = new PlayData();
					solitaireTripeaksData.Quest = new QuestData();
					solitaireTripeaksData.Achievement = new AchievementData();
					solitaireTripeaksData.Promotions = new SaleData();
					solitaireTripeaksData.Auxiliary = new AuxiliaryData();
					solitaireTripeaksData.PokerData = new PokerData();
					solitaireTripeaksData.RankCoin = new RankCoinData();
					_instance = solitaireTripeaksData;
				}
				//@TODO ENABLE_TEST
#if ENABLE_TEST
				var dataPlay = Resources.Load<TextAsset>("test/Play");
				if(dataPlay != null && !string.IsNullOrEmpty(dataPlay.text))
				{
					_instance.Play = JsonUtility.FromJson<PlayData>(dataPlay.text);
				}

                var packData = Resources.Load<TextAsset>("test/Pack");
                if (packData != null && !string.IsNullOrEmpty(packData.text))
                {
                    _instance.Pack = JsonUtility.FromJson<PackData>(packData.text);
                }
#endif
            }
			return _instance;
		}

		public static void Put(SolitaireTripeaksData instance)
		{
			if (instance != null)
			{
				_instance = instance;
				_instance.FlushData();
			}
		}

		public static SolitaireTripeaksData GetData(string vaule)
		{
			SolitaireTripeaksData result = null;
			if (!string.IsNullOrEmpty(vaule))
			{
				try
				{
					result = JsonUtility.FromJson<SolitaireTripeaksData>(vaule);
					return result;
				}
				catch (Exception)
				{
					try
					{
						result = JsonUtility.FromJson<SolitaireTripeaksData>(CompressUtility.DecompressString(vaule));
						return result;
					}
					catch (Exception)
					{
						return result;
					}
				}
			}
			return result;
		}

		public string ToString(bool compress)
		{
			if (compress)
			{
				return CompressUtility.CompressString(JsonUtility.ToJson(this));
			}
			return JsonUtility.ToJson(this);
		}

		public void FlushData()
		{
			UnityEngine.Debug.Log("本地数据保存成功:" + typeof(SolitaireTripeaksData).Name);
			PlayerPrefs.SetString(typeof(SolitaireTripeaksData).Name, JsonUtility.ToJson(this));
			PlayerPrefs.Save();
		}

		public string GetPlayerId()
		{
			if (string.IsNullOrEmpty(PlayerId))
			{
				PlayerId = Guid.NewGuid().ToString();
			}
			return PlayerId;
		}

		public void OnOpenApplication()
		{
			if (UniverseConfig.Get() != null && !IsInited)
			{
				IsInited = true;
				Achievement.PutConfig(AchievementConfigs.Get().GetConfigs());
				Auxiliary.CheckPlayNumber();
			}
		}

		public void OnUpdate()
		{
			if (!IsInited)
			{
				return;
			}
			Promotions.Update();
			if (SystemTime.IsConnect)
			{
				DateTime dateTime = new DateTime(_LastStartTicks);
				if (dateTime.Date != SystemTime.Now.Date)
				{
					Auxiliary.RestbyNewDay();
					Promotions.RestbyNewDay();
					Quest.RestbyNewDay();
					Auxiliary.DoRowDay(dateTime.Date.AddDays(1.0).Date.Equals(SystemTime.Now.Date));
				}
				Achievement.PutAchievement(AchievementType.RowDay, Auxiliary.RowDays);
				_LastStartTicks = SystemTime.Now.Ticks;
			}
		}

		public void Disable()
		{
			foreach (BoosterCommodity commodity in Pack.commoditys)
			{
				commodity.OnChanged.RemoveAllListeners();
			}
			Achievement.Changed.RemoveAllListeners();
		}
	}
}
