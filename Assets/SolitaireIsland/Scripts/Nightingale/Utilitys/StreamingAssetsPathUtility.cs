using System.IO;
using System.Linq;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class StreamingAssetsPathUtility : SingletonBehaviour<StreamingAssetsPathUtility>
	{
		private string[] paths;

		private void Awake()
		{
			paths = SingletonBehaviour<LoaderUtility>.Get().GetText("StreamingAssets.txt").Split('\n');
			int len = paths.Length;
			for(int counter = 0; counter < len; counter++)
            {
				paths[counter] = paths[counter].Trim();

			}
		}

		public bool Exists(string path)
		{
			Debug.Log($"StreamingAssetsPathUtility Exists path:{path}");
            if (Application.streamingAssetsPath.Contains("://"))
			{
				Debug.Log("StreamingAssetsPathUtility Exists 1");
                if (Path.HasExtension(path))
				{
					//foreach (var item in paths)
					//               {
					//	if (item.Trim().Equals(path.Trim())) return true;
					//               }
					//return false;

					return paths.Contains(path.Trim());
				}
                Debug.Log("StreamingAssetsPathUtility Exists 3");
                return paths.ToList().Find((string e) => e.StartsWith(path)) != null;
			}
			Debug.Log("StreamingAssetsPathUtility Exists 2");
            return FileAsynUtility.Exists(Application.streamingAssetsPath, path);
		}

		public static string StreamingAssetsPath(string namePath)
        {
			string path = namePath;
			
			//path = path.Replace('\\', '/');
			//if (namePath.StartsWith(@"blast/Android"))
   //         {
			//	path = path.Replace(@"blast/Android/" + (path.StartsWith(@"blast/Android/local/") ? @"local/" : @"remote/"), "");
			//}

            //Debug.Log("@LOG StreamingAssetsPath namePath:" + namePath);
#if UNITY_EDITOR
            path = Path.Combine(Application.streamingAssetsPath, path);
#elif UNITY_ANDROID
			path = "jar:file://" + Application.dataPath + "!/assets/" + path;
            //path = Path.Combine(@"jar:file://" + Application.dataPath + "!/assets", path);
            //path = Path.Combine(Application.dataPath + "!/assets", path);
#elif UNITY_IOS
			path = Application.dataPath + "/Raw/" + path;
            //path = Path.Combine(@"file://" + Application.dataPath + "/Raw", path);
            //path = Path.Combine(Application.dataPath + "/Raw", path);
#endif
			Debug.Log("@LOG StreamingAssetsPath path:" + path);
            return path;
		}
	}
}
