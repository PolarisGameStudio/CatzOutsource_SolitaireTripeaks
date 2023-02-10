using System.Collections.Generic;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class TaskQueueUtility
	{
		private List<TaskQueue> compeletedTask = new List<TaskQueue>();

		public void AddTask(TaskQueue doSomething)
		{
			if (!compeletedTask.Contains(doSomething))
			{
				compeletedTask.Add(doSomething);
			}
		}

		public void Run(UnityAction unityAction)
		{
			TaskQueue[] array = compeletedTask.ToArray();
			TaskQueue[] array2 = array;
			foreach (TaskQueue queue in array2)
			{
				queue(delegate
				{
					compeletedTask.Remove(queue);
					if (compeletedTask.Count == 0 && unityAction != null)
					{
						unityAction();
					}
				});
			}
		}
	}
}
