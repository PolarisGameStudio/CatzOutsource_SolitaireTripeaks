using UnityEngine;

namespace SolitaireTripeaks
{
	public class BaseAdditional : MonoBehaviour
	{
		public virtual void OnRemove()
		{
		}

		public virtual void OnOver()
		{
		}

		public virtual bool OnClick()
		{
			return false;
		}

		public virtual void UpdateLayer(int index)
		{
		}
	}
}
