//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//using System.IO;
//using System.Linq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//using com.F4A.MobileThird;

//namespace com.F4A.AssetBundles
//{
//using DG.Tweening;
//    public enum ETypeDownloadAssetBundle
//    {
//        LoadFromMemoryAsync,
//        LoadFromFile,
//        LoadFromCacheOrDownload,
//        UnityWebRequest
//    }

//	public enum ETypeLoadConfigAssetBundle{
//		None,
//		FileFromLocal,
//		FileFromServer,
//	}

//    [Serializable]
//    public class PlatformBundleInfo
//    {
//        [JsonProperty("url_server")]
//        public string UrlServer = "";
//        [JsonProperty("enable")]
//        public bool Enable = false;
//        [JsonProperty("version")]
//        public string Version = "1.0.0";
//    }

//    [Serializable]
//    public class BundleInfo
//    {
//        [JsonProperty("url_download")]
//        public string UrlDownload;
//        [JsonProperty("asset_bundle_name")]
//        public string Name;
//        [JsonProperty("type")]
//        public string Type;
//        [JsonProperty("sub_type")]
//        public string SubType = "";
//        [JsonProperty("version")]
//        public int Version = 0;
//        [JsonProperty("first_load")]
//        public bool FirstLoad = true;
//        [JsonProperty("objects")]
//        public ObjectBundleInfo[] objects;
//    }

//    [Serializable]
//    public class ObjectBundleInfo
//    {
//        [JsonProperty("name")]
//        public string Name;
//    }

//    [Serializable]
//    public class AssetBundleInfo
//    {
//        public BundleInfo bundleInfo;
//        public AssetBundle assetBundle;

//        public AssetBundleInfo()
//        {

//        }

//        public AssetBundleInfo(BundleInfo bundleInfo, AssetBundle assetBundle)
//        {
//            this.bundleInfo = bundleInfo;
//            this.assetBundle = assetBundle;
//        }
//    }

//    public class F4AAssetBundleManager : SingletonMonoDontDestroy<F4AAssetBundleManager>
//    {
//        #region Constants
//        private const string KeyAssets = "assets";
//        private const string KeyEnable = "enable";
//	    private const string KeyABConfig = "F4AAB.Config";
//        #endregion

//        #region Action, Delegates
//        /// <summary>
//        /// ACTION
//        /// </summary>

//        //
//        public static event Action OnLoadAllAssetStart;
//        // Load complete all asset bundle
//        public static event Action OnLoadAllAssetCompleted;
//        public static event Action OnLoadAllAssetFailed;
//        // 
//        public static event Action<string> OnLoadAssetStart;
//        //Load complete a asset bundle
//        public static event Action<string> OnLoadAssetCompleted;
//        public static event Action<string> OnLoadAssetFailed;

//        //
//        public static event Action<string, float, long> OnDownloadProgress;
//        //
//        public static event Action<string, float> OnRequestProgress;
//        #endregion


//        #region Variables and Properties
//        [SerializeField]
//        protected ETypeDownloadAssetBundle typeDownloadAssetBundle = ETypeDownloadAssetBundle.LoadFromCacheOrDownload;

//        [SerializeField]
//        protected PlatformBundleInfo platformBundleInfo = null;
//        [SerializeField]
//        [Header("Asset Bundle Info")]
//        protected BundleInfo[] BundleInfos = null;
//        [Tooltip("Type Load Config Asset Bundle")]
//        public ETypeLoadConfigAssetBundle typeLoadConfigAB = ETypeLoadConfigAssetBundle.None;
//        [SerializeField]
//        protected TextAsset assetBundleConfig = null;
//        [SerializeField]
//	    protected string urlConfigAssetBundle = "URL Config Asset Bundle";
//	    [SerializeField]
//	    protected TextAsset configDefault = null;
//        [SerializeField]
//        private string platformName = "";

//        /// <summary>
//        /// Dictionary contain all AssetBundle of program
//        /// </summary>
//        private Dictionary<string, AssetBundleInfo> dicAssetBundleInfo = new Dictionary<string, AssetBundleInfo>();

//        protected long TotalRequiredFileSize = 100;
//		#endregion

//		#region Unity Methods
//		private void Start()
//		{
//            F4ACoreManager.OnDownloadF4AConfigCompleted += F4ACoreManager_OnDownloadF4AConfigCompleted;
//        }

//        private void OnDestroy()
//		{
//            F4ACoreManager.OnDownloadF4AConfigCompleted -= F4ACoreManager_OnDownloadF4AConfigCompleted;
//        }
//        #endregion

//        #region Handles, Events

//        private void F4ACoreManager_OnDownloadF4AConfigCompleted(F4AConfigData configData, bool success)
//        {
//            if (configData != null &&  !string.IsNullOrEmpty(configData.urlAssetBundle))
//            {
//                urlConfigAssetBundle = F4ACoreManager.Instance.ConfigData.urlAssetBundle;
//            }
//        }
//        #endregion

