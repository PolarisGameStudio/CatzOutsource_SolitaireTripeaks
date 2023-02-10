using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.U2D
{
	public class SpriteManager : MonoBehaviour
	{
		public List<Sprite> Sprites;

		public Sprite GetSprite(string spriteName)
		{
			return Sprites.Find((Sprite e) => e.name.Equals(spriteName));
		}

		public static Sprite GetSprite(string group, string fileName, string spriteName)
		{
			SpriteManager assetComponent = SingletonBehaviour<LoaderUtility>.Get().GetAssetComponent<SpriteManager>(group, fileName);
			Sprite sprite = assetComponent.GetSprite(spriteName);
			if (sprite == null)
			{
				return assetComponent.Sprites[0];
			}
			return sprite;
		}
	}
}
