using Nightingale.Socials;
using Nightingale.Utilitys;
using System;
using UnityEngine;

namespace SolitaireTripeaks
{
	[Serializable]
	public class MessageContent
	{
		public string title;

		public string description;

		public string details;

		public string code;

		public string synchronizeId;

		public string facebookId;

		public string deviceId;

		public string deviceModel;

		public string deviceOS;

		public string deviceCountry;

		public string gameName;

		public MessageContent Clone()
		{
			facebookId = SingletonBehaviour<FacebookMananger>.Get().UserId;
			deviceId = SystemInfo.deviceUniqueIdentifier;
			deviceModel = SystemInfo.deviceModel;
			deviceOS = SystemInfo.operatingSystem;
			deviceCountry = PlatformUtility.GetCountry();
			gameName = "SolitaireTripeaks";
			return this;
		}
	}
}
