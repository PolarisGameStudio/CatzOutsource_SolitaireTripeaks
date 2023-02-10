using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class RemainLabel : MonoBehaviour
	{
		public Text Label;

		public bool IsShort;

		private DateTime EndTime;

		public UnityEvent OnCompleted = new UnityEvent();

		private void OnDestroy()
		{
			OnCompleted.RemoveAllListeners();
		}

		public void SetRemainTime(DateTime time)
		{
			EndTime = time;
			RepeatingUpdate();
			CancelInvoke("RepeatingUpdate");
			InvokeRepeating("RepeatingUpdate", 1f, 1f);
		}

		public void SetRemainTime(TimeSpan time)
		{
			EndTime = DateTime.Now.Add(time);
			RepeatingUpdate();
			CancelInvoke("RepeatingUpdate");
			InvokeRepeating("RepeatingUpdate", 1f, 1f);
		}

		private void RepeatingUpdate()
		{
			TimeSpan timeSpan = EndTime.Subtract(DateTime.Now);
			Label.text = timeSpan.TOString();
			if (timeSpan.TotalMilliseconds <= 0.0)
			{
				CancelInvoke("RepeatingUpdate");
				OnCompleted.Invoke();
			}
		}
	}
}
