using UnityEngine;

namespace SolitaireTripeaks
{
	public class PrefabLabel
	{
		public Vector3 position;

		public string prefab;

		public PrefabLabel(Vector3 position, string prefab)
		{
			this.position = position;
			this.prefab = prefab;
		}
	}
}
