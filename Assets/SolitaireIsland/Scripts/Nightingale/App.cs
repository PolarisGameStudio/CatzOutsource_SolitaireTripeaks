using Nightingale.Utilitys;
using System;

namespace Nightingale
{
	public class App : SingletonBehaviour<App>
	{
		private DateTime LeaveTime
		{
			get;
			set;
		}

		private void Awake()
		{
			LeaveTime = DateTime.UtcNow;
			OnAppStart();
		}

		private void OnApplicationQuit()
		{
			OnAppTombstone();
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				LeaveTime = DateTime.UtcNow;
				OnAppTombstone();
			}
			else
			{
				OnAppActive();
				OnLeaveLongTime(DateTime.UtcNow.Subtract(LeaveTime).TotalHours);
			}
		}

		protected virtual void OnAppStart()
		{
		}

		protected virtual void OnAppActive()
		{
		}

		protected virtual void OnAppTombstone()
		{
		}

		public virtual void OnLeaveLongTime(double hours)
		{
		}
	}
}