//		#region Methods
//		/// <summary>
//		/// Initialization
//		/// </summary>
//		protected override void Initialization()
//        {
//	        platformName = F4AAssetBundlesUtility.GetPlatformName();
//        }


//        public void StartDownloadAssetBundle()
//        {
//            StartCoroutine(AsyncDownloadAssetBundle());
//        }

//	    bool isLoadAllAssetSuccess = false;

//	    bool isLoadConfigTextSuccess = false;
//	    string textConfig = "";
//        protected IEnumerator AsyncDownloadAssetBundle()
//        {
//	        isLoadAllAssetSuccess = false;

//	        isLoadConfigTextSuccess = false;
//	        textConfig = "";

//	        if (!string.IsNullOrEmpty(urlConfigAssetBundle) && typeLoadConfigAB == ETypeLoadConfigAssetBundle.FileFromServer)
//                yield return StartCoroutine(AsyncLoadConfigAsset(urlConfigAssetBundle));

//	        if(typeLoadConfigAB == ETypeLoadConfigAssetBundle.FileFromServer || typeLoadConfigAB == ETypeLoadConfigAssetBundle.FileFromLocal){
//#if UNITY_EDITOR
//		        Debug.Log(string.Format("<color=blue>AsyncLoadAssetBundle platformName: {0}</color>", platformName));
//#endif
//		        JObject jobject = null;

//		        if (typeLoadConfigAB == ETypeLoadConfigAssetBundle.FileFromLocal && assetBundleConfig)
//		        {
//		        	textConfig = assetBundleConfig.text;
//		        }
//		        else if(typeLoadConfigAB == ETypeLoadConfigAssetBundle.FileFromServer) // from server
//		        {
//		        	if(string.IsNullOrEmpty(textConfig)){
//		        		textConfig = PlayerPrefs.GetString(KeyABConfig, "");
//		        	}
//		        	if(string.IsNullOrEmpty(textConfig) && configDefault){
//		        		textConfig = configDefault.text;
//		        	}
//		        }

//		        if(!string.IsNullOrEmpty(textConfig)){
//		        	jobject = JsonConvert.DeserializeObject<JObject>(textConfig);
//			        // Get Version Asset Bundle
//			        if (jobject != null && jobject[platformName] != null)
//			        {
//				        BundleInfos = new BundleInfo[0];
//				        jobject = JsonConvert.DeserializeObject<JObject>(jobject[platformName].ToString());
//				        platformBundleInfo = JsonConvert.DeserializeObject<PlatformBundleInfo>(jobject.ToString());
//				        if (jobject != null && platformBundleInfo != null && platformBundleInfo.Enable)
//				        {
//					        Debug.Log("LoadAssetBundle platformName: " + platformName);
//					        BundleInfos = JsonConvert.DeserializeObject<BundleInfo[]>(jobject[KeyAssets].ToString());
//					        if (BundleInfos != null && BundleInfos.Length > 0)
//					        {
//						        if (OnLoadAllAssetStart != null)
//							        OnLoadAllAssetStart();
//						        dicAssetBundleInfo.Clear();
//						        isLoadAllAssetSuccess = true;
//						        switch (typeDownloadAssetBundle)
//						        {
//						        case ETypeDownloadAssetBundle.LoadFromCacheOrDownload:
//							        yield return StartCoroutine(LoadFromCacheOrDownload());
//							        break;
//						        default:
//							        yield return null;
//							        break;
//						        }
//					        }
//				        }
//			        }
//			        yield return null;

//			        if (isLoadAllAssetSuccess)
//			        {
//				        Debug.Log("<color=green>OnLoadAllAssetCompleted</color>");
//				        PlayerPrefs.SetString(KeyABConfig, textConfig);
//				        if (OnLoadAllAssetCompleted != null)
//				        {
//					        OnLoadAllAssetCompleted();
//				        }
//			        }
//			        else
//			        {
//				        Debug.Log("<color=red>OnLoadAllAssetFailed</color>");
//				        if (OnLoadAllAssetFailed != null)
//				        {
//					        OnLoadAllAssetFailed();
//				        }
//			        }
//		        }
//		        else{
//		        	DOVirtual.DelayedCall(2, ()=>{
//		        		Debug.Log("<color=red>OnLoadAllAssetFailed</color>");
//		        		if (OnLoadAllAssetFailed != null)
//				        {
//					        OnLoadAllAssetFailed();
//				        }
//		        	});
//		        }
//	        }
//	        else{
//	        	if (OnLoadAllAssetCompleted != null)
//	        	{
//		        	OnLoadAllAssetCompleted();
//	        	}
//	        }
//        }

