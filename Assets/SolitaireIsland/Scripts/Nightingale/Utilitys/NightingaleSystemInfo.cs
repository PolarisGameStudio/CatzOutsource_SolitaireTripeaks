using System;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class NightingaleSystemInfo : SingletonClass<NightingaleSystemInfo>
	{
		private string deviceModel = string.Empty;

		private string operatingSystem = string.Empty;

		private string deviceUniqueIdentifier = string.Empty;

		public string DeviceModel
		{
			get
			{
				if (string.IsNullOrEmpty(deviceModel))
				{
					deviceModel = SystemInfo.deviceModel;
					if (string.IsNullOrEmpty(deviceModel))
					{
						deviceModel = "UnKnow";
					}
				}
				return deviceModel;
			}
		}

		public string OperatingSystem
		{
			get
			{
				if (string.IsNullOrEmpty(operatingSystem))
				{
					operatingSystem = SystemInfo.operatingSystem;
					if (string.IsNullOrEmpty(operatingSystem))
					{
						operatingSystem = "UnKnow";
					}
				}
				return operatingSystem;
			}
		}

		public string DeviceUniqueIdentifier
		{
			get
			{
				if (string.IsNullOrEmpty(deviceUniqueIdentifier))
				{
					if (PlayerPrefs.HasKey("_identifier"))
					{
						deviceUniqueIdentifier = PlayerPrefs.GetString("_identifier");
					}
					if (string.IsNullOrEmpty(deviceUniqueIdentifier))
					{
						deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
						if (string.IsNullOrEmpty(deviceUniqueIdentifier))
						{
							deviceUniqueIdentifier = $"{Application.platform.ToString()}_{Guid.NewGuid().ToString()}";
							PlayerPrefs.SetString("_identifier", deviceUniqueIdentifier);
							PlayerPrefs.Save();
						}
					}
				}
				return deviceUniqueIdentifier;
			}
		}

		public NightingaleSystemInfo()
		{
			UnityEngine.Debug.Log(DeviceModel + "====" + OperatingSystem + "====" + DeviceUniqueIdentifier);
		}
	}
}
