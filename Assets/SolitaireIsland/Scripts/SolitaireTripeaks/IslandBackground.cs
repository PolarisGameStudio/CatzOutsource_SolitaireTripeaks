using UnityEngine;
using UnityEngine.UI;

namespace SolitaireTripeaks
{
	public class IslandBackground : MonoBehaviour
	{
		public float SpeedX = 0.02f;

		public float SpeedY = 0.02f;

		public float Speed = 0.2f;

		private RawImage target;

		private void Start()
		{
			target = GetComponent<RawImage>();
			target.texture.wrapMode = TextureWrapMode.Clamp;
		}

		private void Update()
		{
			if (target != null)
			{
				target.uvRect = new Rect(Mathf.Cos(Time.timeSinceLevelLoad * Speed) * SpeedX, Mathf.Sin(Time.timeSinceLevelLoad * Speed) * SpeedY, 1f, 1f);
			}
		}
	}
}
