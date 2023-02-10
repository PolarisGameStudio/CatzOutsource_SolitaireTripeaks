using Nightingale.U2D;
using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class LevelStarIslandData
	{
		public DoubleSpriteUI doubleSpriteUI;

		public StarType starType;

		public void SetLevelData(LevelData levelData)
		{
			if (levelData == null)
			{
				SetActive(active: false);
				return;
			}
			SetActive(active: true);
			if (levelData.StarComplete && starType == StarType.Complete)
			{
				doubleSpriteUI.SetState(normal: true);
			}
			else if (levelData.StarTime && starType == StarType.Time)
			{
				doubleSpriteUI.SetState(normal: true);
			}
			else if (levelData.StarSteaks && starType == StarType.Streak)
			{
				doubleSpriteUI.SetState(normal: true);
			}
			else
			{
				doubleSpriteUI.SetState(normal: false);
			}
		}

		public void SetActive(bool active)
		{
			doubleSpriteUI.gameObject.SetActive(active);
		}
	}
}
