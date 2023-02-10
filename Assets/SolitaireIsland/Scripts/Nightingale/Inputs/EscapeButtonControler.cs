using Nightingale.ScenesManager;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.Inputs
{
	public class EscapeButtonControler : MonoBehaviour
	{
		private void Awake()
		{
			SingletonBehaviour<EscapeInputManager>.Get().Append(OnBackKeyDown);
		}

		private void OnDestroy()
		{
			SingletonBehaviour<EscapeInputManager>.Get().Remove(OnBackKeyDown);
		}

		private bool OnBackKeyDown()
		{
			if (base.gameObject.activeInHierarchy)
			{
				Graphic component = base.gameObject.GetComponent<Graphic>();
				if (component == null || component.canvas == null)
				{
					return false;
				}
				BaseScene componentInParent = component.canvas.GetComponentInParent<BaseScene>();
				if (SingletonClass<MySceneManager>.Get().GetTopScene() == componentInParent)
				{
					GraphicRaycaster component2 = component.canvas.GetComponent<GraphicRaycaster>();
					if (component2 == null)
					{
						return true;
					}
					if (!component2.enabled)
					{
						return true;
					}
					Button component3 = base.gameObject.GetComponent<Button>();
					if (component3.interactable)
					{
						component3.onClick.Invoke();
					}
					return true;
				}
			}
			return false;
		}
	}
}
