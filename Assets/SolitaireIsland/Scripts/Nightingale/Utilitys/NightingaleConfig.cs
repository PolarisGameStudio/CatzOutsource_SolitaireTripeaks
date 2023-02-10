using Nightingale.Ads;
using System;
using System.Linq;
using UnityEngine;

namespace Nightingale.Utilitys
{
	[Serializable]
	public class NightingaleConfig : ScriptableObject
	{
		[Header("应用程序Id")]
		public string AppId;

		[Header("Facebook App Id")]
		public string FacebookAppId;

		[Header("苹果id")]
		public string AppleAppId;

		[Header("Azure Blob 地址")]
		public string StorageBlobAddress;

		[Header("Azure账户信息")]
		public string StorageAccount;

		[Header("Azure密钥")]
		public string StorageKey;

		[Header("Azure账户信息")]
		public string NewStorageAccount;

		[Header("Azure密钥")]
		public string NewStorageKey;

		[Header("排行榜")]
		public string LeaderBoardApi;

		[Header("消息")]
		public string MessageApi;

		[Header("Facebook同步")]
		public string FacebookApi;

		[Header("俱乐部接口")]
		public string ClubApi;

		[Header("广告id")]
		[SerializeField]
		public ThirdPartyAdData[] ThirdPartyAdDatas;

		private static NightingaleConfig config;

		public static NightingaleConfig Get()
		{
			if (config == null)
			{
				config = SingletonBehaviour<LoaderUtility>.Get().GetAsset<NightingaleConfig>("NightingaleConfig");
			}
			return config;
		}

		public string GetAppIdAndVersion()
		{
			return $"{AppId}_{Application.version}";
		}

		public ThirdPartyAdData GetThirdPartyAdData(ThirdPartyAdType type, ThirdPartyPlatform platform)
		{
			ThirdPartyAdData thirdPartyAdData = ThirdPartyAdDatas.ToList().Find((ThirdPartyAdData e) => e.type == type && e.platform == platform);
			if (thirdPartyAdData != null)
			{
				return thirdPartyAdData;
			}
			return null;
		}
	}
}
