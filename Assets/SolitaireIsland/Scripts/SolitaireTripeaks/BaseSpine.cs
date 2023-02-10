using Nightingale.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public abstract class BaseSpine : DelayBehaviour
	{
		public virtual void PlayActivation(UnityAction unityAction)
		{
		}

		public virtual void PlayDestroy(UnityAction unityAction)
		{
		}

		public virtual void PlayIndex(float index)
		{
		}

		public virtual void PlayTransform(Transform transform, UnityAction unityAction)
		{
		}

		public virtual void UpdateOrderLayer(int zIndex, int index)
		{
		}

		public virtual void UpdateColor(bool white)
		{
		}
	}
}
