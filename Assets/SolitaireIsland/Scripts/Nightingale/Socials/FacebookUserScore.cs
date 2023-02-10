using System;

namespace Nightingale.Socials
{
	[Serializable]
	public class FacebookUserScore : FacebookUser
	{
		public int user_score;

		public FacebookUserScore()
		{
		}

		public FacebookUserScore(FacebookUser user)
		{
			if (user != null)
			{
				id = user.id;
				name = user.name;
				picture = user.picture;
			}
		}
	}
}
