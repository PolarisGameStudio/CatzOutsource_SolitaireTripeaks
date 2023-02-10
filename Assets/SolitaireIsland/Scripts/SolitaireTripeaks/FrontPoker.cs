using UnityEngine;

namespace SolitaireTripeaks
{
	public class FrontPoker : Poker
	{
		public SpriteRenderer BackgroundRenderer;

		public SpriteRenderer NumberRenderer;

		public SpriteRenderer SuitRenderer;

		public SpriteRenderer MiniNumberRenderer;

		public void UpdateNumber(int index)
		{
			if (BackgroundRenderer != null)
			{
				BackgroundRenderer.sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite("front");
			}
			int num = (index - 1) / 13 + 1;
			int num2 = (index - 1) % 13 + 1 + (num - 1) % 2 * 13;
			if (SuitRenderer != null)
			{
				SuitRenderer.sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite($"Suit_{num}");
			}
			if (NumberRenderer != null)
			{
				NumberRenderer.sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite($"{num2}");
			}
			if (MiniNumberRenderer != null)
			{
				MiniNumberRenderer.sprite = PokerThemeGroup.Get().GetSpriteManager().GetSprite($"{num2}");
			}
		}

		public void SetSuitVisable(bool visable)
		{
			if (SuitRenderer != null)
			{
				SuitRenderer.gameObject.SetActive(visable);
			}
			if (MiniNumberRenderer != null)
			{
				MiniNumberRenderer.gameObject.SetActive(visable);
			}
		}
	}
}
