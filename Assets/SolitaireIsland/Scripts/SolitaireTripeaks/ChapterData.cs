using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class ChapterData
	{
		public List<LevelData> lvs;

		public int playLevel;

		public ChapterData()
		{
			lvs = new List<LevelData>();
		}

		public LevelData GetData(int level)
		{
			if (level >= lvs.Count)
			{
				return null;
			}
			return lvs[level];
		}

		public RecordDataType PutData(int level, LevelData levelData)
		{
			LevelData data = GetData(level);
			if (data == null)
			{
				lvs.Add(levelData);
				return RecordDataType.FirstRecord;
			}
			RecordDataType result = RecordDataType.Normal;
			if (levelData.StarComplete && !data.StarComplete)
			{
				data.StarComplete = true;
				result = RecordDataType.NewRecord;
			}
			if (levelData.StarSteaks && !data.StarSteaks)
			{
				data.StarSteaks = true;
				result = RecordDataType.NewRecord;
			}
			if (levelData.StarTime && !data.StarTime)
			{
				data.StarTime = true;
				result = RecordDataType.NewRecord;
			}
			return result;
		}
	}
}
