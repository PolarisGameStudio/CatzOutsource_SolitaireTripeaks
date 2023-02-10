using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[RequireComponent(typeof(Text), typeof(RectTransform))]
	[AddComponentMenu("UI/Effects/Extensions/Curved Text")]
	[ExecuteInEditMode]
	public class CurvedText : BaseMeshEffect
	{
		public AnimationCurve curveForText = new AnimationCurve();

		private RectTransform rectTrans;

		protected override void Awake()
		{
			base.Awake();
			rectTrans = GetComponent<RectTransform>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			rectTrans = GetComponent<RectTransform>();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			int currentVertCount = vh.currentVertCount;
			if (IsActive() && currentVertCount != 0)
			{
				for (int i = 0; i < vh.currentVertCount; i++)
				{
					UIVertex vertex = default(UIVertex);
					vh.PopulateUIVertex(ref vertex, i);
					ref Vector3 position = ref vertex.position;
					float y = position.y;
					AnimationCurve animationCurve = curveForText;
					float width = rectTrans.rect.width;
					Vector2 pivot = rectTrans.pivot;
					position.y = y + animationCurve.Evaluate((width * pivot.x + vertex.position.x) / rectTrans.rect.width) * rectTrans.rect.height;
					vh.SetUIVertex(vertex, i);
				}
			}
		}
	}
}
