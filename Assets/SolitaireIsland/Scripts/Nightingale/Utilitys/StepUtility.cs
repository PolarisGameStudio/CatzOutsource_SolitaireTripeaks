using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class StepUtility : SingletonBehaviour<StepUtility>
	{
		private List<StepData> StepDatas = new List<StepData>();

		public void Append(string group, float duration, UnityAction unityAction)
		{
			StepDatas.RemoveAll((StepData e) => e.group == group);
			StepDatas.Add(new StepData
			{
				group = group,
				duration = duration,
				unityAction = unityAction
			});
		}

		private void Update()
		{
			StepData[] array = StepDatas.ToArray();
			StepData[] array2 = array;
			foreach (StepData stepData in array2)
			{
				stepData.duration -= Time.unscaledDeltaTime;
				if (stepData.duration <= 0f)
				{
					if (stepData.unityAction != null)
					{
						stepData.unityAction();
					}
					StepDatas.Remove(stepData);
				}
			}
		}
	}
}
