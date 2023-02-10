using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DownloadProgress
	{
		public string path;

		public float progress;

		public bool isCompleted;
	}
}
