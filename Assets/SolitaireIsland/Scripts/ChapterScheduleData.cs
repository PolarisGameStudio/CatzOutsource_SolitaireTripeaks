using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public struct ChapterScheduleData
	{
		public int world;

		public int chapter;

		public ChapterScheduleData(int world, int chapter)
		{
			this.world = world;
			this.chapter = chapter;
		}
	}
}
