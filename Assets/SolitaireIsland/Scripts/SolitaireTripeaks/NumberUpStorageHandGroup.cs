using DG.Tweening;
using Nightingale.Inputs;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class NumberUpStorageHandGroup : MonoBehaviour, IStorageHandGroup
	{
		private BaseCard _baseCard;

		private UnityEvent onChanged = new UnityEvent();

		private void Awake()
		{
			FindObjectsWithClick.Get().Append(InsertClick);
		}

		private void OnDestroy()
		{
			FindObjectsWithClick.Get().Remove(InsertClick);
		}

		private bool InsertClick(Transform[] transforms)
		{
			if (PlayDesk.Get().IsAnimionBusy)
			{
				return false;
			}
			if (HandCardSystem.Get().IsStorageHand)
			{
				foreach (Transform transform in transforms)
				{
					IStorageHandGroup component = transform.gameObject.GetComponent<IStorageHandGroup>();
					if (component != null)
					{
						OperatingHelper.Get().ClearStep();
						AudioUtility.GetSound().Play("Audios/open_Poker.mp3");
						if (_baseCard != null)
						{
							HandCardSystem.Get().AppendRightCardNormal(_baseCard);
							_baseCard = null;
						}
						else
						{
							BaseCard baseCard = _baseCard = HandCardSystem.Get().FlyRightCard();
							_baseCard.UpdateOrderLayer(1000);
							_baseCard.transform.SetParent(base.transform, worldPositionStays: true);
							_baseCard.transform.DOMove(base.transform.position, 0.5f);
						}
						onChanged.Invoke();
						return true;
					}
				}
			}
			return false;
		}

		public void DONext()
		{
			if ((bool)_baseCard)
			{
				int suit = _baseCard.GetSuit();
				int number = _baseCard.GetNumber();
				number = (number + 13 + 1) % 13 + 1;
				_baseCard.SetIndex(number + suit * 13);
			}
		}

		public void Undo()
		{
		}

		public void Over()
		{
			base.transform.DOLocalMoveY(-8.1f, 0.5f);
		}

		public Transform GetTransform()
		{
			return base.transform;
		}

		public UnityEvent Changed()
		{
			return onChanged;
		}
	}
}
