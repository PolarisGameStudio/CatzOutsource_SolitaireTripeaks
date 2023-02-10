using System.Collections.Generic;

namespace DragonBones
{
	public class PathDisplayData : DisplayData
	{
		public bool closed;

		public bool constantSpeed;

		public readonly VerticesData vertices = new VerticesData();

		public readonly List<float> curveLengths = new List<float>();

		protected override void _OnClear()
		{
			base._OnClear();
			type = DisplayType.Path;
			closed = false;
			constantSpeed = false;
			vertices.Clear();
			curveLengths.Clear();
		}
	}
}
