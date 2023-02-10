using Nightingale.Utilitys;
using System;
using System.Collections.Generic;

namespace SolitaireTripeaks
{
	[Serializable]
	public class DeviceFileData : SingletonData<DeviceFileData>
	{
		public List<string> news;

		public DeviceFileData()
		{
			news = new List<string>();
		}

		public void Remove(string id)
		{
			if (news != null && news.Contains(id))
			{
				news.Remove(id);
			}
		}

		public void Remove()
		{
			string[] array = news.ToArray();
			foreach (string text in array)
			{
				if (DateTime.TryParse(text, out DateTime result) && DateTime.Today.Subtract(result).TotalMilliseconds > 0.0)
				{
					news.Remove(text);
				}
			}
		}

		public bool Contains(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			if (news == null)
			{
				return false;
			}
			return news.Contains(id);
		}

		public void Append(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				if (news == null)
				{
					news = new List<string>();
				}
				if (!news.Contains(id))
				{
					news.Add(id);
				}
			}
		}
	}
}
