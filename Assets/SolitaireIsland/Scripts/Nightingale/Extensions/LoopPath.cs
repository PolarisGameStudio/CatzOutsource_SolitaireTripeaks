using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Nightingale.Extensions
{
	public class LoopPath : MonoBehaviour
	{
		[Tooltip("运行的控制点")]
		public Transform[] points;

		[Tooltip("一圈时间")]
		public float duration = 0.5f;

		private void Start()
		{
			base.transform.localPosition = points[0].localPosition;
			base.transform.DOLocalPath((from e in points
				select e.localPosition).ToArray(), duration).SetLoops(-1).SetEase(Ease.Linear);
		}
	}
}
