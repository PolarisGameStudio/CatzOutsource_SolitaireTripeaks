using System.Collections.Generic;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class TaskQueueOnByoneUtility
	{
		private List<TaskQueue> compeletedTask = new List<TaskQueue>();

		private bool isStop;

		public void AddTask(TaskQueue doSomething)
		{
			if (!compeletedTask.Contains(doSomething))
			{
				compeletedTask.Add(doSomething);
			}
		}

		public void Run(UnityAction unityAction)
		{
			if (compeletedTask.Count == 0)
			{
				if (unityAction != null)
				{
					unityAction();
				}
			}
			else
			{
				TaskQueue queue = compeletedTask[0];
				queue(delegate
				{
					if (!isStop)
					{
						if (compeletedTask.Contains(queue))
						{
							compeletedTask.Remove(queue);
						}
						Run(unityAction);
					}
				});
			}
		}

		public void Stop()
		{
			isStop = true;
		}
	}
}
