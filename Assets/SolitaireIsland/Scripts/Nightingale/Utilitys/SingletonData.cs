using System;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class SingletonData<T> where T : class, new()
	{
		private static T _instance;

		public static T Get()
		{
			if (_instance == null)
			{
				_instance = GetData(PlayerPrefs.GetString(typeof(T).Name));
				if (_instance == null)
				{
					_instance = new T();
				}
			}
			return _instance;
		}

		public static T GetData(string vaule)
		{
			T result = (T)null;
			if (!string.IsNullOrEmpty(vaule))
			{
				try
				{
					result = JsonUtility.FromJson<T>(vaule);
					UnityEngine.Debug.LogWarningFormat("----{0}---- Normal Get User Data Successed!", typeof(T).Name);
					return result;
				}
				catch (Exception)
				{
					UnityEngine.Debug.LogWarningFormat("----{0}---- Normal Get User Data failed!", typeof(T).Name);
					try
					{
						result = JsonUtility.FromJson<T>(CompressUtility.DecompressString(vaule));
						UnityEngine.Debug.LogWarningFormat("----{0}---- Decompress Get User Data Successed!", typeof(T).Name);
						return result;
					}
					catch (Exception)
					{
						UnityEngine.Debug.LogWarningFormat("----{0}---- Decompress Get User Data failed!", typeof(T).Name);
						return result;
					}
				}
			}
			return result;
		}

		public void FlushData()
		{
			UnityEngine.Debug.Log("本地数据保存成功:" + typeof(T).Name);
			PlayerPrefs.SetString(typeof(T).Name, JsonUtility.ToJson(this));
			PlayerPrefs.Save();
		}
	}
}
