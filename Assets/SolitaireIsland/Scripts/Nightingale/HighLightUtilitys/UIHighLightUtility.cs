using Nightingale.Utilitys;
using UnityEngine;

namespace Nightingale.HighLightUtilitys
{
	public class UIHighLightUtility : SingletonBehaviour<UIHighLightUtility>
	{
		public void StartHighLightGameObject(RectTransform rectTransform)
		{
			RectTransform targetRectTransform = rectTransform;
			Canvas canvas = null;
			while (!(rectTransform == null))
			{
				canvas = rectTransform.GetComponentInParent<Canvas>();
				rectTransform = rectTransform.GetComponentInParent<RectTransform>();
				if (!(canvas == null))
				{
					break;
				}
			}
			if (canvas != null)
			{
				HighLightMask highLightMask = canvas.GetComponentInChildren<HighLightMask>();
				if (highLightMask == null)
				{
					GameObject gameObject = new GameObject("HighLight Mask");
					gameObject.transform.parent = base.transform;
					highLightMask = gameObject.AddComponent<HighLightMask>();
					highLightMask.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					highLightMask.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					highLightMask.rectTransform.pivot = new Vector2(0.5f, 0.5f);
					highLightMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.height);
					highLightMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width);
				}
				highLightMask.uiCamera = canvas.worldCamera;
				highLightMask.targetRectTransform = targetRectTransform;
			}
		}

		public void StopHighLightGameObject(RectTransform rectTransform, bool destroy = false)
		{
			Canvas canvas = null;
			while (!(rectTransform == null))
			{
				canvas = rectTransform.GetComponentInParent<Canvas>();
				rectTransform = rectTransform.GetComponentInParent<RectTransform>();
				if (!(canvas == null))
				{
					break;
				}
			}
			if (!(canvas != null))
			{
				return;
			}
			HighLightMask componentInChildren = canvas.GetComponentInChildren<HighLightMask>();
			if (componentInChildren != null)
			{
				componentInChildren.targetRectTransform = null;
				if (destroy)
				{
					UnityEngine.Object.Destroy(componentInChildren.gameObject);
				}
			}
		}
	}
}
