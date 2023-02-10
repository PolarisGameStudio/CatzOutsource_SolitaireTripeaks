using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class AppActiveUtility : SingletonBehaviour<AppActiveUtility>
	{
		private class UnityActionData
		{
			public UnityAction<float> unityAction;

			public DateTime dateTime;
		}

		private List<UnityActionData> actions = new List<UnityActionData>();

		public void Append(UnityAction<float> handler)
		{
			actions.Add(new UnityActionData
			{
				unityAction = handler,
				dateTime = DateTime.Now
			});
		}

		public void Clear()
		{
			actions.Clear();
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				foreach (UnityActionData action in actions)
				{
					action.unityAction((float)DateTime.Now.Subtract(action.dateTime).TotalSeconds);
				}
				actions.Clear();
			}
		}
	}
}
