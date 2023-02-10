using System.Collections.Generic;
using UnityEngine;

namespace SolitaireTripeaks
{
	public class LevelStarGroupUI : MonoBehaviour
	{
		public List<LevelStarUI> LevelStars;

		public void SetVisable(bool visable)
		{
			base.gameObject.SetActive(visable);
		}

		public void SetLevelData(LevelData levelData)
		{
			SetVisable(visable: true);
			if (levelData == null)
			{
				foreach (LevelStarUI levelStar in LevelStars)
				{
					levelStar.SetVisable(visable: false);
				}
			}
			else
			{
				foreach (LevelStarUI levelStar2 in LevelStars)
				{
					levelStar2.SetLevelData(levelData);
				}
			}
		}

		public void SetLeaderBoardLevelData(LevelData levelData)
		{
			if (levelData == null)
			{
				SetVisable(visable: false);
				return;
			}
			SetVisable(visable: true);
			foreach (LevelStarUI levelStar in LevelStars)
			{
				levelStar.SetLevelData(levelData);
			}
		}

		public void SetAnimationLevelData(LevelData levelData)
		{
			SetVisable(visable: true);
			if (levelData == null)
			{
				foreach (LevelStarUI levelStar in LevelStars)
				{
					levelStar.SetVisable(visable: false);
				}
				return;
			}
			int num = 0;
			for (int i = 0; i < LevelStars.Count; i++)
			{
				if (LevelStars[i].SetAnimationLevelData(levelData, i, num))
				{
					num++;
				}
			}
		}

		public void SetLeaderBoardAnimationLevelData(LevelData lastLevelData, LevelData newLevelData)
		{
			if (lastLevelData == null)
			{
				SetVisable(visable: false);
				return;
			}
			SetVisable(visable: true);
			int num = 0;
			for (int i = 0; i < LevelStars.Count; i++)
			{
				if (LevelStars[i].SetLeaderBoardAnimationLevelData(lastLevelData, newLevelData, i, num))
				{
					num++;
				}
			}
		}
	}
}
