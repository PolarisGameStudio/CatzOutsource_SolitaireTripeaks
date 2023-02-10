using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[AddComponentMenu("UI/Effects/Text Vertical Two Gradient Color")]
	public class Gradient : BaseMeshEffect
	{
		[SerializeField]
		private Color32 topColor = new Color32(byte.MaxValue, 248, 141, byte.MaxValue);

		[SerializeField]
		private Color32 bottomColor = new Color32(byte.MaxValue, 162, 56, byte.MaxValue);

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
			}
		}

		public void ModifyVertices(List<UIVertex> vertexList)
		{
			if (!IsActive() || vertexList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < vertexList.Count; i += 6)
			{
				UIVertex uIVertex = vertexList[i];
				float num = uIVertex.position.y;
				float num2 = num;
				float num3 = 1f;
				for (int j = 1; j < 6; j++)
				{
					UIVertex uIVertex2 = vertexList[j + i];
					float y = uIVertex2.position.y;
					if (y > num2)
					{
						num2 = y;
					}
					else if (y < num)
					{
						num = y;
					}
				}
				num3 = num2 - num;
				for (int k = 0; k < 6; k++)
				{
					UIVertex value = vertexList[k + i];
					value.color = Color32.Lerp(bottomColor, topColor, (value.position.y - num) / num3);
					vertexList[k + i] = value;
				}
			}
		}
	}
}
