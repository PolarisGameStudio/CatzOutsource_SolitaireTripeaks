using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class ResourceManager : MonoBehaviour
	{
		private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

		private static ResourceManager mInstance;

		public static ResourceManager Instance
		{
			get
			{
				if (mInstance == null)
				{
					GameObject gameObject = new GameObject("ResourceManager", typeof(ResourceManager));
					mInstance = gameObject.GetComponent<ResourceManager>();
					if (Application.isPlaying)
					{
						Object.DontDestroyOnLoad(mInstance.gameObject);
					}
					else
					{
						UnityEngine.Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
					}
				}
				return mInstance;
			}
		}

		public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
		{
			if (!poolDict.ContainsKey(poolName))
			{
				GameObject gameObject = Resources.Load<GameObject>(poolName);
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
				}
				else
				{
					poolDict[poolName] = new Pool(poolName, gameObject, base.gameObject, size, type);
				}
			}
		}

		public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
		{
			GameObject result = null;
			if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
			{
				InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
			}
			if (poolDict.ContainsKey(poolName))
			{
				Pool pool = poolDict[poolName];
				result = pool.NextAvailableObject(autoActive);
			}
			return result;
		}

		public void ReturnObjectToPool(GameObject go)
		{
			PoolObject component = go.GetComponent<PoolObject>();
			if (!(component == null))
			{
				Pool value = null;
				if (poolDict.TryGetValue(component.poolName, out value))
				{
					value.ReturnObjectToPool(component);
				}
			}
		}

		public void ReturnTransformToPool(Transform t)
		{
			if (!(t == null))
			{
				t.gameObject.SetActive(value: false);
				t.SetParent(null, worldPositionStays: false);
				ReturnObjectToPool(t.gameObject);
			}
		}
	}
}