//        IEnumerator AsyncLoadConfigAsset(string urlConfig)
//        {
//            isLoadConfigTextSuccess = false;
//            //string url = Path.Combine(urlServer, "CustomerConfig");
//            //url = Path.Combine(url, ConfigManager.instance.customerInfo.customerId + ".txt");
//            Debug.Log("AsyncLoadConfigAsset url: " + urlConfig);
//            if(string.IsNullOrEmpty(urlConfig))
//            {
//                textConfig = "";
//                isLoadConfigTextSuccess = false;
//                yield break;
//            }
//	        WWW www = new WWW(urlConfig);
//#if UNITY_WEBGL
//	        bool isDone = false;
//	        float timeRun = 0;
//	        while(!isDone && timeRun < 8){
//	        timeRun += Time.deltaTime;
//	        isDone = www.isDone;
//	        }
//#else
//	        while (!www.isDone)
//	        {
//		        yield return null;
//	        }
//#endif
//            yield return www;
//            if (!string.IsNullOrEmpty(www.error))
//            {
//                textConfig = "";
//                isLoadConfigTextSuccess = false;
//            }
//            else
//            {
//                textConfig = www.text;
//                isLoadConfigTextSuccess = true;
//            }
//            //File.WriteAllText(Path.Combine(Application.persistentDataPath, XMirror.ConfigManager.instance.customerInfo.customerId + ".txt"), www.text);
//        }

//        #region LoadFromCacheOrDownload

//        protected IEnumerator LoadFromCacheOrDownload()
//        {
//            foreach (var info in BundleInfos)
//            {
//                string urlAssetBundle = "";
//                if (string.IsNullOrEmpty(info.UrlDownload))
//                {
//                    urlAssetBundle = Path.Combine(platformBundleInfo.UrlServer, platformName);
//                    urlAssetBundle = Path.Combine(urlAssetBundle, info.Name);
//                }
//                else
//                {
//                    urlAssetBundle = info.UrlDownload;
//                }
//                if (!string.IsNullOrEmpty(urlAssetBundle))
//                {
//                    Debug.Log("LoadAssetBundle Start Load Asset Bundle: " + info.Name + " at " + urlAssetBundle);
//                    //WWW www = WWW.LoadFromCacheOrDownload(info.LinkUrl, info.Version);
//                    WWW www = WWW.LoadFromCacheOrDownload(urlAssetBundle, info.Version);
//                    StartCoroutine(WatchDownloadingProgress(info.Name, www));
//                    yield return www;
//                    if (www.isDone && string.IsNullOrEmpty(www.error))
//                    {
//                        AssetBundleInfo assetBundleInfo = new AssetBundleInfo(info, www.assetBundle);
//                        dicAssetBundleInfo[info.Name] = assetBundleInfo;
//                        isLoadAllAssetSuccess &= true;
//                        Debug.Log("<color=green>Load Asset " + info.Name + " Completed!</color>");
//                        if (OnLoadAssetCompleted != null)
//                        {
//                            OnLoadAssetCompleted(info.Name);
//                        }
//                    }
//                    else
//                    {
//                        isLoadAllAssetSuccess &= false;
//                        Debug.Log("<color=green>Load Asset " + info.Name + " Failed!</color>");
//                        if (OnLoadAssetFailed != null)
//                        {
//                            OnLoadAssetFailed(info.Name);
//                        }
//                    }
//                    www.Dispose();
//                }
//                else
//                {
//                    OnLoadAssetFailed(info.Name);
//                    Debug.Log("<color=green>Load Asset " + info.Name + " Failed!");
//                    yield return null;
//                }
//            }
//        }

//        protected IEnumerator WatchDownloadingProgress(string bundleName, WWW www)
//	    {
//            while (!www.isDone)
//            {
//                if (OnDownloadProgress != null)
//                {
//                    OnDownloadProgress(bundleName, www.progress, TotalRequiredFileSize);
//                }
//#if UNITY_EDITOR
//                Debug.Log("WatchDownloadingProgress " + bundleName + ": " + www.progress);
//#endif
//                yield return null;
//            }

//            if (OnDownloadProgress != null)
//            {
//                OnDownloadProgress(bundleName, 1, TotalRequiredFileSize);
//                OnDownloadProgress = null;
//            }
//            Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + " is complete</color>");
//        }

//        public IEnumerator WatchRequestingProgress(string bundleName, AssetBundleRequest request)
//        {
//            while (!request.isDone)
//            {
//                if (OnDownloadProgress != null)
//                {
//                    OnRequestProgress(bundleName, request.progress);
//                }
//#if UNITY_EDITOR
//                Debug.Log("WatchRequestingProgress " + bundleName + ": " + request.progress);
//#endif
//                yield return null;
//            }

//            if (OnRequestProgress != null)
//            {
//                OnRequestProgress(bundleName, 1);
//                OnRequestProgress = null;
//            }
//            Debug.Log("<color=green>WatchRequestingProgress " + bundleName + " is complete</color>");
//        }

//        #endregion

