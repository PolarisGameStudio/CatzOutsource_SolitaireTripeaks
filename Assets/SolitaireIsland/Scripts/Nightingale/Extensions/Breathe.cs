using DG.Tweening;
using UnityEngine;

namespace Nightingale.Extensions
{
	public class Breathe : MonoBehaviour
	{
		public float minScaleX = 0.97f;

		public float maxScaleX = 1.03f;

		public float minScaleY = 0.98f;

		public float maxScaleY = 1.02f;

		private void Start()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(base.transform.DOScaleX(minScaleX, 0.2f));
			sequence.Join(base.transform.DOScaleY(maxScaleY, 0.2f));
			sequence.Append(base.transform.DOScaleX(maxScaleX, 0.4f));
			sequence.Join(base.transform.DOScaleY(minScaleY, 0.4f));
			sequence.Append(base.transform.DOScaleX(1f, 0.2f));
			sequence.Join(base.transform.DOScaleY(1f, 0.2f));
			sequence.SetLoops(-1);
		}
	}
}
