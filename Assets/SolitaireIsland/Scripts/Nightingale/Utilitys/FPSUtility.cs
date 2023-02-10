using Nightingale.Debuggers;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class FPSUtility : SingletonBehaviour<FPSUtility>
	{
		private const float fpsMeasurePeriod = 0.5f;

		private int m_FpsAccumulator;

		private float m_FpsNextPeriod;

		private int m_CurrentFps;

		private const string display = "{0} FPS";

		public void Refresh()
		{
		}

		private void Start()
		{
			m_FpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
		}

		private void Update()
		{
			m_FpsAccumulator++;
			if (Time.realtimeSinceStartup > m_FpsNextPeriod)
			{
				m_CurrentFps = (int)((float)m_FpsAccumulator / 0.5f);
				m_FpsAccumulator = 0;
				m_FpsNextPeriod += 0.5f;
				SingletonBehaviour<Debugger>.Get().WriteLine($"{m_CurrentFps} FPS");
			}
		}
	}
}
