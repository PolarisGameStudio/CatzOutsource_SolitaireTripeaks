using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class TimerUtility : SingletonBehaviour<TimerUtility>
	{
		private List<TimeSpanData> TimeSpans = new List<TimeSpanData>();

		private void Awake()
		{
			InvokeRepeating("RepeatingUpdate", 0.1f, 1f);
		}

		private void RepeatingUpdate()
		{
			TimeSpanData[] array = TimeSpans.ToArray();
			TimeSpanData[] array2 = array;
			foreach (TimeSpanData timeSpanData in array2)
			{
				if (timeSpanData.FrameUpdateCallBack())
				{
					TimeSpans.Remove(timeSpanData);
				}
			}
		}

		public void Append(string tag, TimeSpan timeSpan, UnityAction completedCallBack = null, UnityAction<TimeSpan> frameUpdateCallBack = null)
		{
			Append(tag, SystemTime.Now.Add(timeSpan), completedCallBack, frameUpdateCallBack);
		}

		public void Append(string tag, DateTime timeSpan, UnityAction completedCallBack = null, UnityAction<TimeSpan> frameUpdateCallBack = null)
		{
			Remove(tag);
			TimeSpanData timeSpanData = new TimeSpanData(tag, timeSpan, completedCallBack, frameUpdateCallBack);
			if (!timeSpanData.FrameUpdateCallBack())
			{
				TimeSpans.Add(timeSpanData);
			}
		}

		public void Remove(string tag)
		{
			TimeSpans.RemoveAll((TimeSpanData e) => e.Tag.Equals(tag));
		}
	}
}
