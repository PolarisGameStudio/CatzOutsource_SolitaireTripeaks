using System;
using UnityEngine.Events;

namespace Nightingale.Utilitys
{
	public class TimeSpanData
	{
		public string Tag
		{
			get;
			private set;
		}

		public DateTime DateTime
		{
			get;
			private set;
		}

		public UnityAction Completed
		{
			get;
			private set;
		}

		public UnityAction<TimeSpan> FrameUpdate
		{
			get;
			private set;
		}

		public TimeSpanData(string Tag, DateTime DateTime, UnityAction Completed, UnityAction<TimeSpan> FrameUpdate)
		{
			this.Tag = Tag;
			this.DateTime = DateTime;
			this.Completed = Completed;
			this.FrameUpdate = FrameUpdate;
		}

		public bool FrameUpdateCallBack()
		{
			TimeSpan arg = DateTime.Subtract(SystemTime.Now);
			if (FrameUpdate != null)
			{
				FrameUpdate(arg);
			}
			if (arg.TotalSeconds < 0.0)
			{
				if (Completed != null)
				{
					Completed();
				}
				return true;
			}
			return false;
		}
	}
}
