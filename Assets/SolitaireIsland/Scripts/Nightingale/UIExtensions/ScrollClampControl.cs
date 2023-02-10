using UnityEngine;
using UnityEngine.UI;

namespace Nightingale.UIExtensions
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollClampControl : MonoBehaviour
	{
		private ScrollRect scrollRect;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			scrollRect.onValueChanged.AddListener(delegate(Vector2 pos)
			{
				scrollRect.horizontalNormalizedPosition = Mathf.Clamp(pos.x, -0.04f, 1.04f);
				scrollRect.verticalNormalizedPosition = Mathf.Clamp(pos.y, -0.04f, 1.04f);
			});
		}
	}
}
