using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public abstract class HandGroup : MonoBehaviour
	{
		public const int StartZIndex = 1000;

		public Stack<BaseCard> baseCards = new Stack<BaseCard>();

		public bool IsDestory
		{
			get;
			set;
		}

		public virtual void AppendCard(BaseCard card)
		{
		}

		public virtual BaseCard FlyCard()
		{
			return null;
		}

		public virtual void DestoryWhenSuccess(UnityAction unityAction)
		{
		}

		public virtual void DestoryWhenFaild(UnityAction unityAction)
		{
		}

		public virtual void UpdatePosition()
		{
		}

		public virtual float GetPositionX(int i)
		{
			return 0f;
		}

		public BaseCard GetTop()
		{
			if (baseCards.Count == 0)
			{
				return null;
			}
			return baseCards.Peek();
		}
	}
}
