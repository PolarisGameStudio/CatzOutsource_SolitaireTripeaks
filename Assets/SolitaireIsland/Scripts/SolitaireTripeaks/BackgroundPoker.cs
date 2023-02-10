using UnityEngine;

namespace SolitaireTripeaks
{
	public class BackgroundPoker : Poker
	{
		public SpriteRenderer BackgroundRenderer;

		private void Awake()
		{
			if (BackgroundRenderer != null)
			{
				BackgroundRenderer.sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite("back");
			}
		}
	}
}
