using Nightingale.Utilitys;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DevicePlayLevels : SingletonData<DevicePlayLevels>
	{
		public List<PlayLevel> winlevelTemmps;

		public List<PlayLevel> faildlevelTemmps;

		public DevicePlayLevels()
		{
			winlevelTemmps = new List<PlayLevel>();
			faildlevelTemmps = new List<PlayLevel>();
		}

		public PlayLevel PutNewData(PlayLevel levelTemmp, bool success)
		{
			List<PlayLevel> list = null;
			list = ((!success) ? faildlevelTemmps : winlevelTemmps);
			PlayLevel playLevel = list.Find((PlayLevel e) => e.world == levelTemmp.world && e.chapter == levelTemmp.chapter && e.level == levelTemmp.level);
			if (playLevel == null)
			{
				playLevel = levelTemmp;
				playLevel.Index++;
				list.Add(levelTemmp);
				return playLevel;
			}
			playLevel.Index++;
			playLevel.completionCoins += (levelTemmp.completionCoins - playLevel.completionCoins) / (float)playLevel.Index;
			playLevel.timeCoins += (levelTemmp.timeCoins - playLevel.timeCoins) / (float)playLevel.Index;
			playLevel.streakCoins += (levelTemmp.streakCoins - playLevel.streakCoins) / (float)playLevel.Index;
			playLevel.time += (levelTemmp.time - playLevel.time) / (float)playLevel.Index;
			playLevel.rewardCoins += (levelTemmp.rewardCoins - playLevel.rewardCoins) / (float)playLevel.Index;
			playLevel.UndoCoins += (levelTemmp.UndoCoins - playLevel.UndoCoins) / (float)playLevel.Index;
			playLevel.BuyStepCoins += (levelTemmp.BuyStepCoins - playLevel.BuyStepCoins) / (float)playLevel.Index;
			if (playLevel.LevelBoosters == null)
			{
				playLevel.LevelBoosters = new PlayBoosterGroup
				{
					levelTempBoosters = new List<PlayBooster>()
				};
			}
			IEnumerator enumerator = Enum.GetValues(typeof(BoosterType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BoosterType type = (BoosterType)enumerator.Current;
					PlayBooster playBooster = levelTemmp.LevelBoosters.levelTempBoosters.Find((PlayBooster e) => e.boosterType == type);
					PlayBooster playBooster2 = playLevel.LevelBoosters.levelTempBoosters.Find((PlayBooster e) => e.boosterType == type);
					if (playBooster2 != null || playBooster != null)
					{
						if (playBooster2 == null)
						{
							PlayBooster playBooster3 = new PlayBooster();
							playBooster3.boosterType = type;
							playBooster3.count = 0f;
							playBooster2 = playBooster3;
							playLevel.LevelBoosters.levelTempBoosters.Add(playBooster2);
						}
						else if (playBooster == null)
						{
							PlayBooster playBooster3 = new PlayBooster();
							playBooster3.boosterType = type;
							playBooster3.count = 0f;
							playBooster = playBooster3;
						}
						playBooster2.count += (playBooster.count - playBooster2.count) / (float)playLevel.Index;
						playLevel.LevelBoosters.levelTempBoosters.RemoveAll((PlayBooster e) => e.count == 0f);
					}
				}
				return playLevel;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public int GetPlayCount(ScheduleData scheduleData)
		{
			int num = 0;
			PlayLevel playLevel = winlevelTemmps.Find((PlayLevel e) => e.world == scheduleData.world && e.chapter == scheduleData.chapter && e.level == scheduleData.level);
			if (playLevel != null)
			{
				num += playLevel.Index;
			}
			playLevel = faildlevelTemmps.Find((PlayLevel e) => e.world == scheduleData.world && e.chapter == scheduleData.chapter && e.level == scheduleData.level);
			if (playLevel != null)
			{
				num += playLevel.Index;
			}
			return num;
		}
	}
}
