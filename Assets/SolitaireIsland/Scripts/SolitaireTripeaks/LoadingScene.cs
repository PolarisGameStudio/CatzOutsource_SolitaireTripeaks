using com.F4A.MobileThird;
using Nightingale.Ads;
using Nightingale.ScenesManager;
using Nightingale.Tasks;
using Nightingale.Utilitys;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace SolitaireTripeaks
{
	public class LoadingScene : BaseScene
	{
		public Image FillImage;

		public Transform chameleon;

		private float Progress;

		private float currentProgress;

		private bool _isLoadContentDone = true;

		private void Start()
		{
			SingletonBehaviour<EffectUtility>.Get().Dispose();
			SingletonBehaviour<LeaderBoardUtility>.Get().ClearCache();
			SingletonBehaviour<LeaderBoardUtility>.Get().ClearTopPlayers();
			UpdateFill(0f);
			SingletonBehaviour<GlobalConfig>.Get().PlayBackground();
			
			//StartCoroutine(AsyncLoadContent());
			ReadConfig();
        }

        IEnumerator AsyncLoadContent()
        {
			_isLoadContentDone = false;
            var paths = SingletonBehaviour<LoaderUtility>.Get().GetText("StreamingAssets.txt").Split('\n');
            foreach (var pathTemp in paths)
            {
				if (!pathTemp.StartsWith("blast/Android")) continue;
				var path = pathTemp.Trim().Replace('\\', '/');
                var filePer = Path.Combine(Application.persistentDataPath, path);
                //Debug.Log($"@LOG LoadingScene perPath:{perPath}");
                if (File.Exists(filePer))
                {
                    Debug.Log($"@LOG LoadingScene perPath:{filePer}".Color(Color.green));
                }
                else
                {
                    //Debug.Log($"@LOG LoadingScene perPath:{perPath}".Color(Color.red));
                    var folder = filePer.Substring(0, filePer.LastIndexOf(@"/"));
					if (!Directory.Exists(folder))
                    {
                        DMCFileUtilities.CreateDirectory(folder);
						yield return new WaitForSeconds(1);
                    }
					//Debug.Log($"@LOG LoadingScene folder:{folder}".Color(Color.red));
					string fileStreaming = fileStreaming = StreamingAssetsPathUtility.StreamingAssetsPath(path);
					Debug.Log($"@LOG LoadingScene fileStreaming:{fileStreaming}");
					//AndroidNativeFunctions.ShowToast($"LoadingScene fileStreaming:{fileStreaming}");
					UnityWebRequest www = UnityWebRequest.Get(fileStreaming);
					yield return www.SendWebRequest();

					if(string.IsNullOrEmpty(www.error))
                    {
						File.WriteAllBytes(filePer, www.downloadHandler.data);
						yield return new WaitForSeconds(0.2f);
                    }
                }
            }
			yield return null;
            ReadConfig();
		}

        protected override void OnDestroy()
		{
			base.OnDestroy();
			SingletonBehaviour<LoaderUtility>.Get().UnLoadScene(typeof(LoadingScene).Name);
		}

		private void ReadConfig()
		{
			Debug.Log("@LOG ReadConfig");
			TaskHelper.Get("Loading").AppendTask(new LocalAssetTask(GlobalConfig.GetPathByRuntimePlatform("remote/config.asset"))).RemoveAllListeners()
				.AddListener(delegate(object asset, float pro)
				{
					Debug.Log("@LOG ReadConfig completed 1");
                    if (asset != null)
                    {
						Debug.Log("@LOG ReadConfig completed 2");
                        AssetBundle assetBundle = asset as AssetBundle;
                        GameConfig.PutAssetBundle(assetBundle);
                        LevelRetrunCoinConfig.PutAssetBundle(assetBundle);
						UniverseConfig.PutAssetBundle(assetBundle);
						SingletonBehaviour<ThirdPartyAdManager>.Get().Initialization(assetBundle);
						HightScoreRewardGroup.PutAssetBundle(assetBundle);
						VersionConfig.Initialization(assetBundle, delegate
						{
							Progress = 1f;
						});
						assetBundle.Unload(unloadAllLoadedObjects: true);
						DownloadRemoteConfig();
					}
                });
		}

		private void DownloadRemoteConfig()
		{
			Debug.Log("@LOG DownloadRemoteConfig");
            TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress, 
				GlobalConfig.GetPathByRuntimePlatform("remote/config.asset"))).RemoveAllListeners()
				.AddListener(delegate(object asset, float pro)
				{
					Debug.Log("@LOG DownloadRemoteConfig 1");
                    if (asset != null)
					{
						Debug.Log("@LOG DownloadRemoteConfig 2");
                        AssetBundle assetBundle = asset as AssetBundle;
						GameConfig.PutAssetBundle(assetBundle);
						LevelRetrunCoinConfig.PutAssetBundle(assetBundle);
						UniverseConfig.PutAssetBundle(assetBundle);
						HightScoreRewardGroup.PutAssetBundle(assetBundle);
						assetBundle.Unload(unloadAllLoadedObjects: true);
                    }
                    DownloadAssets();
                    //_isLoadContentDone = true;
                });
        }

		private void DownloadAssets()
        {
			Debug.Log("@LOG DownloadAssets");
            TaskHelper.GetDownload().AppendTask(new RemoteAssetTask(NightingaleConfig.Get().StorageBlobAddress,
				GlobalConfig.GetPathByRuntimePlatform("local/1.asset"))).RemoveAllListeners()
				.AddListener(delegate (object asset, float pro)
				{
                    _isLoadContentDone = true;
                });
        }

		private void UpdateFill(float progress)
		{
			FillImage.fillAmount = progress;
			Transform transform = chameleon;
			float x = -385f + 770f * progress;
			Vector3 localPosition = chameleon.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = chameleon.localPosition;
			transform.localPosition = new Vector3(x, y, localPosition2.z);
		}

		private void Update()
		{
			currentProgress = Mathf.Lerp(currentProgress, Progress, Time.fixedDeltaTime * 5f);
			UpdateFill(currentProgress);
			if (_isLoadContentDone && Progress > 0f && Mathf.Abs(1f - currentProgress) <= 0.01f)
			{
				JoinPlayHelper.JoinPlay();
				base.enabled = false;
			}
		}
	}
}
