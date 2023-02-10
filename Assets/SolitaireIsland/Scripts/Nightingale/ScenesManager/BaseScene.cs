using Nightingale.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nightingale.ScenesManager
{
	public class BaseScene : DelayBehaviour
	{
		[HideInInspector]
		private SceneLoadEvent OnLoad = new SceneLoadEvent();

		[HideInInspector]
		private UnityEvent OnClosed = new UnityEvent();

		[HideInInspector]
		public SceneStateEvent SceneStateChanged = new SceneStateEvent();

		[Header("Specify the scene animation target, the default is the entire screen")]
		public Transform SceneEffectTransform;

		public bool IsStay
		{
			get;
			set;
		}

		public bool IsFixed
		{
			get;
			set;
		}

		private void OnApplicationQuit()
		{
			OnLoad.RemoveAllListeners();
			OnClosed.RemoveAllListeners();
		}

		protected virtual void OnDestroy()
		{
			SceneStateChanged.RemoveAllListeners();
			OnLoad.RemoveAllListeners();
			OnClosed.Invoke();
			OnClosed.RemoveAllListeners();
		}

		protected virtual void OnLoadCompeted(bool completed = false, UnityAction unityAction = null)
		{
			DelayDo(new WaitForSeconds(0.5f), delegate
			{
				if (unityAction != null)
				{
					unityAction();
				}
				OnLoad.Invoke(completed);
			});
		}

		public void SetSceneState(SceneState state)
		{
			SceneStateChanged.Invoke(state);
			OnSceneStateChanged(state);
		}

		public virtual void OnSceneStateChanged(SceneState state)
		{
		}

		public virtual void SetLayer(string sortingLayerName, int index)
		{
			Canvas component = base.transform.Find("Canvas").GetComponent<Canvas>();
			component.sortingLayerName = sortingLayerName;
			component.sortingOrder = index;
		}

		public void SetCanvasGraphicRaycaster(bool enabled)
		{
			if (base.transform == null)
			{
				return;
			}
			GraphicRaycaster[] componentsInChildren = base.transform.GetComponentsInChildren<GraphicRaycaster>();
			if (componentsInChildren != null)
			{
				GraphicRaycaster[] array = componentsInChildren;
				foreach (GraphicRaycaster graphicRaycaster in array)
				{
					graphicRaycaster.enabled = enabled;
				}
			}
		}

		public void AddLoadListener(UnityAction<bool> unityAction)
		{
			if (unityAction != null)
			{
				OnLoad.AddListener(unityAction);
			}
		}

		public void AddClosedListener(UnityAction unityAction)
		{
			if (unityAction != null)
			{
				OnClosed.AddListener(unityAction);
			}
		}

		public Transform GetSceneEffectTransform()
		{
			if (SceneEffectTransform == null)
			{
				return base.transform;
			}
			return SceneEffectTransform;
		}
	}
}
