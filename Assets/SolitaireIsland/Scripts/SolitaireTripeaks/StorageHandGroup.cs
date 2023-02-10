using DG.Tweening;
using Nightingale.Inputs;
using Nightingale.Utilitys;
using UnityEngine;
using UnityEngine.Events;

namespace SolitaireTripeaks
{
	public class StorageHandGroup : MonoBehaviour, IStorageHandGroup
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
				BaseCard top = HandCardSystem.Get()._RightHandGroup.GetTop();
				if ((bool)top)
				{
					foreach (Transform transform in transforms)
					{
						IStorageHandGroup component = transform.gameObject.GetComponent<IStorageHandGroup>();
						if (component != null)
						{
							AudioUtility.GetSound().Play("Audios/open_Poker.mp3");
							OperatingHelper.Get().ClearStep();
							BaseCard baseCard;
							if (_baseCard != null)
							{
								baseCard = HandCardSystem.Get()._RightHandGroup.FlyCard();
								HandCardSystem.Get().AppendRightCardNormal(_baseCard);
							}
							else
							{
								baseCard = HandCardSystem.Get().FlyRightCard();
							}
							_baseCard = baseCard;
							_baseCard.UpdateOrderLayer(1000);
							_baseCard.transform.SetParent(base.transform, worldPositionStays: true);
							_baseCard.transform.DOMove(base.transform.position, 0.5f);
							onChanged.Invoke();
							return true;
						}
					}
				}
			}
			return false;
		}

		public void DONext()
		{
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
