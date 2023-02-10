using Nightingale.Extensions;
using UnityEngine;

namespace Nightingale.Utilitys
{
	public class SingletonBehaviour<T> : DelayBehaviour where T : DelayBehaviour
	{
		private static T instance;

		public static T Get()
		{
			if ((Object)instance == (Object)null)
			{
				instance = (T)Object.FindObjectOfType(typeof(T));
				if ((Object)instance == (Object)null)
				{
					GameObject gameObject = new GameObject(typeof(T).Name);
					instance = (T)gameObject.AddComponent(typeof(T));
					Object.DontDestroyOnLoad(gameObject);
				}
			}
			return instance;
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(base.gameObject);
			instance = (T)null;
		}

		protected virtual void OnDestroy()
		{
			instance = (T)null;
		}
	}
}
