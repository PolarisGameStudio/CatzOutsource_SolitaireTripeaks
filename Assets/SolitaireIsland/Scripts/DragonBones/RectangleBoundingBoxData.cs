namespace DragonBones
{
	public class RectangleBoundingBoxData : BoundingBoxData
	{
		private static int _ComputeOutCode(float x, float y, float xMin, float yMin, float xMax, float yMax)
		{
			OutCode outCode = OutCode.InSide;
			if (x < xMin)
			{
				outCode |= OutCode.Left;
			}
			else if (x > xMax)
			{
				outCode |= OutCode.Right;
			}
			if (y < yMin)
			{
				outCode |= OutCode.Top;
			}
			else if (y > yMax)
			{
				outCode |= OutCode.Bottom;
			}
			return (int)outCode;
		}

		public static int RectangleIntersectsSegment(float xA, float yA, float xB, float yB, float xMin, float yMin, float xMax, float yMax, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			bool flag = xA > xMin && xA < xMax && yA > yMin && yA < yMax;
			bool flag2 = xB > xMin && xB < xMax && yB > yMin && yB < yMax;
			if (flag && flag2)
			{
				return -1;
			}
			int num = 0;
			int num2 = _ComputeOutCode(xA, yA, xMin, yMin, xMax, yMax);
			int num3 = _ComputeOutCode(xB, yB, xMin, yMin, xMax, yMax);
			while (true)
			{
				if ((num2 | num3) == 0)
				{
					num = 2;
					break;
				}
				if ((num2 & num3) != 0)
				{
					break;
				}
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				int num7 = (num2 == 0) ? num3 : num2;
				if ((num7 & 4) != 0)
				{
					num4 = xA + (xB - xA) * (yMin - yA) / (yB - yA);
					num5 = yMin;
					if (normalRadians != null)
					{
						num6 = -1.57079637f;
					}
				}
				else if ((num7 & 8) != 0)
				{
					num4 = xA + (xB - xA) * (yMax - yA) / (yB - yA);
					num5 = yMax;
					if (normalRadians != null)
					{
						num6 = 1.57079637f;
					}
				}
				else if ((num7 & 2) != 0)
				{
					num5 = yA + (yB - yA) * (xMax - xA) / (xB - xA);
					num4 = xMax;
					if (normalRadians != null)
					{
						num6 = 0f;
					}
				}
				else if ((num7 & 1) != 0)
				{
					num5 = yA + (yB - yA) * (xMin - xA) / (xB - xA);
					num4 = xMin;
					if (normalRadians != null)
					{
						num6 = 3.14159274f;
					}
				}
				if (num7 == num2)
				{
					xA = num4;
					yA = num5;
					num2 = _ComputeOutCode(xA, yA, xMin, yMin, xMax, yMax);
					if (normalRadians != null)
					{
						normalRadians.x = num6;
					}
				}
				else
				{
					xB = num4;
					yB = num5;
					num3 = _ComputeOutCode(xB, yB, xMin, yMin, xMax, yMax);
					if (normalRadians != null)
					{
						normalRadians.y = num6;
					}
				}
			}
			if (num > 0)
			{
				if (flag)
				{
					num = 2;
					if (intersectionPointA != null)
					{
						intersectionPointA.x = xB;
						intersectionPointA.y = yB;
					}
					if (intersectionPointB != null)
					{
						intersectionPointB.x = xB;
						intersectionPointB.y = xB;
					}
					if (normalRadians != null)
					{
						normalRadians.x = normalRadians.y + 3.14159274f;
					}
				}
				else if (flag2)
				{
					num = 1;
					if (intersectionPointA != null)
					{
						intersectionPointA.x = xA;
						intersectionPointA.y = yA;
					}
					if (intersectionPointB != null)
					{
						intersectionPointB.x = xA;
						intersectionPointB.y = yA;
					}
					if (normalRadians != null)
					{
						normalRadians.y = normalRadians.x + 3.14159274f;
					}
				}
				else
				{
					num = 3;
					if (intersectionPointA != null)
					{
						intersectionPointA.x = xA;
						intersectionPointA.y = yA;
					}
					if (intersectionPointB != null)
					{
						intersectionPointB.x = xB;
						intersectionPointB.y = yB;
					}
				}
			}
			return num;
		}

		protected override void _OnClear()
		{
			base._OnClear();
			type = BoundingBoxType.Rectangle;
		}

		public override bool ContainsPoint(float pX, float pY)
		{
			float num = width * 0.5f;
			if (pX >= 0f - num && pX <= num)
			{
				float num2 = height * 0.5f;
				if (pY >= 0f - num2 && pY <= num2)
				{
					return true;
				}
			}
			return false;
		}

		public override int IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			float num = width * 0.5f;
			float num2 = height * 0.5f;
			return RectangleIntersectsSegment(xA, yA, xB, yB, 0f - num, 0f - num2, num, num2, intersectionPointA, intersectionPointB, normalRadians);
		}
	}
}
