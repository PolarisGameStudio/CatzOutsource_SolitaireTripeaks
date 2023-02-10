using System;

namespace SolitaireTripeaks
{
	[Serializable]
	public struct RequestClubData
	{
		public string clubName;

		public string clubIcon;

		public string clubId;

		public RequestClubData(string clubId, string clubName, string clubIcon)
		{
			this.clubName = clubName;
			this.clubId = clubId;
			this.clubIcon = clubIcon;
		}
	}
}
