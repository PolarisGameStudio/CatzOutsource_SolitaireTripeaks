using Nightingale.Extensions;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class GlobalBoosterUtility : MonoBehaviour
	{
		private static GlobalBoosterUtility _system;

		private List<GlobalBooster> boosters = new List<GlobalBooster>();

		public bool IsRunning = true;

		public static GlobalBoosterUtility Get()
		{
			if (_system == null)
			{
				_system = UnityEngine.Object.FindObjectOfType<GlobalBoosterUtility>();
			}
			return _system;
		}

		private void Awake()
		{
			_system = this;
		}

		private void OnDestroy()
		{
			_system = null;
		}

		public void OnStart(List<BoosterType> globalBoosterTypes, UnityAction unityAction)
		{
			PlayDesk.Get().OnDestopChanged.AddListener(OpenPoker);
			if (globalBoosterTypes == null)
			{
				if (unityAction == null)
				{
					unityAction();
				}
			}
			else
			{
				AchievementData.Get().DoAchievement(AchievementType.UseBooster, globalBoosterTypes.Count);
				TaskQueueOnByoneUtility taskQueueOnByoneUtility = new TaskQueueOnByoneUtility();
				foreach (BoosterType globalBoosterType in globalBoosterTypes)
				{
					SingletonClass<OnceGameData>.Get().Use(globalBoosterType);
					GameObject gameObject = new GameObject(globalBoosterType.ToString());
					gameObject.transform.SetParent(base.transform, worldPositionStays: false);
					GlobalBooster globalBooster = (GlobalBooster)gameObject.AddComponent(EnumUtility.GetStringType(globalBoosterType));
					globalBooster.boosterType = globalBoosterType;
					boosters.Add(globalBooster);
					TaskQueueOnByoneUtility taskQueueOnByoneUtility2 = taskQueueOnByoneUtility;
					GlobalBooster globalBooster2 = globalBooster;
					taskQueueOnByoneUtility2.AddTask(globalBooster2.OnStart);
					if (globalBoosterType == BoosterType.SnakeEliminate)
					{
						AchievementData.Get().DoAchievement(AchievementType.UseSnake);
					}
				}
				taskQueueOnByoneUtility.Run(unityAction);
			}
		}

		public void PutBooster(BoosterType boosterType)
		{
			PackData.Get().UseBoosters(boosterType);
			SingletonClass<OnceGameData>.Get().Use(boosterType);
			AchievementData.Get().DoAchievement(AchievementType.UseBooster);
			if (boosterType == BoosterType.SnakeEliminate)
			{
				AchievementData.Get().DoAchievement(AchievementType.UseSnake);
			}
			GameObject gameObject = new GameObject(boosterType.ToString());
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			GlobalBooster globalBooster = (GlobalBooster)gameObject.AddComponent(EnumUtility.GetStringType(boosterType));
			boosters.Add(globalBooster);
			globalBooster.boosterType = boosterType;
			globalBooster.OnStart(null);
			OpenPoker();
		}

		public void OpenPoker()
		{
			this.DelayDo(new WaitForEndOfFrame(), delegate
			{
				PlayScene.Get().CalcBoosterEffect();
				if (IsRunning)
				{
					GlobalBooster[] array = boosters.ToArray();
					GlobalBooster[] array2 = array;
					foreach (GlobalBooster globalBooster in array2)
					{
						globalBooster.ClosePoker();
					}
					TaskQueueOnByoneUtility taskQueueOnByoneUtility = new TaskQueueOnByoneUtility();
					GlobalBooster[] array3 = array;
					foreach (GlobalBooster globalBooster2 in array3)
					{
						if (globalBooster2.OpenPoker())
						{
							TaskQueueOnByoneUtility taskQueueOnByoneUtility2 = taskQueueOnByoneUtility;
							GlobalBooster globalBooster3 = globalBooster2;
							taskQueueOnByoneUtility2.AddTask(globalBooster3.ShowEffectEveryOnce);
						}
					}
					taskQueueOnByoneUtility.Run(null);
				}
			});
		}

		public int MultipleStreaks()
		{
			int num = 1;
			foreach (GlobalBooster booster in boosters)
			{
				num *= booster.MultipleStreaks();
			}
			return num;
		}

		public void RemoveBooster(GlobalBooster globalBooster)
		{
			globalBooster.ClosePoker();
			boosters.Remove(globalBooster);
		}

		public void Destory()
		{
			foreach (GlobalBooster booster in boosters)
			{
				booster.ClosePoker();
			}
			boosters.Clear();
		}
	}
}
