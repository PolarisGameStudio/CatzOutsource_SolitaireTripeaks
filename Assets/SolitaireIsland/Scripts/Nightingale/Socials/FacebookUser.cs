using System;
using System.Collections.Generic;

namespace Nightingale.Socials
{
	[Serializable]
	public class FacebookUser
	{
		public string id;

		public string name;

		public string picture;

		public bool isPlayer;

		private static List<FacebookUser> users = new List<FacebookUser>();

		public static FacebookUser Parse(object userObject, bool isPlayer = true)
		{
			FacebookUser facebookUser = null;
			Dictionary<string, object> dictionary = userObject as Dictionary<string, object>;
			if (dictionary.ContainsKey("id"))
			{
				string _id = dictionary["id"].ToString();
				if (users.Count > 0)
				{
					facebookUser = users.Find((FacebookUser e) => e.id.Equals(_id));
				}
				if (facebookUser == null)
				{
					FacebookUser facebookUser2 = new FacebookUser();
					facebookUser2.id = _id;
					facebookUser = facebookUser2;
					users.Add(facebookUser);
				}
				facebookUser.isPlayer = isPlayer;
				facebookUser.name = dictionary["name"].ToString();
				if (dictionary.ContainsKey("picture"))
				{
					Dictionary<string, object> dictionary2 = dictionary["picture"] as Dictionary<string, object>;
					if (dictionary2.ContainsKey("data"))
					{
						dictionary2 = (dictionary2["data"] as Dictionary<string, object>);
						facebookUser.picture = dictionary2["url"].ToString();
					}
				}
			}
			if (facebookUser == null)
			{
				facebookUser = new FacebookUser();
			}
			return facebookUser;
		}
	}
}
