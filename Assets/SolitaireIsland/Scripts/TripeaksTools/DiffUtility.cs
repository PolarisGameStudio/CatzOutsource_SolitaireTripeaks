using Nightingale;
using Nightingale.Utilitys;
using SolitaireTripeaks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TripeaksTools
{
	public class DiffUtility : MonoBehaviour
	{
		public int CalcCount = 100;

		public int HandNumber;

		[Header("是否是精英关卡")]
		public bool isExpert;

		[Header("自定义关卡")]
		public bool customize;

		[Header("自定义的关卡编号")]
		public List<int> Levels;

		[Header("连续关卡（开始）")]
		public int StartLevel;

		[Header("连续关卡（结束）")]
		public int EndLevel;

		public DiffLoading OnLoading = new DiffLoading();

		private void Start()
		{
			if (isExpert)
			{
				StartCoroutine(DelayExpertStart());
			}
			else
			{
				StartCoroutine(DelayNormalStart());
			}
		}

		private IEnumerator DelayExpertStart()
		{
			string resluts = string.Empty;
			ExpertWorldConfig worldConfig = SingletonClass<ExpertLevelConfigGroup>.Get().GetWorldConfig();
			List<Point> points = worldConfig.GetPoints();
			points = (customize ? (from e in points
				where Levels.Contains(worldConfig.GetLevel(new ScheduleData(-1, e.X, e.Y)) + 1)
				select e).ToList() : points.GetRange(StartLevel - 1, EndLevel - StartLevel + 1));
			int total = points.Count * CalcCount;
			int current = 0;
			foreach (Point item in points)
			{
				ScheduleData scheduleData = new ScheduleData(-1, item.X, item.Y);
				LevelConfig levelConfig = worldConfig.GetLevelConfig(scheduleData);
				LevelRetrunCoinConfig returnConfig = worldConfig.GetLevelRetrunCoinConfig(scheduleData);
				List<TripeaksOnceReslut> OnceResluts = new List<TripeaksOnceReslut>();
				for (int i = 0; i < CalcCount; i++)
				{
					UnityEngine.Debug.Log($"{scheduleData.world + 1}-{scheduleData.chapter + 1}-{scheduleData.level + 1}");
					TripeaksDesk desk = new TripeaksDesk(scheduleData.world + 1, HandNumber, levelConfig, returnConfig, base.transform);
					while (desk.FindCard())
					{
					}
					OnceResluts.Add(desk.GetTripeaksOnceReslut());
					current++;
					OnLoading.Invoke((float)current / (float)total);
					yield return new WaitForSeconds(1E-05f);
				}
				TripeaksOnceReslut[] wons = (from e in OnceResluts
					where e.Success
					select e).ToArray();
				TripeaksOnceReslut[] failds = (from e in OnceResluts
					where !e.Success
					select e).ToArray();
				if (wons.Length > 0)
				{
					string text = string.Format("{15}:{0}-{1}-{2}:总游戏场次:{3}，胜利场次:{4}，胜率:{5}，通关后赢得总金币数：{6}，通关后节点金币数：{7}，通关后剩余手牌金币数：{8}，通关后时间金币数：{9},消牌获得金币数：{10},图腾金币数：{11},最大星星数目：{12},三星概率：{13},平均星星数目：{14}", scheduleData.world + 1, scheduleData.chapter + 1, scheduleData.level + 1, OnceResluts.Count, wons.Length, (float)wons.Length / (float)OnceResluts.Count, Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonSteaksCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.RemainHandCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.TimeCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonPokerCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.TotamCoins), 2), wons.Max((TripeaksOnceReslut e) => e.StarNumber), (float)wons.Count((TripeaksOnceReslut e) => e.StarNumber == 3) / (float)wons.Count(), Math.Round(wons.Average((TripeaksOnceReslut e) => e.StarNumber), 2), SingletonClass<AAOConfig>.Get().GetLevel(scheduleData) + 1);
					if (failds.Count() > 0)
					{
						text += $"\n     失败信息：失败后赢得总金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonCoins), 2)}，失败后节点金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonSteaksCoins), 2)}，失败后剩余手牌金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.RemainHandCoins), 2)}，失败后时间金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.TimeCoins), 2)},消牌获得金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonPokerCoins), 2)}";
					}
					resluts = resluts + text + "\n\n\n";
				}
				else
				{
					string str = $"{scheduleData.world + 1}-{scheduleData.chapter + 1}-{scheduleData.level + 1}：总游戏场次:{OnceResluts.Count}，胜利场次:0，胜率:0";
					resluts = resluts + str + "\n\n\n";
				}
				GC.Collect();
				yield return new WaitForSeconds(1f);
			}
			UnityEngine.Debug.Log("完成分析。。。。");
			FileUtility.SaveFile(Application.dataPath, string.Format("TripeaksTools/{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")), resluts);
			OnLoading.Invoke(1f);
		}

		private IEnumerator DelayNormalStart()
		{
			string resluts = string.Empty;
			List<ScheduleData> scheduleDatas = UniverseConfig.Get().GetAllScheduleDatas();
			scheduleDatas = (customize ? (from e in scheduleDatas
				where Levels.Contains(UniverseConfig.Get().GetLevels(e) + 1)
				select e).ToList() : scheduleDatas.GetRange(StartLevel - 1, EndLevel - StartLevel + 1));
			int total = scheduleDatas.Count * CalcCount;
			int current = 0;
			foreach (ScheduleData item in scheduleDatas)
			{
				ScheduleData scheduleData = item;
				WorldConfig worldConfig = UniverseConfig.Get().worlds[scheduleData.world];
				LevelConfig levelConfig = worldConfig.GetLevelConfig(scheduleData);
				LevelRetrunCoinConfig returnConfig = LevelRetrunCoinConfig.Read(scheduleData);
				List<TripeaksOnceReslut> OnceResluts = new List<TripeaksOnceReslut>();
				for (int i = 0; i < CalcCount; i++)
				{
					UnityEngine.Debug.Log($"{scheduleData.world + 1}-{scheduleData.chapter + 1}-{scheduleData.level + 1}");
					TripeaksDesk desk = new TripeaksDesk(scheduleData.world + 1, HandNumber, levelConfig, returnConfig, base.transform);
					while (desk.FindCard())
					{
					}
					OnceResluts.Add(desk.GetTripeaksOnceReslut());
					current++;
					OnLoading.Invoke((float)current / (float)total);
					yield return new WaitForSeconds(1E-05f);
				}
				TripeaksOnceReslut[] wons = (from e in OnceResluts
					where e.Success
					select e).ToArray();
				TripeaksOnceReslut[] failds = (from e in OnceResluts
					where !e.Success
					select e).ToArray();
				if (wons.Length > 0)
				{
					string text = string.Format("{15}:{0}-{1}-{2}：总游戏场次:{3}，胜利场次:{4}，胜率:{5}，通关后赢得总金币数：{6}，通关后节点金币数：{7}，通关后剩余手牌金币数：{8}，通关后时间金币数：{9},消牌获得金币数：{10},图腾金币数：{11},最大星星数目：{12},三星概率：{13},平均星星数目：{14}", scheduleData.world + 1, scheduleData.chapter + 1, scheduleData.level + 1, OnceResluts.Count, wons.Length, (float)wons.Length / (float)OnceResluts.Count, Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonSteaksCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.RemainHandCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.TimeCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.WonPokerCoins), 2), Math.Round(wons.Average((TripeaksOnceReslut e) => e.TotamCoins), 2), wons.Max((TripeaksOnceReslut e) => e.StarNumber), (float)wons.Count((TripeaksOnceReslut e) => e.StarNumber == 3) / (float)wons.Count(), Math.Round(wons.Average((TripeaksOnceReslut e) => e.StarNumber), 2), UniverseConfig.Get().GetLevels(scheduleData) + 1);
					if (failds.Count() > 0)
					{
						text += $"\n     失败信息：失败后赢得总金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonCoins), 2)}，失败后节点金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonSteaksCoins), 2)}，失败后剩余手牌金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.RemainHandCoins), 2)}，失败后时间金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.TimeCoins), 2)},消牌获得金币数：{Math.Round(failds.Average((TripeaksOnceReslut e) => e.WonPokerCoins), 2)}";
					}
					resluts = resluts + text + "\n\n\n";
				}
				else
				{
					string str = $"{scheduleData.world + 1}-{scheduleData.chapter + 1}-{scheduleData.level + 1}：总游戏场次:{OnceResluts.Count}，胜利场次:{wons.Length}，胜率:0";
					resluts = resluts + str + "\n\n\n";
				}
				GC.Collect();
				yield return new WaitForSeconds(1f);
			}
			UnityEngine.Debug.Log("完成分析。。。。");
			FileUtility.SaveFile(Application.dataPath, string.Format("TripeaksTools/{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")), resluts);
			OnLoading.Invoke(1f);
		}
	}
}
