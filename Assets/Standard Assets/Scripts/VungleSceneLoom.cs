using System;
using System.Collections.Generic;
using UnityEngine;

public class VungleSceneLoom : MonoBehaviour
{
	public interface ILoom
	{
		void QueueOnMainThread(Action action);
	}

	private class NullLoom : ILoom
	{
		public void QueueOnMainThread(Action action)
		{
		}
	}

	private class LoomDispatcher : ILoom
	{
		private readonly List<Action> actions = new List<Action>();

		public void QueueOnMainThread(Action action)
		{
			lock (actions)
			{
				actions.Add(action);
			}
		}

		public void Update()
		{
			Action[] array = null;
			lock (actions)
			{
				array = actions.ToArray();
				actions.Clear();
			}
			Action[] array2 = array;
			foreach (Action action in array2)
			{
				action();
			}
		}
	}

	private static NullLoom _nullLoom = new NullLoom();

	private static LoomDispatcher _loom;

	private static VungleSceneLoom _instance;

	private static bool _initialized = false;

	public static ILoom Loom
	{
		get
		{
			if (_loom != null)
			{
				return _loom;
			}
			return _nullLoom;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_loom = new LoomDispatcher();
	}

	public static void Initialize()
	{
		if (!_initialized)
		{
			GameObject gameObject = new GameObject("VungleSceneLoom");
			_instance = gameObject.AddComponent<VungleSceneLoom>();
			_initialized = true;
		}
	}

	private void OnDestroy()
	{
		_loom = null;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			_loom.Update();
		}
	}
}
