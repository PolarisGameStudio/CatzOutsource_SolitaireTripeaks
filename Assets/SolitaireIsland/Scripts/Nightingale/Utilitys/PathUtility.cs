using System.Collections.Generic;

namespace Nightingale.Utilitys
{
	public class PathUtility
	{
		public static string[] GetFiles(string path, string[] files)
		{
			path = path.Replace("\\", "/");
			if (path[path.Length - 1] != '/')
			{
				path += "/";
			}
			List<string> list = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = files[i].Replace("\\", "/");
				if (files[i].StartsWith(path))
				{
					string text = files[i].Substring(path.Length);
					if (!text.Contains("/") && !list.Contains(text))
					{
						list.Add(text);
					}
				}
			}
			return list.ToArray();
		}

		public static string[] GetDirectories(string path, string[] files)
		{
			path = path.Replace("\\", "/");
			if (path[path.Length - 1] != '/')
			{
				path += "/";
			}
			List<string> list = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = files[i].Replace("\\", "/");
				if (!files[i].StartsWith(path))
				{
					continue;
				}
				string text = files[i].Substring(path.Length);
				if (text.Contains("/"))
				{
					text = text.Substring(0, text.IndexOf("/"));
					if (!list.Contains(text))
					{
						list.Add(text);
					}
				}
			}
			return list.ToArray();
		}

		public static string[] GetFiles(string baseFile)
		{
			return GetFiles(baseFile, FileAsynUtility.GetFiles(baseFile).ToArray());
		}

		public static string[] GetDirectories(string baseFile)
		{
			return GetDirectories(baseFile, FileAsynUtility.GetFiles(baseFile).ToArray());
		}
	}
}
