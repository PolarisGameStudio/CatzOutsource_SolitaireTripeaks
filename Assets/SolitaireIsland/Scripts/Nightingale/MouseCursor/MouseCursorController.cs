using UnityEngine;

namespace Nightingale.MouseCursor
{
	public class MouseCursorController : MonoBehaviour
	{
		public Texture2D _MouseUp;

		public Texture2D _MouseDown;

		private void Update()
		{
			if (Cursor.visible)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Cursor.SetCursor(_MouseDown, Vector2.zero, CursorMode.Auto);
				}
				else if (Input.GetMouseButtonUp(0))
				{
					Cursor.SetCursor(_MouseUp, Vector2.zero, CursorMode.Auto);
				}
			}
		}
	}
}
