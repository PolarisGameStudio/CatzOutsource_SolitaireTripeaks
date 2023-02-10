using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[AddComponentMenu("UI/Effects/Text Vertical Three Gradient Color")]
	[RequireComponent(typeof(Text))]
	public class TextVerticalGradientColor : BaseMeshEffect
	{
		public Color colorTop = Color.white;

		public Color colorCenter = Color.grey;

		public Color colorBottom = Color.black;

		public bool MultiplyTextColor;

		protected TextVerticalGradientColor()
		{
		}

		public static Color32 Multiply(Color32 a, Color32 b)
		{
			a.r = (byte)(a.r * b.r >> 8);
			a.g = (byte)(a.g * b.g >> 8);
			a.b = (byte)(a.b * b.b >> 8);
			a.a = (byte)(a.a * b.a >> 8);
			return a;
		}

		private void ModifyVertices(VertexHelper vh)
		{
			List<UIVertex> list = new List<UIVertex>(vh.currentVertCount);
			vh.GetUIVertexStream(list);
			vh.Clear();
			int num = 6;
			for (int i = 0; i < list.Count; i += num)
			{
				UIVertex v = multiplyColor(list[i], colorTop);
				UIVertex v2 = multiplyColor(list[i + 1], colorTop);
				UIVertex v3 = multiplyColor(list[i + 4], colorBottom);
				UIVertex v4 = multiplyColor(list[i + 3], colorBottom);
				UIVertex v5 = calcCenterVertex(list[i], list[i + 4]);
				UIVertex v6 = calcCenterVertex(list[i + 1], list[i + 2]);
				vh.AddVert(v);
				vh.AddVert(v2);
				vh.AddVert(v6);
				vh.AddVert(v6);
				vh.AddVert(v5);
				vh.AddVert(v);
				vh.AddVert(v5);
				vh.AddVert(v6);
				vh.AddVert(v4);
				vh.AddVert(v4);
				vh.AddVert(v3);
				vh.AddVert(v5);
			}
			for (int j = 0; j < vh.currentVertCount; j += 12)
			{
				vh.AddTriangle(j, j + 1, j + 2);
				vh.AddTriangle(j + 3, j + 4, j + 5);
				vh.AddTriangle(j + 6, j + 7, j + 8);
				vh.AddTriangle(j + 9, j + 10, j + 11);
			}
		}

		private UIVertex multiplyColor(UIVertex vertex, Color color)
		{
			if (MultiplyTextColor)
			{
				vertex.color = Multiply(vertex.color, color);
			}
			else
			{
				vertex.color = color;
			}
			return vertex;
		}

		private UIVertex calcCenterVertex(UIVertex top, UIVertex bottom)
		{
			UIVertex result = default(UIVertex);
			result.normal = (top.normal + bottom.normal) / 2f;
			result.position = (top.position + bottom.position) / 2f;
			result.tangent = (top.tangent + bottom.tangent) / 2f;
			result.uv0 = (top.uv0 + bottom.uv0) / 2f;
			result.uv1 = (top.uv1 + bottom.uv1) / 2f;
			if (MultiplyTextColor)
			{
				Color c = Color.Lerp(top.color, bottom.color, 0.5f);
				result.color = Multiply(c, colorCenter);
			}
			else
			{
				result.color = colorCenter;
			}
			return result;
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				ModifyVertices(vh);
			}
		}
	}
}
