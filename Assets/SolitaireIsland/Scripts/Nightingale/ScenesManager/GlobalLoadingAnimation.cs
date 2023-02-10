using Nightingale.Inputs;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace Nightingale.ScenesManager
{
	public class GlobalLoadingAnimation : MonoBehaviour
	{
		public static void Show(string path, LoadBaseSceneHandler loadBaseSceneHandler)
		{
			SingletonBehaviour<EscapeInputManager>.Get().AppendKey("GlobalLoading");
			GlobalLoadingAnimation[] array = UnityEngine.Object.FindObjectsOfType<GlobalLoadingAnimation>();
			GlobalLoadingAnimation[] array2 = array;
			foreach (GlobalLoadingAnimation globalLoadingAnimation in array2)
			{
				UnityEngine.Object.Destroy(globalLoadingAnimation.gameObject);
			}
			GlobalLoadingAnimation component = UnityEngine.Object.Instantiate(SingletonBehaviour<LoaderUtility>.Get().GetAsset<GameObject>(path)).GetComponent<GlobalLoadingAnimation>();
			component.OnStart(loadBaseSceneHandler);
		}

		private void OnStart(LoadBaseSceneHandler loadBaseSceneHandler)
		{
			OnOpenAnimation(delegate
			{
				BaseScene baseScene = loadBaseSceneHandler();
				if (baseScene == null)
				{
					OnClosedAnimation(delegate
					{
						SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("GlobalLoading");
						UnityEngine.Object.Destroy(base.gameObject);
					});
				}
				else
				{
					baseScene.AddLoadListener(delegate(bool success)
					{
						if (!success)
						{
							OnClosedAnimation(delegate
							{
								SingletonBehaviour<EscapeInputManager>.Get().RemoveKey("GlobalLoading");
								UnityEngine.Object.Destroy(base.gameObject);
							});
						}
					});
				}
			});
		}

		protected virtual void OnOpenAnimation(UnityAction unityAction)
		{
		}

		protected virtual void OnClosedAnimation(UnityAction unityAction)
		{
		}
	}
}
