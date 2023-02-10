using UnityEngine;

namespace SolitaireTripeaks
{
	public abstract class Poker : MonoBehaviour
	{
		private SpriteRenderer[] Renderers;

		public void UpdateLayer(int layer)
		{
			if (Renderers == null)
			{
				Renderers = GetComponentsInChildren<SpriteRenderer>();
			}
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				int num = spriteRenderer.transform.GetSiblingIndex();
				if (num > 0)
				{
					num++;
				}
				spriteRenderer.sortingOrder = layer + num;
			}
		}

		public void UpdateColor(bool white)
		{
			if (Renderers == null)
			{
				Renderers = GetComponentsInChildren<SpriteRenderer>();
			}
			SpriteRenderer[] renderers = Renderers;
			foreach (SpriteRenderer spriteRenderer in renderers)
			{
				spriteRenderer.color = ((!white) ? Color.gray : Color.white);
			}
		}

		public void UpdateLayer(string layer = "Default")
		{
			if (Renderers == null)
			{
				Renderers = GetComponentsInChildren<SpriteRenderer>();
			}
			for (int i = 0; i < Renderers.Length; i++)
			{
				Renderers[i].sortingLayerName = layer;
			}
		}
	}
}
