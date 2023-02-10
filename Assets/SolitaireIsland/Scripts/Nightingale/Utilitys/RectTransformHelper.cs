using UnityEngine;

namespace Nightingale.Utilitys
{
	public class RectTransformHelper
	{
		public static void Center(RectTransform rectTransform, CenterDir centerDir = CenterDir.Both)
		{
			Center(rectTransform, rectTransform.parent as RectTransform, centerDir);
		}

		public static void Center(RectTransform rectTransform, RectTransform contentTransform, CenterDir centerDir = CenterDir.Both)
		{
			RectTransform rectTransform2 = contentTransform.parent.parent as RectTransform;
			Vector3 a = rectTransform2.position + (Vector3)rectTransform2.rect.center;
			Vector3 position = rectTransform.position;
			Vector3 b = a - position;
			b.z = 0f;
			Vector3 position2 = contentTransform.position + b;
			switch (centerDir)
			{
			case CenterDir.Horizontal:
			{
				Vector3 position4 = contentTransform.position;
				position2.y = position4.y;
				break;
			}
			case CenterDir.Vertical:
			{
				Vector3 position3 = contentTransform.position;
				position2.x = position3.x;
				break;
			}
			}
			contentTransform.position = position2;
		}
	}
}
