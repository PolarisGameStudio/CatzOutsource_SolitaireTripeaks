using UnityEngine;

namespace Nightingale.Extensions
{
	public class AutomaticRotation : MonoBehaviour
	{
		public Vector3 AngleAndSpeed = new Vector3(0f, 0f, 300f);

		private void Update()
		{
			base.transform.localEulerAngles -= AngleAndSpeed * Time.deltaTime;
		}
	}
}
