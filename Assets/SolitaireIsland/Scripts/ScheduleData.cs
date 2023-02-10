using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public struct ScheduleData
	{
		public int world;

		public int chapter;

		public int level;

		public static ScheduleData Empty => new ScheduleData(-1, -1, -1);

		public ScheduleData(int world, int chapter, int level)
		{
			this.world = world;
			this.chapter = chapter;
			this.level = level;
		}

		public ScheduleData(int world)
		{
			this.world = world;
			chapter = world;
			level = world;
		}

		public static ScheduleData Parse(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return Empty;
			}
			string[] array = s.Split('-');
			if (array.Length >= 3)
			{
				return new ScheduleData(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
			}
			return Empty;
		}

		public string ToJson()
		{
			return $"{world}-{chapter}-{level}";
		}

		public bool IsEmpty()
		{
			return Equals(Empty);
		}

		public bool Than(ScheduleData scheduleData)
		{
			if (world > scheduleData.world)
			{
				return true;
			}
			if (world < scheduleData.world)
			{
				return false;
			}
			if (chapter > scheduleData.chapter)
			{
				return true;
			}
			if (chapter < scheduleData.chapter)
			{
				return false;
			}
			return level > scheduleData.level;
		}
	}
}
