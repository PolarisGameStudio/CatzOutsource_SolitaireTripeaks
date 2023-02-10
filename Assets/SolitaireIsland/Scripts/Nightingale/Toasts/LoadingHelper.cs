using Nightingale.Inputs;
using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.Toasts
{
	public class LoadingHelper : MonoBehaviour
	{
		private static readonly GameObject _LoadingHelperGameObject = new GameObject("LoadingHelper");

		private static Dictionary<string, LoadingHelper> LoadingHelpers = new Dictionary<string, LoadingHelper>();

		private GameObject loadingGameObject;

		private Text Label;

		private LoadingHelperEvent LoadingHelperCallback = new LoadingHelperEvent();

		private LoadingHelperEvent CloseCallback = new LoadingHelperEvent();

		private UnityEvent CloseEvent = new UnityEvent();

		private float total;

		public string LoadingPath
		{
			get;
			private set;
		}

		public static LoadingHelper Get(string loadingPath)
		{
			if (!LoadingHelpers.ContainsKey(loadingPath))
			{
				GameObject gameObject = new GameObject(loadingPath);
				gameObject.transform.SetParent(_LoadingHelperGameObject.transform);
				LoadingHelper loadingHelper = gameObject.AddComponent<LoadingHelper>();
				loadingHelper.LoadingPath = loadingPath;
				LoadingHelpers.Add(loadingPath, loadingHelper);
			}
			return LoadingHelpers[loadingPath];
		}

		public void StopLoading()
		{
			base.gameObject.SetActive(value: false);
			LoadingHelperCallback.RemoveAllListeners();
			CloseCallback.RemoveAllListeners();
			SingletonBehaviour<EscapeInputManager>.Get().Remove(OnPressBack);
			CloseEvent.Invoke();
			CloseEvent.RemoveAllListeners();
		}

		public void ShowLoading(UnityAction<LoadingHelper, float> loadingHelperCallback = null, UnityAction<LoadingHelper, float> closeCallback = null, string content = "")
		{
			if (loadingGameObject == null)
			{
				loadingGameObject = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>("ToastLoading"));
				loadingGameObject.transform.SetParent(base.transform, worldPositionStays: false);
				Label = loadingGameObject.GetComponentInChildren<Text>();
			}
			if (Label != null)
			{
				Label.text = content;
			}
			base.gameObject.SetActive(value: true);
			total = 0f;
			if (loadingHelperCallback != null)
			{
				LoadingHelperCallback.AddListener(loadingHelperCallback);
			}
			if (closeCallback != null)
			{
				CloseCallback.AddListener(closeCallback);
			}
			SingletonBehaviour<EscapeInputManager>.Get().Remove(OnPressBack);
			SingletonBehaviour<EscapeInputManager>.Get().InsertTop(OnPressBack);
		}

		public void AddCloseListener(UnityAction unityAction)
		{
			if (unityAction != null)
			{
				CloseEvent.AddListener(unityAction);
			}
		}

		private bool OnPressBack()
		{
			CloseCallback.Invoke(this, total);
			return true;
		}

		private void Update()
		{
			total += Time.deltaTime;
			LoadingHelperCallback.Invoke(this, total);
		}
	}
}