//        public AssetBundle GetAssetBundle(string assetBundleName)
//        {
//            if (!dicAssetBundleInfo.ContainsKey(assetBundleName) || dicAssetBundleInfo[assetBundleName] == null)
//                return null; //return new AssetBundle();
//            else
//            {
//                if (dicAssetBundleInfo[assetBundleName].bundleInfo.Type.ToLower().Equals("prefab"))
//                    return dicAssetBundleInfo[assetBundleName].assetBundle;
//                else
//                    return null;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="type">type objects in pack (prefab, scene, image, ...)</param>
//        /// <param name="subType">sub type, option of developer(clothes, image_ui, ...)</param>
//        /// <returns></returns>
//        public Dictionary<string, AssetBundleInfo> GetAssetBundle(string type, string subType)
//        {
//            var assets = dicAssetBundleInfo.Where(info => info.Value.bundleInfo.Type.ToLower().Equals(type.ToLower())
//                && info.Value.bundleInfo.SubType.ToLower().Equals(subType.ToLower()));
//            Dictionary<string, AssetBundleInfo> dic = new Dictionary<string, AssetBundleInfo>();
//            foreach (var asset in assets)
//            {
//                dic[asset.Key] = asset.Value;
//            }
//            return dic;
//        }

//        public IEnumerator LoadAllAssetsAsync(string assetBundleName, Type type, Action<object> onComplete)
//        {
//            if (!dicAssetBundleInfo.ContainsKey(assetBundleName) || dicAssetBundleInfo[assetBundleName] == null)
//                yield return null;
//            else
//            {
//                if (dicAssetBundleInfo[assetBundleName].bundleInfo.Type.ToLower().Equals("prefab"))
//                {
//                    AssetBundle bundle = dicAssetBundleInfo[assetBundleName].assetBundle;
//                    AssetBundleRequest request = bundle.LoadAllAssetsAsync(type);
//                    StartCoroutine(WatchRequestingProgress(assetBundleName, request));
//                    yield return request;
//                    if (onComplete != null)
//                    {
//                        onComplete(request.allAssets);
//                    }
//                }
//                else
//                    yield return null;
//            }
//        }

//        public IEnumerator LoadAssetAsync(string assetBundleName, string assetName, Type type, Action<object> onComplete)
//        {
//            if (!dicAssetBundleInfo.ContainsKey(assetBundleName) || dicAssetBundleInfo[assetBundleName] == null)
//            {
//                Debug.Log("<color=red>LoadAssetAsync Can't exist " + assetBundleName + " bundle</color>");
//                yield return null;
//            }
//            else
//            {
//                AssetBundle bundle = dicAssetBundleInfo[assetBundleName].assetBundle;
//                AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
//                StartCoroutine(WatchRequestingProgress(assetBundleName, request));
//                yield return request;
//                if (onComplete != null)
//                {
//                    onComplete(request.asset);
//                }
//            }
//        }

//        public T[] LoadAllAssetAsync<T>(string assetBundleName) where T : UnityEngine.Object
//        {
//            return null;
//        }

//        public T LoadAssetAsync<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
//        {
//            return null;
//        }

//        public AssetBundle GetLevelBundle(string assetBundleName)
//        {
//            if (!dicAssetBundleInfo.ContainsKey(assetBundleName) || dicAssetBundleInfo[assetBundleName] == null)
//                return null; //return new AssetBundle();
//            else
//            {
//                if (dicAssetBundleInfo[assetBundleName].bundleInfo.Type.ToLower().Equals("scene"))
//                    return dicAssetBundleInfo[assetBundleName].assetBundle;
//                else
//                    return null;
//            }
//        }


//        public string GetVersion()
//        {
//            if (platformBundleInfo != null)
//            {
//                return platformBundleInfo.Version;
//            }
//            else
//            {
//                return "Version zero";
//            }
//        }
//        #endregion
//    }
//}

namespace com.F4A.MobileThird
{
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
#if UNITY_2018_3_OR_NEWER
    using UnityEngine.Networking;
#endif

    [Serializable]
    public class DMCAssetBundleConfigInfo
    {
        [SerializeField]
        private List<DMCPlatformBundleInfo> _listPlatformBundleInfo;
        public List<DMCPlatformBundleInfo> ListPlatformBundleInfo
        {
            get { return _listPlatformBundleInfo; }
            set { _listPlatformBundleInfo = value; }
        }

        [SerializeField]
        private int _timeWaitConnect = 10;
        public int TimeWaitConnect
        {
            get { return _timeWaitConnect; }
            set { _timeWaitConnect = value; }
        }

        [SerializeField]
        private bool _enableLogProgress;
        public bool EnableLogProgress
        {
            get { return _enableLogProgress; }
        }

    }

    [Serializable]
    public class DMCPlatformBundleInfo
    {
        [JsonProperty("platform")]
        public string PlatformName = "";
        [JsonProperty("url_server")]
        public string UrlServer = "";
        [JsonProperty("enable")]
        public bool Enable = true;
        [JsonProperty("version")]
        public string Version = "1.0.0";
        [JsonProperty("version_code")]
        public int VersionCode = 1;
        [JsonProperty("assets")]
        public DMCBundleInfo[] BundleInfos;

