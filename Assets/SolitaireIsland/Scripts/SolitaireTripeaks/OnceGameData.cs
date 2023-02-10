using Nightingale.Azure;
using Nightingale.Utilitys;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class OnceGameData : SingletonClass<OnceGameData>
	{
		[Serializable]
		private class FaildData
		{
			public float streak_coins_avg;

			public float reward_coins_avg;

			public float undo_coins_avg;

			public float buy_step_coins_avg;

			public int play_count;

			public string use_booster_count_avg;
		}

		[Serializable]
		private class SuccessData
		{
			public float completion_coins_avg;

			public float time_reward_coins_avg;

			public float streak_coins_avg;

			public float reward_coins_avg;

			public float undo_coins_avg;

			public float buy_step_coins_avg;

			public float complete_time_avg;

			public int play_count;

			public string use_booster_count_avg;
		}

		public int StreaksCoins;

		public int CompletionCoins;

		public int CompletionMoreCoins;

		public int StarCount;

		public float PlayTime;

		public int BuyStepCoins;

		public int UndoCoins;

		public List<PlayBooster> LevelTempBoosters;

		private const string TripeaksLevelWinStatics = "TripeaksLevelWinStatics";

		private const string TripeaksLevelLoseStatics = "TripeaksLevelLoseStatics";

		public OnceGameData()
		{
			AzureTableStorage.GetOld().CreateTable("TripeaksLevelWinStatics");
			AzureTableStorage.GetOld().CreateTable("TripeaksLevelLoseStatics");
		}

		public bool IsTutorial()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (playSchedule.Equals(new ScheduleData(0, 0, 0)) && !PlayData.Get().HasLevelData(playSchedule))
			{
				return true;
			}
			if (playSchedule.Equals(new ScheduleData(0, 0, 1)) && !PlayData.Get().HasLevelData(playSchedule))
			{
				return true;
			}
			return false;
		}

		public bool IsRandom()
		{
			ScheduleData playSchedule = SingletonClass<AAOConfig>.Get().GetPlaySchedule();
			if (IsTutorial())
			{
				return true;
			}
			if (AppearNodeConfig.Get().HasCardType(playSchedule))
			{
				return true;
			}
			if (AppearNodeConfig.Get().HasExtraType(playSchedule))
			{
				return true;
			}
			return false;
		}

		public void Rest()
		{
			BuyStepCoins = 0;
			UndoCoins = 0;
			StreaksCoins = 0;
			CompletionCoins = 0;
			CompletionMoreCoins = 0;
			StarCount = 0;
			PlayTime = 0f;
			LevelTempBoosters = new List<PlayBooster>();
		}

		public int WonCoins()
		{
			return StreaksCoins + CompletionCoins;
		}

		public int WonTotalCoins()
		{
			return StreaksCoins + CompletionCoins + CompletionMoreCoins;
		}

		public void Use(BoosterType boosterType)
		{
			PlayBooster playBooster = LevelTempBoosters.Find((PlayBooster e) => e.boosterType == boosterType);
			if (playBooster == null)
			{
				PlayBooster playBooster2 = new PlayBooster();
				playBooster2.boosterType = boosterType;
				playBooster = playBooster2;
				LevelTempBoosters.Add(playBooster);
			}
			playBooster.count += 1f;
		}

		public void UploadFaild(ScheduleData scheduleData)
		{
			PlayLevel playLevel = SingletonData<DevicePlayLevels>.Get().PutNewData(new PlayLevel
			{
				world = scheduleData.world,
				chapter = scheduleData.chapter,
				level = scheduleData.level,
				completionCoins = CompletionCoins,
				streakCoins = StreaksCoins,
				time = PlayTime,
				LevelBoosters = new PlayBoosterGroup
				{
					levelTempBoosters = LevelTempBoosters
				},
				UndoCoins = UndoCoins,
				BuyStepCoins = BuyStepCoins
			}, success: false);
			FaildData faildData = new FaildData();
			faildData.streak_coins_avg = playLevel.streakCoins;
			faildData.reward_coins_avg = playLevel.rewardCoins;
			faildData.undo_coins_avg = playLevel.UndoCoins;
			faildData.buy_step_coins_avg = playLevel.BuyStepCoins;
			faildData.play_count = playLevel.Index;
			faildData.use_booster_count_avg = JsonUtility.ToJson(playLevel.LevelBoosters);
			FaildData obj = faildData;
			if (SingletonData<DevicePlayLevels>.Get().GetPlayCount(scheduleData) >= 10)
			{
				AzureTableStorage.GetOld().InsertOrMergeEntity("TripeaksLevelLoseStatics", SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, $"{playLevel.world}-{playLevel.chapter}-{playLevel.level}", JsonUtility.ToJson(obj), delegate(DownloadHandler download)
				{
					UnityEngine.Debug.Log((!download.isDone) ? "上传关卡数据失败。" : "上传关卡数据成功。");
				});
			}
		}

		public void UploadSuccess(ScheduleData scheduleData)
		{
			PlayLevel playLevel = SingletonData<DevicePlayLevels>.Get().PutNewData(new PlayLevel
			{
				world = scheduleData.world,
				chapter = scheduleData.chapter,
				level = scheduleData.level,
				completionCoins = CompletionCoins,
				streakCoins = StreaksCoins,
				time = PlayTime,
				LevelBoosters = new PlayBoosterGroup
				{
					levelTempBoosters = LevelTempBoosters
				},
				UndoCoins = UndoCoins,
				BuyStepCoins = BuyStepCoins
			}, success: true);
			SuccessData successData = new SuccessData();
			successData.completion_coins_avg = playLevel.completionCoins;
			successData.time_reward_coins_avg = playLevel.timeCoins;
			successData.streak_coins_avg = playLevel.streakCoins;
			successData.reward_coins_avg = playLevel.rewardCoins;
			successData.undo_coins_avg = playLevel.UndoCoins;
			successData.buy_step_coins_avg = playLevel.BuyStepCoins;
			successData.complete_time_avg = playLevel.time;
			successData.play_count = playLevel.Index;
			successData.use_booster_count_avg = JsonUtility.ToJson(playLevel.LevelBoosters);
			SuccessData obj = successData;
			if (SingletonData<DevicePlayLevels>.Get().GetPlayCount(scheduleData) >= 10)
			{
				AzureTableStorage.GetOld().InsertOrMergeEntity("TripeaksLevelWinStatics", SingletonClass<NightingaleSystemInfo>.Get().DeviceUniqueIdentifier, $"{playLevel.world}-{playLevel.chapter}-{playLevel.level}", JsonUtility.ToJson(obj), delegate(DownloadHandler download)
				{
					UnityEngine.Debug.Log((!download.isDone) ? "上传关卡数据失败。" : "上传关卡数据成功。");
				});
			}
		}
	}
}
