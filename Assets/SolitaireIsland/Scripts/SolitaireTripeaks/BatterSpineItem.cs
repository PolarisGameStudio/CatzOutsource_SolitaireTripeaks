using DG.Tweening;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class BatterSpineItem : MonoBehaviour
	{
		protected SpriteRenderer[] Renderers;

		private void Awake()
		{
			Renderers = base.transform.GetComponentsInChildren<SpriteRenderer>();
		}

		public void UpdateOrderLayer(int zIndex)
		{
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				spriteRenderer.sortingOrder = zIndex;
			}
		}

		public void PlayActivation()
		{
			Sequence s = DOTween.Sequence();
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				if (!(spriteRenderer.transform.localScale == Vector3.one))
				{
					spriteRenderer.transform.localScale = Vector3.zero;
					s.Join(spriteRenderer.transform.DOScale(1f, 0.5f));
				}
			}
		}

		public void PlayDestroy()
		{
			Sequence s = DOTween.Sequence();
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				if (!(spriteRenderer.transform.localScale == Vector3.zero))
				{
					spriteRenderer.transform.localScale = Vector3.one;
					s.Join(spriteRenderer.transform.DOScale(0f, 0.5f));
				}
			}
		}

		public void UpdateColor(bool white)
		{
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				spriteRenderer.color = ((!white) ? Color.gray : Color.white);
			}
		}
	}
}
