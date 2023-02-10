namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	// dummy code, just to add some rotation to the cube from example scene
	[AddComponentMenu("")]
	public class InfiniteRotator : MonoBehaviour
	{
		[Range(1f, 100f)]
		public float speed = 5f;

		private void Update()
		{
			transform.Rotate(0, speed * Time.deltaTime, 0);
		}
	}
}