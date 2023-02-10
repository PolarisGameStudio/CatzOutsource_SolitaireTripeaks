using UnityEngine;

namespace Nightingale.Utilitys
{
	public class AssetData
	{
		public string scene;

		public string path;

		public Object asset;

		public bool IsMatch(string path)
		{
			return this.path.Equals(path);
		}

		public bool IsMatch(string scene, string path)
		{
			return this.scene.Equals(scene) && this.path.Equals(path);
		}
	}
}
