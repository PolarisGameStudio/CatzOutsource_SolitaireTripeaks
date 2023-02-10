using Nightingale.Utilitys;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	public class LineCurve : MaskableGraphic
	{
		public float m_LineWidth = 1f;

		public float smoothness = 10f;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			vh.Clear();
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).localPosition);
			}
			list = MathUtility.MakeSmoothCurve(list, smoothness);
			for (int j = 1; j < list.Count; j++)
			{
				vh.AddUIVertexQuad(GenerateQuad(list[j - 1], list[j]));
			}
			for (int k = 0; k < vh.currentVertCount - 4; k += 4)
			{
				vh.AddTriangle(k + 1, k + 2, k + 4);
				vh.AddTriangle(k + 1, k + 2, k + 7);
			}
			UnityEngine.Debug.Log("PopulateMesh..." + vh.currentVertCount);
		}

		private UIVertex[] GenerateQuad(Vector2 pos1, Vector2 pos2)
		{
			float num = Vector2.Distance(pos1, pos2);
			float num2 = m_LineWidth * 0.5f * (pos2.x - pos1.x) / num;
			float num3 = m_LineWidth * 0.5f * (pos2.y - pos1.y) / num;
			if (num2 <= 0f)
			{
				num2 = 0f - num2;
			}
			else
			{
				num3 = 0f - num3;
			}
			UIVertex[] array = new UIVertex[4];
			array[0].position = new Vector3(pos1.x + num3, pos1.y + num2);
			array[1].position = new Vector3(pos2.x + num3, pos2.y + num2);
			array[2].position = new Vector3(pos2.x - num3, pos2.y - num2);
			array[3].position = new Vector3(pos1.x - num3, pos1.y - num2);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
			return array;
		}
	}
}
