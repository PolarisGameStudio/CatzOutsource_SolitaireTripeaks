using DG.Tweening;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class BoatAnimationUI : MonoBehaviour
	{
		private void Start()
		{
			float num = UnityEngine.Random.Range(3, 7);
			float duration = UnityEngine.Random.Range(1.5f, 2.5f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DORotate(new Vector3(0f, 0f, (UnityEngine.Random.Range(0, 100) % 2 != 0) ? (0f - num) : num), duration));
			sequence.Append(base.transform.DORotate(Vector3.zero, duration));
			sequence.SetLoops(-1);
			sequence.SetEase(Ease.Linear);
			duration = UnityEngine.Random.Range(1.5f, 2.5f);
			num = UnityEngine.Random.Range(8, 15);
			sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOLocalMoveY((UnityEngine.Random.Range(0, 100) % 2 != 0) ? (0f - num) : num, duration));
			sequence.Append(base.transform.DOLocalMoveY(0f, duration));
			sequence.SetLoops(-1);
			sequence.SetEase(Ease.Linear);
		}
	}
}
