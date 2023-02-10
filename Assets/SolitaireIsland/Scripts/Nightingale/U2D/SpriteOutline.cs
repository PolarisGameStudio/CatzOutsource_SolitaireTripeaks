using UnityEngine;

namespace Nightingale.U2D
{
	[ExecuteInEditMode]
	public class SpriteOutline : MonoBehaviour
	{
		public Color color = Color.white;

		[Range(0f, 16f)]
		public int outlineSize = 1;

		private SpriteRenderer spriteRenderer;

		private void OnEnable()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			UpdateOutline(outline: true);
		}

		private void OnDisable()
		{
			UpdateOutline(outline: false);
		}

		private void LateUpdate()
		{
			UpdateOutline(outline: true);
		}

		private void UpdateOutline(bool outline)
		{
			Sprite sprite = spriteRenderer.sprite;
			Vector2 min = sprite.textureRect.min;
			float x = min.x / (float)sprite.texture.width;
			Vector2 min2 = sprite.textureRect.min;
			float y = min2.y / (float)sprite.texture.height;
			Vector2 max = sprite.textureRect.max;
			float z = max.x / (float)sprite.texture.width;
			Vector2 max2 = sprite.textureRect.max;
			Vector4 value = new Vector4(x, y, z, max2.y / (float)sprite.texture.height);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			spriteRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_Outline", (!outline) ? 0f : 1f);
			materialPropertyBlock.SetColor("_OutlineColor", color);
			materialPropertyBlock.SetFloat("_OutlineSize", outlineSize);
			materialPropertyBlock.SetVector("_Rect", value);
			spriteRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
