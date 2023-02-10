using System.Collections;
using UnityEngine.Events;

namespace Nightingale.Tasks
{
	public class NormalTask
	{
		protected TaskDoSomething DoSomething;

		protected TaskCompleted Completed = new TaskCompleted();

		public bool IsRetry
		{
			get;
			protected set;
		}

		public TaskState TaskState
		{
			get;
			protected set;
		}

		public object CacheObject
		{
			get;
			protected set;
		}

		public string TaskId
		{
			get;
			protected set;
		}

		public float Process
		{
			get;
			protected set;
		}

		public bool IsStay
		{
			get;
			protected set;
		}

		public NormalTask()
		{
		}

		public NormalTask(string taskId, TaskDoSomething doSomething)
		{
			TaskId = taskId;
			DoSomething = doSomething;
		}

		public void TryRetry()
		{
			if (TaskState == TaskState.Completed)
			{
				TaskState = TaskState.Ready;
			}
		}

		public NormalTask TryCompleted()
		{
			Completed.Invoke(CacheObject, (CacheObject != null) ? 1 : 0);
			return this;
		}

		public NormalTask RemoveAllListeners()
		{
			Completed.RemoveAllListeners();
			return this;
		}

		public NormalTask RemoveListener(UnityAction<object, float> unityAction)
		{
			if (unityAction == null)
			{
				return this;
			}
			Completed.RemoveListener(unityAction);
			return this;
		}

		public NormalTask AddListener(UnityAction<object, float> completed)
		{
			if (completed != null)
			{
				Completed.AddListener(completed);
			}
			return this;
		}

		public NormalTask SetStay(bool isStay = true)
		{
			IsStay = isStay;
			return this;
		}

		public NormalTask SetRetry(bool isRetry)
		{
			IsRetry = isRetry;
			return this;
		}

		public IEnumerator RunTask(UnityAction unityAction)
		{
			if (TaskState == TaskState.Ready)
			{
				TaskState = TaskState.Running;
				yield return DoSomething(delegate(object cacheObject, float process)
				{
					if (process >= 1f || cacheObject != null)
					{
						TaskState = TaskState.Completed;
						CacheObject = cacheObject;
						Completed.Invoke(cacheObject, process);
						if (unityAction != null)
						{
							unityAction();
						}
					}
					else
					{
						Completed.Invoke(null, process);
					}
				});
			}
		}
	}
}
