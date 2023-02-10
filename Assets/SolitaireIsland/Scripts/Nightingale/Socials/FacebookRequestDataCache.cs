using Nightingale.Utilitys;
using System;
using System.Collections.Generic;

namespace Nightingale.Socials
{
	[Serializable]
	public class FacebookRequestDataCache : SingletonData<FacebookRequestDataCache>
	{
		public List<string> ids;

		public FacebookRequestDataCache()
		{
			ids = new List<string>();
		}

		public void PutId(string id)
		{
			ids.Add(id);
		}

		public bool Contains(string id)
		{
			return ids.Contains(id);
		}
	}
}
