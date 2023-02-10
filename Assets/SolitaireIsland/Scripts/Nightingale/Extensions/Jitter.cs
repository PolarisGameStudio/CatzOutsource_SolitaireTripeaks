using DG.Tweening;
using UnityEngine;

namespace Nightingale.Extensions
{
	public class Jitter : MonoBehaviour
	{
		private void Start()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOLocalMoveY(0.5f, 0.1f));
			sequence.Append(base.transform.DOLocalMoveY(1.5f, 0.1f));
			sequence.Append(base.transform.DOLocalMoveY(0.5f, 0.1f));
			sequence.SetLoops(-1);
		}
	}
}
