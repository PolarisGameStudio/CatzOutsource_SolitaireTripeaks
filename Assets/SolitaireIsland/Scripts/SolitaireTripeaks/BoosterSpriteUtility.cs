using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class BoosterSpriteUtility : MonoBehaviour
	{
		public List<BoosterSprite> sprites;

		public Sprite GetSprite(BoosterType boosterType)
		{
			return sprites.Find((BoosterSprite e) => e.boosterType == boosterType)?.sprite;
		}
	}
}
