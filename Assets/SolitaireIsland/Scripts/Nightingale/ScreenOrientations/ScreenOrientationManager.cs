using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;

namespace Nightingale.ScreenOrientations
{
	public class ScreenOrientationManager : SingletonBehaviour<ScreenOrientationManager>
	{
		private DeviceOrientation deviceOrientation;

		public ScreenOrientationEvent orientationChange = new ScreenOrientationEvent();

		public bool IsLockScreen
		{
			get;
			private set;
		}

		public NightingaleOrientation nightingaleOrientation
		{
			get;
			private set;
		}

		private void Awake()
		{
			nightingaleOrientation = NightingaleOrientation.Portrait;
		}

		public void LockScreen(bool lockScreen)
		{
			IsLockScreen = lockScreen;
		}

		private void Update()
		{
			if (!IsLockScreen && SingletonClass<MySceneManager>.Get().Count() <= 1 && Input.deviceOrientation != deviceOrientation)
			{
				deviceOrientation = Input.deviceOrientation;
				NightingaleOrientation nightingaleOrientation = this.nightingaleOrientation;
				switch (deviceOrientation)
				{
				case DeviceOrientation.Portrait:
					Screen.orientation = ScreenOrientation.Portrait;
					nightingaleOrientation = NightingaleOrientation.Portrait;
					break;
				case DeviceOrientation.LandscapeLeft:
					Screen.orientation = ScreenOrientation.LandscapeLeft;
					nightingaleOrientation = NightingaleOrientation.Landscape;
					break;
				case DeviceOrientation.LandscapeRight:
					Screen.orientation = ScreenOrientation.LandscapeRight;
					nightingaleOrientation = NightingaleOrientation.Landscape;
					break;
				case DeviceOrientation.PortraitUpsideDown:
					Screen.orientation = ScreenOrientation.PortraitUpsideDown;
					nightingaleOrientation = NightingaleOrientation.Portrait;
					break;
				}
				if (this.nightingaleOrientation != nightingaleOrientation)
				{
					this.nightingaleOrientation = nightingaleOrientation;
					orientationChange.Invoke(this.nightingaleOrientation);
				}
			}
		}
	}
}
