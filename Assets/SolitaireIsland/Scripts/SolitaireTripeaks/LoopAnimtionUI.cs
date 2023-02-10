using UnityEngine;

namespace SolitaireTripeaks
{
	public class LoopAnimtionUI : MonoBehaviour
	{
		public Vector3 Radius;

		private Vector3 localPosition;

		private void Awake()
		{
			localPosition = base.transform.localPosition;
		}

		private void Update()
		{
			base.transform.localPosition = new Vector3(localPosition.x + Mathf.Sin(Time.timeSinceLevelLoad * Radius.z) * Radius.x, localPosition.y + Mathf.Cos(Time.timeSinceLevelLoad * Radius.z) * Radius.y, 0f);
		}
	}
}
