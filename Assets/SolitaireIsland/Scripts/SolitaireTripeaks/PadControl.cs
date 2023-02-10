using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class PadControl : MonoBehaviour
	{
		public CanvasScaler CanvasScaler;

		[Range(0f, 1f)]
		public float matchWidthOrHeight;

		private void Start()
		{
		}
	}
}
