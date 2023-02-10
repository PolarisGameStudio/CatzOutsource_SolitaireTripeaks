using System;
using System.Collections.Generic;

namespace DragonBones
{
	public abstract class BaseObject
	{
		private static uint _hashCode = 0u;

		private static uint _defaultMaxCount = 3000u;

		private static readonly Dictionary<Type, uint> _maxCountMap = new Dictionary<Type, uint>();

		private static readonly Dictionary<Type, List<BaseObject>> _poolsMap = new Dictionary<Type, List<BaseObject>>();

		public readonly uint hashCode = _hashCode++;

		private static void _ReturnObject(BaseObject obj)
		{
			Type type = obj.GetType();
			uint num = (!_maxCountMap.ContainsKey(type)) ? _defaultMaxCount : _maxCountMap[type];
			object list;
			if (_poolsMap.ContainsKey(type))
			{
				list = _poolsMap[type];
			}
			else
			{
				List<BaseObject> list2 = new List<BaseObject>();
				_poolsMap[type] = list2;
				list = list2;
			}
			List<BaseObject> list3 = (List<BaseObject>)list;
			if (list3.Count < num)
			{
				if (!list3.Contains(obj))
				{
					list3.Add(obj);
				}
				else
				{
					Helper.Assert(condition: false, "The object is already in the pool.");
				}
			}
		}

		public static void SetMaxCount(Type classType, uint maxCount)
		{
			if (classType != null)
			{
				if (_poolsMap.ContainsKey(classType))
				{
					List<BaseObject> list = _poolsMap[classType];
					if (list.Count > maxCount)
					{
						list.ResizeList((int)maxCount);
					}
				}
				_maxCountMap[classType] = maxCount;
			}
			else
			{
				_defaultMaxCount = maxCount;
				foreach (Type key in _poolsMap.Keys)
				{
					List<BaseObject> list2 = _poolsMap[key];
					if (list2.Count > maxCount)
					{
						list2.ResizeList((int)maxCount);
					}
					if (_maxCountMap.ContainsKey(key))
					{
						_maxCountMap[key] = maxCount;
					}
				}
			}
		}

		public static void ClearPool(Type classType)
		{
			if (classType != null)
			{
				if (_poolsMap.ContainsKey(classType))
				{
					_poolsMap[classType]?.Clear();
				}
			}
			else
			{
				foreach (KeyValuePair<Type, List<BaseObject>> item in _poolsMap)
				{
					_poolsMap[item.Key]?.Clear();
				}
			}
		}

		public static T BorrowObject<T>() where T : BaseObject, new()
		{
			Type typeFromHandle = typeof(T);
			List<BaseObject> list = (!_poolsMap.ContainsKey(typeFromHandle)) ? null : _poolsMap[typeFromHandle];
			if (list != null && list.Count > 0)
			{
				int index = list.Count - 1;
				BaseObject baseObject = list[index];
				list.RemoveAt(index);
				return (T)baseObject;
			}
			T result = new T();
			result._OnClear();
			return result;
		}

		protected abstract void _OnClear();

		public void ReturnToPool()
		{
			_OnClear();
			_ReturnObject(this);
		}
	}
}
