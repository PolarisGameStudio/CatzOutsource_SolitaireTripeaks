using Nightingale.Extensions;
using UnityEngine;

namespace SolitaireTripeaks
{
	public abstract class NormalBooster : MonoBehaviour
	{
		protected bool enabling = true;

		protected int Number
		{
			get;
			private set;
		}

		public void OnStart(int number)
		{
			Number = number;
			NoDrawingRayCast noDrawingRayCast = base.gameObject.AddComponent<NoDrawingRayCast>();
			noDrawingRayCast.rectTransform.anchorMin = Vector2.zero;
			noDrawingRayCast.rectTransform.anchorMax = Vector2.one;
			Init(number);
		}

		protected virtual void Init(int number)
		{
		}

		protected virtual bool IsMatch(BaseCard baseCard)
		{
			return true;
		}
	}
}
