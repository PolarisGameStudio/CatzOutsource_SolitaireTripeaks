using UnityEngine.UI;

namespace Nightingale.Extensions
{
	public class NoDrawingRayCast : Graphic
	{
		public override void SetMaterialDirty()
		{
		}

		public override void SetVerticesDirty()
		{
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
