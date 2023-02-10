using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class MultipleBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static Dictionary<string, T> instances = new Dictionary<string, T>();

		public static T Get(string key)
		{
			if (!instances.ContainsKey(key))
			{
				GameObject gameObject = new GameObject(typeof(T).Name);
				T value = (T)gameObject.AddComponent(typeof(T));
				Object.DontDestroyOnLoad(gameObject);
				instances.Add(key, value);
			}
			return instances[key];
		}
	}
}
