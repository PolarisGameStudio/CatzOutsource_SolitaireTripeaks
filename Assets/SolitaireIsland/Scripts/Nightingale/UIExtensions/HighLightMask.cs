using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	public class HighLightMask : Graphic, ICanvasRaycastFilter
	{
		public Camera uiCamera;

		public RectTransform targetRectTransform;

		private Vector3 _targetPosition = Vector3.zero;

		private Vector2 _targetCenter = Vector2.zero;

		private Vector2 _targetSize = Vector2.zero;

		private bool _isValid;

		private void Update()
		{
			if (uiCamera == null || uiCamera.gameObject == null || targetRectTransform == null || targetRectTransform.gameObject == null || !targetRectTransform.gameObject.activeInHierarchy)
			{
				if (_isValid)
				{
					_isValid = false;
					SetAllDirty();
				}
				return;
			}
			if (!_isValid)
			{
				_isValid = true;
				SetAllDirty();
			}
			if (_targetPosition != targetRectTransform.position || _targetSize != targetRectTransform.rect.size)
			{
				_targetPosition = targetRectTransform.position;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, RectTransformUtility.WorldToScreenPoint(uiCamera, _targetPosition), uiCamera, out _targetCenter);
				_targetSize = targetRectTransform.rect.size;
				SetAllDirty();
			}
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			if (targetRectTransform == null || targetRectTransform.gameObject == null || !targetRectTransform.gameObject.activeInHierarchy)
			{
				return false;
			}
			bool flag = RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, sp, eventCamera);
			return !flag;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (_isValid)
			{
				Vector2 pivot = base.rectTransform.pivot;
				float x = (0f - pivot.x) * base.rectTransform.rect.width;
				Vector2 pivot2 = base.rectTransform.pivot;
				float y = (0f - pivot2.y) * base.rectTransform.rect.height;
				Vector2 pivot3 = base.rectTransform.pivot;
				float z = (1f - pivot3.x) * base.rectTransform.rect.width;
				Vector2 pivot4 = base.rectTransform.pivot;
				Vector4 vector = new Vector4(x, y, z, (1f - pivot4.y) * base.rectTransform.rect.height);
				Vector4 vector2 = new Vector4(_targetCenter.x - _targetSize.x / 2f, _targetCenter.y - _targetSize.y / 2f, _targetCenter.x + _targetSize.x * 0.5f, _targetCenter.y + _targetSize.y * 0.5f);
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.position = new Vector2(vector.x, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector.x, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector2.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector2.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector.z, vector.w);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector.z, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.x, vector2.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector2.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				simpleVert.position = new Vector2(vector2.z, vector.y);
				simpleVert.color = color;
				vh.AddVert(simpleVert);
				vh.AddTriangle(0, 1, 2);
				vh.AddTriangle(2, 3, 0);
				vh.AddTriangle(4, 5, 6);
				vh.AddTriangle(6, 7, 4);
				vh.AddTriangle(8, 9, 10);
				vh.AddTriangle(10, 11, 8);
				vh.AddTriangle(12, 13, 14);
				vh.AddTriangle(14, 15, 12);
			}
		}
	}
}
