using DG.Tweening;
using Nightingale.Utilitys;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class TreasureUI : MonoBehaviour
	{
		public GameObject DoubleTreasureGameObject;

		private void Awake()
		{
			DoubleTreasureGameObject.gameObject.SetActive(SingletonClass<DayActivityHelper>.Get().HasDayActivty(DayActivityType.DoubleTreause));
			if (DoubleTreasureGameObject.gameObject.activeSelf)
			{
				ScaleXY(DoubleTreasureGameObject.transform);
			}
		}

		private void ScaleXY(Transform transform)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOScaleX(1.2f, 0.3f));
			sequence.Join(transform.DOScaleY(1.3f, 0.3f));
			sequence.Append(transform.DOScaleX(1f, 0.3f));
			sequence.Join(transform.DOScaleY(1f, 0.3f));
			sequence.SetLoops(-1);
		}
	}
}
