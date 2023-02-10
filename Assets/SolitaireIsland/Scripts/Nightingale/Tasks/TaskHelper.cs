using Nightingale.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Tasks
{
	public class TaskHelper : DelayBehaviour
	{
		private static Dictionary<string, TaskHelper> LoadingHelpers = new Dictionary<string, TaskHelper>();

		public readonly List<NormalTask> tasks = new List<NormalTask>();

		public static TaskHelper Get(string path)
		{
			if (!LoadingHelpers.ContainsKey(path))
			{
				GameObject gameObject = new GameObject(path);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				TaskHelper value = gameObject.AddComponent<TaskHelper>();
				LoadingHelpers.Add(path, value);
			}
			return LoadingHelpers[path];
		}

		public static TaskHelper GetDownload()
		{
			return Get("Download");
		}

		public static TaskHelper GetMiniDownload()
		{
			return Get("MiniDownload");
		}

		public static TaskHelper GetLocal()
		{
			return Get("Local");
		}

		public static TaskHelper Create()
		{
			return Get(Guid.NewGuid().ToString());
		}

		public NormalTask AppendTask(NormalTask task)
		{
			NormalTask finder = tasks.Find((NormalTask e) => e.TaskId == task.TaskId);
			if (finder == null)
			{
				finder = task;
				tasks.Add(task);
			}
			DelayDo(delegate
			{
				finder.TryCompleted();
				DelayRun();
			});
			return finder;
		}

		public void RemoveTask(string taskId)
		{
			NormalTask[] array = tasks.ToArray();
			foreach (NormalTask normalTask in array)
			{
				if (normalTask.TaskId == taskId)
				{
					tasks.Remove(normalTask);
					normalTask.RemoveAllListeners();
				}
			}
		}

		public void RemoveTaskListeners(string taskId)
		{
			NormalTask[] array = tasks.ToArray();
			foreach (NormalTask normalTask in array)
			{
				if (normalTask.TaskId == taskId)
				{
					normalTask.RemoveAllListeners();
				}
			}
		}

		public void RemoveTaskListeners(string taskId, UnityAction<object, float> unityAction)
		{
			NormalTask[] array = tasks.ToArray();
			foreach (NormalTask normalTask in array)
			{
				if (normalTask.TaskId == taskId)
				{
					normalTask.RemoveListener(unityAction);
				}
			}
		}

		public NormalTask GetTask(string taskId)
		{
			NormalTask[] array = tasks.ToArray();
			foreach (NormalTask normalTask in array)
			{
				if (normalTask.TaskId == taskId)
				{
					return normalTask;
				}
			}
			return new NormalTask();
		}

		private void DelayRun()
		{
			DelayDo(delegate
			{
				if (tasks.Count != 0)
				{
					(from e in tasks
						where e.TaskState == TaskState.Completed
						select e).ToList().ForEach(delegate(NormalTask task)
					{
						if (task.CacheObject == null)
						{
							task.TryRetry();
						}
						else
						{
							task.RemoveAllListeners();
							tasks.Remove(task);
						}
					});
					if (tasks.Find((NormalTask e) => e.TaskState == TaskState.Running) == null)
					{
						NormalTask normalTask = tasks.Find((NormalTask t) => t.TaskState == TaskState.Ready);
						if (normalTask != null)
						{
							StartCoroutine(normalTask.RunTask(DelayRun));
						}
					}
				}
			});
		}
	}
}
