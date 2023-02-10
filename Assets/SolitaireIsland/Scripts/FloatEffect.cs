using DG.Tweening;
using UnityEngine;

public class FloatEffect : MonoBehaviour
{
	public float FloatDistance = 10f;

	public float FloatSpeed = 1.5f;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		Sequence s = sequence;
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		s.Append(transform.DOLocalMoveY(localPosition.y + FloatDistance, FloatSpeed));
		Sequence s2 = sequence;
		Transform transform2 = base.transform;
		Vector3 localPosition2 = base.transform.localPosition;
		s2.Append(transform2.DOLocalMoveY(localPosition2.y, FloatSpeed));
		sequence.SetEase(Ease.Linear);
		sequence.SetLoops(-1);
	}
}
