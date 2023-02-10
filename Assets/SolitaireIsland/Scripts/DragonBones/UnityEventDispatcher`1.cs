using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragonBones
{
	public class UnityEventDispatcher<T> : MonoBehaviour
	{
		private readonly Dictionary<string, ListenerDelegate<T>> _listeners = new Dictionary<string, ListenerDelegate<T>>();

		public void DispatchEvent(string type, T eventObject)
		{
			if (_listeners.ContainsKey(type))
			{
				_listeners[type](type, eventObject);
			}
		}

		public bool HasEventListener(string type)
		{
			return _listeners.ContainsKey(type);
		}

		public void AddEventListener(string type, ListenerDelegate<T> listener)
		{
			if (_listeners.ContainsKey(type))
			{
				Delegate[] invocationList = _listeners[type].GetInvocationList();
				int i = 0;
				for (int num = invocationList.Length; i < num; i++)
				{
					if (listener == invocationList[i] as ListenerDelegate<T>)
					{
						return;
					}
				}
				Dictionary<string, ListenerDelegate<T>> listeners;
				string key;
				(listeners = _listeners)[key = type] = (ListenerDelegate<T>)Delegate.Combine(listeners[key], listener);
			}
			else
			{
				_listeners.Add(type, listener);
			}
		}

		public void RemoveEventListener(string type, ListenerDelegate<T> listener)
		{
			if (!_listeners.ContainsKey(type))
			{
				return;
			}
			Delegate[] invocationList = _listeners[type].GetInvocationList();
			int i = 0;
			for (int num = invocationList.Length; i < num; i++)
			{
				if (listener == invocationList[i] as ListenerDelegate<T>)
				{
					Dictionary<string, ListenerDelegate<T>> listeners;
					string key;
					(listeners = _listeners)[key = type] = (ListenerDelegate<T>)Delegate.Remove(listeners[key], listener);
					break;
				}
			}
			if (_listeners[type] == null)
			{
				_listeners.Remove(type);
			}
		}
	}
}
