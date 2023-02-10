using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;

namespace Nightingale.Debuggers
{
	public class Debugger : SingletonBehaviour<Debugger>
	{
		private List<string> clearContents = new List<string>();

		private List<string> contents = new List<string>();

		private Vector2 vector;

		public void WriteLine(string content)
		{
			clearContents.Add(content);
		}

		public void WriteLineStay(string content)
		{
			contents.Add(content);
		}

		private void LateUpdate()
		{
			clearContents.Clear();
		}

		private void OnGUI()
		{
			if (contents.Count > 0)
			{
				GUI.color = Color.black;
				vector = GUILayout.BeginScrollView(vector);
				foreach (string content in contents)
				{
					GUILayout.Label(content, new GUIStyle
					{
						fontSize = 50
					});
				}
				GUILayout.EndScrollView();
			}
		}
	}
}
