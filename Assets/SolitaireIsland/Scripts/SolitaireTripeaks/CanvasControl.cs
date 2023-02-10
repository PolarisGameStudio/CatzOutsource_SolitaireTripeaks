using UnityEngine;

namespace SolitaireTripeaks
{
	public class CanvasControl : MonoBehaviour
	{
		private Canvas canvas;

		public float planeDistance = 100f;

		private void Awake()
		{
			canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = Camera.main;
			canvas.planeDistance = planeDistance;
			UnityEngine.Object.Destroy(this);
		}
	}
}
