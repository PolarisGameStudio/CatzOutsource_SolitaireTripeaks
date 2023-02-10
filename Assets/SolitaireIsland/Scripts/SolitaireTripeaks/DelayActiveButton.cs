using DG.Tweening;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class DelayActiveButton : MonoBehaviour
	{
		public float DelayTime;

		private void Start()
		{
			base.transform.localScale = Vector3.zero;
			base.transform.gameObject.SetActive(value: false);
			Sequence s = DOTween.Sequence();
			s.AppendInterval(DelayTime);
			s.AppendCallback(delegate
			{
				base.transform.gameObject.SetActive(value: true);
			});
			s.Append(base.transform.DOScale(Vector3.one, 0.2f));
		}
	}
}
