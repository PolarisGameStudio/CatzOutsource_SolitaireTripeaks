using UnityEngine;

namespace Nightingale.Extensions
{
	public static class SpriteExtension
	{
		public static Texture2D ToTexture(this byte[] buffer)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(buffer);
			return texture2D;
		}

		public static Sprite ToSprite(this byte[] buffer)
		{
			return buffer.ToTexture().ToSprite();
		}

		public static Sprite ToSprite(this Texture2D source)
		{
			if (source == null)
			{
				return null;
			}
			return Sprite.Create(source, new Rect(0f, 0f, source.width, source.height), new Vector2(0.5f, 0.5f));
		}

		public static Texture2D WriteTo(this Texture2D source, Texture2D dest)
		{
			int num = (dest.width - dest.width) / 2;
			int num2 = (dest.height - dest.height) / 2;
			for (int i = 0; i < source.width; i++)
			{
				for (int j = 0; j < source.height; j++)
				{
					Color pixel = source.GetPixel(i, j);
					pixel.a = 255f;
					dest.SetPixel(num + i, num2 + j, pixel);
				}
			}
			dest.Apply();
			return dest;
		}

		public static Sprite GetThumbnails(this Sprite sourceImage, int scale = 3)
		{
			return sourceImage.texture.GetThumbnails(scale).ToSprite();
		}

		public static Sprite GetThumbnails(this Sprite source, int width, int height)
		{
			return source.texture.GetThumbnails(width, height).ToSprite();
		}

		public static Texture2D GetThumbnails(this Texture2D source, int scale = 3)
		{
			return source.GetThumbnails(source.width / scale, source.height / scale);
		}

		public static Texture2D GetThumbnails(this Texture2D source, int width, int height)
		{
			width = Mathf.Min(width, source.width);
			height = Mathf.Min(height, source.height);
			Texture2D texture2D = new Texture2D(width, height);
			int num = source.width / width;
			int num2 = source.height / height;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Color color = Color.clear;
					if (i * num < source.width && j * num2 < source.height)
					{
						color = source.GetPixel(i * num, j * num2);
					}
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D GetCut(this Texture2D source, int width, int height)
		{
			float num = Mathf.Max(source.width / width, source.height / height);
			Texture2D texture2D = new Texture2D(width, height);
			int num2 = (int)(((float)source.width - (float)width * num) / 2f);
			int num3 = (int)(((float)source.height - (float)height * num) / 2f);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Color color = Color.white;
					int num4 = (int)((float)num2 + (float)i * num);
					int num5 = (int)((float)num3 + (float)j * num);
					if (num4 < source.width && num4 >= 0 && num5 < source.height && num5 >= 0)
					{
						color = source.GetPixel(num4, num5);
					}
					texture2D.SetPixel(i, j, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Sprite GetCut(this Sprite source, int width, int height)
		{
			return source.texture.GetCut(width, height).ToSprite();
		}
	}
}
