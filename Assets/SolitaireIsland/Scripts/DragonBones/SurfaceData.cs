using System.Collections.Generic;

namespace DragonBones
{
	public class SurfaceData : BoneData
	{
		public float vertexCountX;

		public float vertexCountY;

		public readonly List<float> vertices = new List<float>();

		protected override void _OnClear()
		{
			base._OnClear();
			vertexCountX = 0f;
			vertexCountY = 0f;
			vertices.Clear();
		}
	}
}
