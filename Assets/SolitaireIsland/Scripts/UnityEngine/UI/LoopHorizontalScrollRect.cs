namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Loop Horizontal Scroll Rect", 50)]
	[DisallowMultipleComponent]
	public class LoopHorizontalScrollRect : LoopScrollRect
	{
		protected override float GetSize(RectTransform item)
		{
			float contentSpacing = base.contentSpacing;
			if (m_GridLayout != null)
			{
				float num = contentSpacing;
				Vector2 cellSize = m_GridLayout.cellSize;
				return num + cellSize.x;
			}
			return contentSpacing + LayoutUtility.GetPreferredWidth(item);
		}

		protected override float GetDimension(Vector2 vector)
		{
			return 0f - vector.x;
		}

		protected override Vector2 GetVector(float value)
		{
			return new Vector2(0f - value, 0f);
		}

		protected override void Awake()
		{
			base.Awake();
			directionSign = 1;
			GridLayoutGroup component = base.content.GetComponent<GridLayoutGroup>();
			if (component != null && component.constraint != GridLayoutGroup.Constraint.FixedRowCount)
			{
				UnityEngine.Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
			}
		}

		protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
		{
			bool result = false;
			Vector3 max = viewBounds.max;
			float x = max.x;
			Vector3 max2 = contentBounds.max;
			if (x > max2.x)
			{
				float num = NewItemAtEnd();
				float num2 = num;
				while (num > 0f)
				{
					Vector3 max3 = viewBounds.max;
					float x2 = max3.x;
					Vector3 max4 = contentBounds.max;
					if (!(x2 > max4.x + num2))
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
				Vector3 max5 = viewBounds.max;
				float x3 = max5.x;
				Vector3 max6 = contentBounds.max;
				if (x3 < max6.x - threshold)
				{
					float num3 = DeleteItemAtEnd();
					float num4 = num3;
					while (num3 > 0f)
					{
						Vector3 max7 = viewBounds.max;
						float x4 = max7.x;
						Vector3 max8 = contentBounds.max;
						if (!(x4 < max8.x - threshold - num4))
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
			Vector3 min = viewBounds.min;
			float x5 = min.x;
			Vector3 min2 = contentBounds.min;
			if (x5 < min2.x)
			{
				float num5 = NewItemAtStart();
				float num6 = num5;
				while (num5 > 0f)
				{
					Vector3 min3 = viewBounds.min;
					float x6 = min3.x;
					Vector3 min4 = contentBounds.min;
					if (!(x6 < min4.x - num6))
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
				Vector3 min5 = viewBounds.min;
				float x7 = min5.x;
				Vector3 min6 = contentBounds.min;
				if (x7 > min6.x + threshold)
				{
					float num7 = DeleteItemAtStart();
					float num8 = num7;
					while (num7 > 0f)
					{
						Vector3 min7 = viewBounds.min;
						float x8 = min7.x;
						Vector3 min8 = contentBounds.min;
						if (!(x8 > min8.x + threshold + num8))
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
	}
}
