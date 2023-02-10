using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Loop Vertical Scroll Rect", 51)]
	[DisallowMultipleComponent]
	public class LoopVerticalScrollRect : LoopScrollRect
	{
		protected override float GetSize(RectTransform item)
		{
			float contentSpacing = base.contentSpacing;
			if (m_GridLayout != null)
			{
				float num = contentSpacing;
				Vector2 cellSize = m_GridLayout.cellSize;
				return num + cellSize.y;
			}
			return contentSpacing + LayoutUtility.GetPreferredHeight(item);
		}

		protected override float GetDimension(Vector2 vector)
		{
			return vector.y;
		}

		protected override Vector2 GetVector(float value)
		{
			return new Vector2(0f, value);
		}

		protected override void Awake()
		{
			base.Awake();
			directionSign = -1;
			GridLayoutGroup component = base.content.GetComponent<GridLayoutGroup>();
			if (component != null && component.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
			{
				UnityEngine.Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
			}
		}

		protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
		{
			bool result = false;
			Vector3 min = viewBounds.min;
			float y = min.y;
			Vector3 min2 = contentBounds.min;
			if (y < min2.y)
			{
				float num = NewItemAtEnd();
				float num2 = num;
				while (num > 0f)
				{
					Vector3 min3 = viewBounds.min;
					float y2 = min3.y;
					Vector3 min4 = contentBounds.min;
					if (!(y2 < min4.y - num2))
					{
						break;
					}
					num = NewItemAtEnd();
					num2 += num;
				}
				if (num2 > 0f)
				{
					result = true;
				}
			}
			else
			{
				Vector3 min5 = viewBounds.min;
				float y3 = min5.y;
				Vector3 min6 = contentBounds.min;
				if (y3 > min6.y + threshold)
				{
					float num3 = DeleteItemAtEnd();
					float num4 = num3;
					while (num3 > 0f)
					{
						Vector3 min7 = viewBounds.min;
						float y4 = min7.y;
						Vector3 min8 = contentBounds.min;
						if (!(y4 > min8.y + threshold + num4))
						{
							break;
						}
						num3 = DeleteItemAtEnd();
						num4 += num3;
					}
					if (num4 > 0f)
					{
						result = true;
					}
				}
			}
			Vector3 max = viewBounds.max;
			float y5 = max.y;
			Vector3 max2 = contentBounds.max;
			if (y5 > max2.y)
			{
				float num5 = NewItemAtStart();
				float num6 = num5;
				while (num5 > 0f)
				{
					Vector3 max3 = viewBounds.max;
					float y6 = max3.y;
					Vector3 max4 = contentBounds.max;
					if (!(y6 > max4.y + num6))
					{
						break;
					}
					num5 = NewItemAtStart();
					num6 += num5;
				}
				if (num6 > 0f)
				{
					result = true;
				}
			}
			else
			{
				Vector3 max5 = viewBounds.max;
				float y7 = max5.y;
				Vector3 max6 = contentBounds.max;
				if (y7 < max6.y - threshold)
				{
					float num7 = DeleteItemAtStart();
					float num8 = num7;
					while (num7 > 0f)
					{
						Vector3 max7 = viewBounds.max;
						float y8 = max7.y;
						Vector3 max8 = contentBounds.max;
						if (!(y8 < max8.y - threshold - num8))
						{
							break;
						}
						num7 = DeleteItemAtStart();
						num8 += num7;
					}
					if (num8 > 0f)
					{
						result = true;
					}
				}
			}
			return result;
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			Vector2 sizeDelta = (base.transform as RectTransform).sizeDelta;
			float y = sizeDelta.y;
			Vector2 anchoredPosition = base.content.anchoredPosition;
			if (anchoredPosition.y < (0f - y) * 0.3f)
			{
				OnFullRefresh.Invoke();
			}
			Vector2 anchoredPosition2 = base.content.anchoredPosition;
			if (anchoredPosition2.y > y * 0.7f)
			{
				OnFullLoad.Invoke();
			}
		}
	}
}
