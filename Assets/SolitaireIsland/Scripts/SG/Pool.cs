using System.Collections.Generic;
using UnityEngine;

namespace SG
{
	internal class Pool
	{
		private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

		private GameObject rootObj;

		private PoolInflationType inflationType;

		private string poolName;

		private int objectsInUse;

		public Pool(string poolName, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount, PoolInflationType type)
		{
			if (!(poolObjectPrefab == null))
			{
				this.poolName = poolName;
				inflationType = type;
				rootObj = new GameObject(poolName + "Pool");
				rootObj.transform.SetParent(rootPoolObj.transform, worldPositionStays: false);
				GameObject gameObject = Object.Instantiate(poolObjectPrefab);
				PoolObject poolObject = gameObject.GetComponent<PoolObject>();
				if (poolObject == null)
				{
					poolObject = gameObject.AddComponent<PoolObject>();
				}
				poolObject.poolName = poolName;
				AddObjectToPool(poolObject);
				populatePool(Mathf.Max(initialCount, 1));
			}
		}

		private void AddObjectToPool(PoolObject po)
		{
			po.gameObject.SetActive(value: false);
			po.gameObject.name = poolName;
			availableObjStack.Push(po);
			po.isPooled = true;
			po.gameObject.transform.SetParent(rootObj.transform, worldPositionStays: false);
		}

		private void populatePool(int initialCount)
		{
			for (int i = 0; i < initialCount; i++)
			{
				PoolObject po = Object.Instantiate(availableObjStack.Peek());
				AddObjectToPool(po);
			}
		}

		public GameObject NextAvailableObject(bool autoActive)
		{
			PoolObject poolObject = null;
			if (availableObjStack.Count > 1)
			{
				poolObject = availableObjStack.Pop();
			}
			else
			{
				int num = 0;
				if (inflationType == PoolInflationType.INCREMENT)
				{
					num = 1;
				}
				else if (inflationType == PoolInflationType.DOUBLE)
				{
					num = availableObjStack.Count + Mathf.Max(objectsInUse, 0);
				}
				if (num > 0)
				{
					populatePool(num);
					poolObject = availableObjStack.Pop();
				}
			}
			GameObject gameObject = null;
			if (poolObject != null)
			{
				objectsInUse++;
				poolObject.isPooled = false;
				gameObject = poolObject.gameObject;
				if (autoActive)
				{
					gameObject.SetActive(value: true);
				}
			}
			return gameObject;
		}

		public void ReturnObjectToPool(PoolObject po)
		{
			if (poolName.Equals(po.poolName))
			{
				objectsInUse--;
				if (!po.isPooled)
				{
					AddObjectToPool(po);
				}
			}
			else
			{
				UnityEngine.Debug.LogError($"Trying to add object to incorrect pool {po.poolName} {poolName}");
			}
		}
	}
}
