using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	[CreateAssetMenu(fileName = "LinkImageConfig.asset", menuName = "Nightingale/Link Image Config", order = 1)]
	public class LinkImageConfig : ScriptableObject
	{
		public Sprite defaultSprite;

		public List<Sprite> Sprites;

		private static LinkImageConfig config;

		public static LinkImageConfig GetDefault()
		{
			if (config == null)
			{
				config = SingletonBehaviour<LoaderUtility>.Get().GetAsset<LinkImageConfig>("Configs/LinkImageConfig");
			}
			return config;
		}

		public Sprite GetSprite(string fileName)
		{
			Sprite sprite = Sprites.Find((Sprite e) => e.name == fileName);
			if (sprite == null)
			{
				return defaultSprite;
			}
			return sprite;
		}
	}
}
