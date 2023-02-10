using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class WorldData
	{
		public List<ChapterData> chapters;

		public int world;

		public int playChapter;

		public WorldData()
		{
			chapters = new List<ChapterData>();
		}

		public ChapterData GetData(int chapter)
		{
			if (chapter >= chapters.Count)
			{
				return null;
			}
			return chapters[chapter];
		}

		public RecordDataType PutData(int chapter, int level, LevelData levelData)
		{
			ChapterData chapterData = GetData(chapter);
			if (chapterData == null)
			{
				chapterData = new ChapterData();
				chapters.Add(chapterData);
			}
			return chapterData.PutData(level, levelData);
		}

		public int GetLevels()
		{
			int levels = 0;
			chapters.ForEach(delegate(ChapterData chapter)
			{
				levels += chapter.lvs.Count;
			});
			return levels;
		}
	}
}