        [SerializeField]
        private bool _isDownloadAssetWhenStart = false;
        public bool IsDownloadAssetWhenStart
        {
            get { return _isDownloadAssetWhenStart; }
            set { _isDownloadAssetWhenStart = value; }
        }
    }

    [Serializable]
    public class DMCBundleInfo
    {
        [JsonProperty("url_download")]
        public string UrlDownload;
        [JsonProperty("asset_bundle_name")]
        public string Name;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("sub_type")]
        public string SubType = "";
        [JsonProperty("version")]
        public int Version = 0;
        [JsonProperty("first_load")]
        public bool FirstLoad = true;
        [JsonProperty("objects")]
        public DMCObjectBundleInfo[] ObjectInBundles;

        //[JsonIgnore]
        //public AssetBundle assetBundle;
    }

    [Serializable]
    public class DMCObjectBundleInfo
    {
        [JsonProperty("name")]
        public string Name;
    }

    [Serializable]
    public class DMCAssetBundleData
    {
        public DMCBundleInfo bundleInfo;
        public AssetBundle assetBundle;

        public DMCAssetBundleData()
        {

        }

        public DMCAssetBundleData(DMCBundleInfo bundleInfo, AssetBundle assetBundle)
        {
            this.bundleInfo = bundleInfo;
            this.assetBundle = assetBundle;
        }
    }

    public class F4AAssetBundleManager : SingletonMonoDontDestroy<F4AAssetBundleManager>
    {

        #region Constants
        private const string KeyABConfig = "VSGAB.Config";
        #endregion

        #region Events
        public static event Action OnDownloadConfigCompleted = delegate { };

        public static event Action OnDownloadAllAssetStart = delegate { };
        public static event Action OnDownloadAllAssetCompleted = delegate { };
        public static event Action OnDownloadAllAssetFailed = delegate { };

        // string: asset name
        public static event Action<string> OnDownloadAssetStart = delegate { };
        public static event Action<string> OnDownloadAssetCompleted = delegate { };
        public static event Action<string> OnDownloadAssetFailed = delegate { };

        public static event Action<string, float> OnDownloadAssetProgress = delegate { };
        #endregion


        #region Variables and Properties
        [Header("Setup Config")]
        [SerializeField]
        private DMCAssetBundleConfigInfo _assetBundleConfigInfo;
        //[SerializeField]
        //private ELoadFileMethod _loadFileConfigBundleMethod = ELoadFileMethod.FileFromLocal;
        [SerializeField]
        protected string _urlFileConfigBundle = "URL Config Asset Bundle";
        public string UrlFileConfigBundle
        {
            get { return _urlFileConfigBundle; }
            set { _urlFileConfigBundle = value; }
        }

        [SerializeField]
        protected TextAsset _textConfigLocal = null;

        [SerializeField]
        protected EDownloadAssetBundleMethod _downloadAssetBundleMethod = EDownloadAssetBundleMethod.LoadFromCacheOrDownload;
        [SerializeField]
        protected DMCPlatformBundleInfo _infoPlatformBundle;

        [SerializeField]
        protected string _platformName = "";
        [SerializeField]
        private bool _isDownloadingAsset = false;

        /// <summary>
        /// Dictionary contain all AssetBundle of program
        /// </summary>
        private Dictionary<string, DMCAssetBundleData> _dicAssetBundleData = new Dictionary<string, DMCAssetBundleData>();

        protected long TotalRequiredFileSize = 100;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _platformName = DMCMobileUtils.GetPlatformName();
            F4ACoreManager.OnDownloadF4AConfigCompleted += F4ACoreManager_OnDownloadF4AConfigCompleted;
        }

        private void OnDestroy()
        {
            F4ACoreManager.OnDownloadF4AConfigCompleted -= F4ACoreManager_OnDownloadF4AConfigCompleted;

        }
        #endregion

        private void F4ACoreManager_OnDownloadF4AConfigCompleted(F4AConfigData info, bool success)
        {
            if (!string.IsNullOrEmpty(info.urlAssetBundle))
            {
                _urlFileConfigBundle = info.urlAssetBundle;
            }

            // Load Config File Bundle
            StartCoroutine(DMCMobileUtils.AsyncGetDataFromUrl(_urlFileConfigBundle, _textConfigLocal, (string data) =>
            {
                if (!string.IsNullOrEmpty(data))
                {
                    _assetBundleConfigInfo = JsonConvert.DeserializeObject<DMCAssetBundleConfigInfo>(data);
                }
                _infoPlatformBundle = _assetBundleConfigInfo.ListPlatformBundleInfo.Where(p => p.PlatformName.Equals(_platformName)).FirstOrDefault();
                OnDownloadConfigCompleted?.Invoke();

                if (_infoPlatformBundle != null && _infoPlatformBundle.IsDownloadAssetWhenStart)
                {
                    DownloadAssetBundle();
                }
            }));
        }

        #region Methods
        /// <summary>
        /// Initialization
        /// </summary>
        protected override void Initialization()
        {
            _dicAssetBundleData.Clear();
        }

        public void DownloadAssetBundle()
        {
            StartCoroutine(AsyncDownloadAssetBundle());
        }

        private bool _isLoadAllAssetSuccess = false;
        private bool _isLoadConfigTextSuccess = false;
        protected IEnumerator AsyncDownloadAssetBundle()
        {
            Debug.Log(string.Format("<color=blue>AsyncDownloadAssetBundle platformName: {0}</color>", _platformName));

            if (_isDownloadingAsset) yield return null;

            _isDownloadingAsset = true;
            _isLoadAllAssetSuccess = false;
            _isLoadConfigTextSuccess = false;

            // Load Asset Bundles
            if (_infoPlatformBundle != null && _infoPlatformBundle.Enable)
            {
                _isLoadAllAssetSuccess = true;
                if (OnDownloadAllAssetStart != null) OnDownloadAllAssetStart();
                switch (_downloadAssetBundleMethod)
                {
                    case EDownloadAssetBundleMethod.LoadFromCacheOrDownload:
                        yield return StartCoroutine(AsyncLoadAllFromCacheOrDownload());
                        break;
                    default:
                        yield return null;
                        break;
                }
            }
            yield return null;

            if (_isLoadAllAssetSuccess)
            {
                Debug.Log("<color=green>OnLoadAllAssetCompleted</color>");
                if (OnDownloadAllAssetCompleted != null) OnDownloadAllAssetCompleted();
            }
            else
            {
                Debug.Log("<color=red>OnLoadAllAssetFailed</color>");
                if (OnDownloadAllAssetFailed != null) OnDownloadAllAssetFailed();
            }

            _isDownloadingAsset = false;
        }

        #region LoadFromCacheOrDownload

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IEnumerator AsyncLoadAllFromCacheOrDownload()
        {
            foreach (DMCBundleInfo info in _infoPlatformBundle.BundleInfos)
            {
                yield return StartCoroutine(AsyncLoadFromCacheOrDownload(info));
            }
            yield return null;
        }

        protected IEnumerator AsyncLoadFromCacheOrDownload(DMCBundleInfo info)
        {
            if (!_dicAssetBundleData.ContainsKey(info.Name))
            {
                string urlAssetBundle = "";
                bool isDownloadAssetSuccess = false;
                if (string.IsNullOrEmpty(info.UrlDownload) ||
                    (!info.UrlDownload.StartsWith("http") && !info.UrlDownload.StartsWith("file:\\")))
                {
                    urlAssetBundle = Path.Combine(_infoPlatformBundle.UrlServer, _platformName);
                    //if(!string.IsNullOrEmpty(info.UrlDownload)) urlAssetBundle = Path.Combine(_infoPlatformBundle.UrlServer, info.UrlDownload);
                    urlAssetBundle = Path.Combine(urlAssetBundle, info.Name);
                }
                else
                {
                    urlAssetBundle = info.UrlDownload;
                }

                if (!string.IsNullOrEmpty(urlAssetBundle))
                {
                    while (!Caching.ready)
                    {
                        yield return null;
                    }

                    OnDownloadAssetStart.Invoke(info.Name);

                    Hash128 hash = default(Hash128);
                    hash = Hash128.Parse(info.Name.Replace('/', '_').Replace(@"\", "_"));
                    Debug.Log("LoadAssetBundle Start Load Asset Bundle: " + info.Name + " at " + urlAssetBundle);
                    CachedAssetBundle cachedAssetBundle = new CachedAssetBundle(info.Name.Replace('/', '_').Replace(@"\", "_"), hash);

                    if (!Caching.IsVersionCached(cachedAssetBundle))
                    {
                        int timeWaitConnect = 0;
                        while (!DMCMobileUtils.IsInternetAvailable() && timeWaitConnect <= _assetBundleConfigInfo.TimeWaitConnect)
                        {
                            timeWaitConnect += 1;
                            yield return new WaitForSeconds(1);
                        }
                    }
#if UNITY_2018_3_OR_NEWER
                    UnityWebRequest www = new UnityWebRequest();
                    //www = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle, hash, 0u);
                    www = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle, cachedAssetBundle, 0u);
                    StartCoroutine(WatchDownloadingProgress(info.Name, www));
                    yield return www.SendWebRequest();
                    var assetBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                    if (www.isDone && string.IsNullOrEmpty(www.error) && assetBundle)
                    {
                        DMCAssetBundleData bundleData = new DMCAssetBundleData(info, assetBundle);
                        _dicAssetBundleData[info.Name] = bundleData;
                        _isLoadAllAssetSuccess &= true;
                        isDownloadAssetSuccess = true;
                        CPlayerPrefs.SetString(info.Name + "_hash", hash.ToString());
                    }
                    else
                    {
                        Debug.Log(www.error);
                        _isLoadAllAssetSuccess &= false;
                    }
                    www.Dispose();
                    www = null;
#else
                        //WWW www = WWW.LoadFromCacheOrDownload(info.LinkUrl, info.Version);
                        WWW www = WWW.LoadFromCacheOrDownload(urlAssetBundle, cachedAssetBundle, 0u);
                        //WWW www = WWW.LoadFromCacheOrDownload(urlAssetBundle, hash);
                        StartCoroutine(WatchDownloadingProgress(info.Name, www));
                        yield return www;
                        if (www.isDone && string.IsNullOrEmpty(www.error))
                        {
                            DMCAssetBundleData bundleData = new DMCAssetBundleData(info, www.assetBundle);
                            _dicAssetBundleData[info.Name] = bundleData;
                            _isLoadAllAssetSuccess &= true;
                            isDownloadAssetSuccess = true;
                            CPlayerPrefs.SetString(info.Name + "_hash", hash.ToString());
                        }
                        else
                        {
                            isLoadAllAssetSuccess &= false;
                        }
                        www.Dispose();
                        www = null;
#endif
                    Resources.UnloadUnusedAssets();
                }


                if (isDownloadAssetSuccess)
                {
                    Debug.Log("<color=green>Load Asset " + info.Name + " Success!</color>");
                    OnDownloadAssetCompleted.Invoke(info.Name);
                    yield return null;
                }
                else
                {
                    Debug.Log("<color=red>Load Asset " + info.Name + " Failed!</color>");
                    OnDownloadAssetFailed.Invoke(info.Name);
                    yield return null;
                }

                yield return null;
            }
        }

#if UNITY_2018_3_OR_NEWER
        protected IEnumerator WatchDownloadingProgress(string bundleName, UnityWebRequest www)
        {
            while (www != null && !www.isDone)
            {
                if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, www.downloadProgress);
                if (_assetBundleConfigInfo.EnableLogProgress)
                {
                    Debug.Log("WatchDownloadingProgress " + bundleName + ": " + www.downloadProgress);
                }
                yield return null;
            }

            if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, 1);
            Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + " is complete</color>");
            yield return null;
        }
