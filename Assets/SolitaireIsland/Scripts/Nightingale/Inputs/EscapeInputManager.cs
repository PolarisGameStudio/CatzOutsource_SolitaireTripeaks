using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Inputs
{
	public class EscapeInputManager : SingletonBehaviour<EscapeInputManager>
	{
		private List<PressBackEventHandler> normals = new List<PressBackEventHandler>();

		private List<PressBackEventHandler> tops = new List<PressBackEventHandler>();

		private List<string> busys = new List<string>();

		public bool IsBusying => busys.Count > 0;

		private void Update()
		{
			if (!IsBusying && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				foreach (PressBackEventHandler top in tops)
				{
					if (top())
					{
						return;
					}
				}
				foreach (PressBackEventHandler normal in normals)
				{
					if (normal())
					{
						break;
					}
				}
			}
		}

		public void Append(PressBackEventHandler handler)
		{
			normals.Insert(0, handler);
		}

		public void InsertTop(PressBackEventHandler handler)
		{
			tops.Insert(0, handler);
		}

		public void Remove(PressBackEventHandler handler)
		{
			if (normals.Contains(handler))
			{
				normals.Remove(handler);
			}
			if (tops.Contains(handler))
			{
				tops.Remove(handler);
			}
		}

		public void Clear()
		{
			normals.Clear();
		}

		public void AppendKey(string key)
		{
			if (!busys.Contains(key))
			{
				busys.Add(key);
			}
		}

		public void RemoveKey(string key)
		{
			if (busys.Contains(key))
			{
				busys.Remove(key);
			}
		}

		public void ClearKey()
		{
			busys.Clear();
		}
	}
}
