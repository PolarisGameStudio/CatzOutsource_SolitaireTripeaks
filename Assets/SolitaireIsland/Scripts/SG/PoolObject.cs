using UnityEngine;

namespace SG
{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class PoolObject : MonoBehaviour
	{
		public string poolName;

		public bool isPooled;
	}
}