#else
        protected IEnumerator WatchDownloadingProgress(string bundleName, WWW www)
        {
            while (!www.isDone)
            {
                if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, www.progress);
                if (_assetBundleConfigInfo.EnableLogProgress)
                {
                    Debug.Log("WatchDownloadingProgress " + bundleName + ": " + www.progress);
                }
                yield return null;
            }

            if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, 1);
            //Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + " is complete</color>");
        }
#endif

        public IEnumerator WatchRequestingProgress(string bundleName, AssetBundleRequest request)
        {
            while (!request.isDone)
            {
                if (_assetBundleConfigInfo.EnableLogProgress)
                {
                    Debug.Log("WatchRequestingProgress " + bundleName + ": " + request.progress);
                }
                yield return null;
            }

            Debug.Log("<color=green>WatchRequestingProgress " + bundleName + " is complete</color>");
            yield return null;
        }

        #endregion

        #region OLD
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="type">type objects in pack (prefab, scene, image, ...)</param>
        ///// <param name="subType">sub type, option of developer(clothes, image_ui, ...)</param>
        ///// <returns></returns>
        //public Dictionary<string, DMCAssetBundleData> GetAssetBundleData(string type, string subType)
        //{
        //    var assets = _dicAssetBundleData.Where(info => info.Value.bundleInfo.Type.ToLower().Equals(type.ToLower())
        //        && info.Value.bundleInfo.SubType.ToLower().Equals(subType.ToLower()));
        //    Dictionary<string, DMCAssetBundleData> dic = new Dictionary<string, DMCAssetBundleData>();
        //    foreach (var asset in assets)
        //    {
        //        dic[asset.Key] = asset.Value;
        //    }
        //    return dic;
        //}

        //public IEnumerator LoadAllAssetsAsync(string assetBundleName, Type type, Action<object> onComplete)
        //{
        //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        //    {
        //        yield return null;
        //    }
        //    else
        //    {
        //        if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("prefab"))
        //        {
        //            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
        //            AssetBundleRequest request = bundle.LoadAllAssetsAsync(type);
        //            StartCoroutine(WatchRequestingProgress(assetBundleName, request));
        //            yield return request;
        //            if (onComplete != null)
        //            {
        //                onComplete(request.allAssets);
        //            }
        //        }
        //        else
        //            yield return null;
        //    }
        //}

        //public IEnumerator LoadAssetAsync(string assetBundleName, string assetName, Type type, Action<object> onComplete)
        //{
        //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        //    {
        //        Debug.Log("<color=red>LoadAssetAsync Can't exist " + assetBundleName + " bundle</color>");
        //        yield return null;
        //    }
        //    else
        //    {
        //        AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
        //        AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
        //        StartCoroutine(WatchRequestingProgress(assetBundleName, request));
        //        yield return request;
        //        if (onComplete != null)
        //        {
        //            onComplete(request.asset);
        //        }
        //    }
        //}

        //public T[] LoadAllAssetAsync<T>(string assetBundleName) where T : UnityEngine.Object
        //{
        //    return null;
        //}

        //public T LoadAssetAsync<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        //{
        //    //if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        //    //{
        //    //    return null; //return new AssetBundle();
        //    //}
        //    //else
        //    //{
        //    //    if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("scene"))
        //    //    {
        //    //        AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
        //    //        StartCoroutine(WatchRequestingProgress(assetBundleName, request));
        //    //        yield return request;
        //    //    }
        //    //    else
        //    //    {
        //    //        return null;
        //    //    }
        //    //}

        //    return null;
        //}

        //public AssetBundle GetAssetBundle(string assetBundleName)
        //{
        //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        //    {
        //        return null; //return new AssetBundle();
        //    }
        //    else
        //    {
        //        //if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("scene"))
        //        //{
        //        //    return _dicAssetBundleData[assetBundleName].assetBundle;
        //        //}
        //        //else
        //        //{
        //        //    return null;
        //        //}

        //        return _dicAssetBundleData[assetBundleName].assetBundle;
        //    }
        //}
        #endregion


        public AssetBundleRequest GetAssetBundleRequest(string assetBundleName, string assetName, Type type)
        {
            if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
            {
                return null; //return new AssetBundle();
            }
            else
            {
                AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
                AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
                return request;
            }
        }

        public IEnumerator AsyncLoadAllAssets(string assetBundleName, Type type, Action<UnityEngine.Object[]> onComplete)
        {
            if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
            {
                yield return null;
            }
            else
            {
                AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
                AssetBundleRequest request = bundle.LoadAllAssetsAsync(type);
                StartCoroutine(WatchRequestingProgress(assetBundleName, request));
                yield return request;
                if (onComplete != null)
                {
                    onComplete(request.allAssets);
                }
            }
        }


        public IEnumerator AsyncLoadAllAssetsByName(string assetBundleName, Action<UnityEngine.Object[]> onComplete)
        {
            if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
            {
                yield return null;
            }
            else
            {
                AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
                AssetBundleRequest request = bundle.LoadAllAssetsAsync<UnityEngine.Object>();
                StartCoroutine(WatchRequestingProgress(assetBundleName, request));
                yield return request;
                if (onComplete != null)
                {
                    onComplete(request.allAssets);
                }
            }
        }

        public IEnumerator AsyncLoadAllAssets(string typeAsset, Action<UnityEngine.Object[]> onComplete)
        {
            var pairs = _dicAssetBundleData.Where(ob => ob.Value.bundleInfo.Type.Equals(typeAsset)).ToArray();

            List<UnityEngine.Object> allAssets = new List<UnityEngine.Object>();
            foreach (var pair in pairs)
            {
                AssetBundleRequest request = pair.Value.assetBundle.LoadAllAssetsAsync<UnityEngine.Object>();
                StartCoroutine(WatchRequestingProgress(pair.Key, request));
                yield return request;
                allAssets.AddRange(request.allAssets);
            }

            onComplete?.Invoke(allAssets.ToArray());
        }

        public IEnumerator AsyncLoadAllAssets(string typeAsset, string subTypeAsset, Action<UnityEngine.Object[]> onComplete)
        {
            var pairs = _dicAssetBundleData.Where(ob => ob.Value.bundleInfo.Type.Equals(typeAsset)
            && ob.Value.bundleInfo.SubType.Equals(subTypeAsset)).ToArray();

            List<UnityEngine.Object> allAssets = new List<UnityEngine.Object>();
            foreach (var pair in pairs)
            {
                AssetBundleRequest request = pair.Value.assetBundle.LoadAllAssetsAsync<UnityEngine.Object>();
                StartCoroutine(WatchRequestingProgress(pair.Key, request));
                yield return request;
                allAssets.AddRange(request.allAssets);
            }

            onComplete?.Invoke(allAssets.ToArray());
        }


        public string GetVersion()
        {
            if (_infoPlatformBundle != null)
            {
                return _infoPlatformBundle.Version;
            }
            else
            {
                return "Version zero";
            }
        }
        #endregion


#if UNITY_EDITOR
        [ContextMenu("Save Info")]
        public void SaveInfo()
        {
            string str = JsonConvert.SerializeObject(_assetBundleConfigInfo);

            string pathDirectory = Path.Combine(Application.dataPath, DMCMobileUtils.PathFolderData);
            DMCMobileUtils.CreateDirectory(pathDirectory);
            string path = Path.Combine(pathDirectory, @"AssetBundle.txt");
            StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine(str);
            file.Close();
            UnityEditor.AssetDatabase.Refresh();
        }

        [ContextMenu("Load Info")]
        public void LoadInfo()
        {
            string path = Path.Combine(Application.dataPath, DMCMobileUtils.PathFolderData);
            path = Path.Combine(path, @"AssetBundle.txt");
            if (File.Exists(path))
            {
                string text = System.IO.File.ReadAllText(path);
                _assetBundleConfigInfo = JsonConvert.DeserializeObject<DMCAssetBundleConfigInfo>(text);
            }
        }
#endif
    }
}