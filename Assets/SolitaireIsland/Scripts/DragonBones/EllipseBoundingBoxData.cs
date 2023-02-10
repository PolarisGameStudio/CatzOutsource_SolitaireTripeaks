using System;

namespace DragonBones
{
	public class EllipseBoundingBoxData : BoundingBoxData
	{
		public static int EllipseIntersectsSegment(float xA, float yA, float xB, float yB, float xC, float yC, float widthH, float heightH, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			float num = widthH / heightH;
			float num2 = num * num;
			yA *= num;
			yB *= num;
			float num3 = xB - xA;
			float num4 = yB - yA;
			float num5 = (float)Math.Sqrt(num3 * num3 + num4 * num4);
			float num6 = num3 / num5;
			float num7 = num4 / num5;
			float num8 = (xC - xA) * num6 + (yC - yA) * num7;
			float num9 = num8 * num8;
			float num10 = xA * xA + yA * yA;
			float num11 = widthH * widthH;
			float num12 = num11 - num10 + num9;
			int result = 0;
			if (num12 >= 0f)
			{
				float num13 = (float)Math.Sqrt(num12);
				float num14 = num8 - num13;
				float num15 = num8 + num13;
				int num16 = ((double)num14 < 0.0) ? (-1) : ((!(num14 <= num5)) ? 1 : 0);
				int num17 = ((double)num15 < 0.0) ? (-1) : ((!(num15 <= num5)) ? 1 : 0);
				int num18 = num16 * num17;
				if (num18 < 0)
				{
					return -1;
				}
				if (num18 == 0)
				{
					if (num16 == -1)
					{
						result = 2;
						xB = xA + num15 * num6;
						yB = (yA + num15 * num7) / num;
						if (intersectionPointA != null)
						{
							intersectionPointA.x = xB;
							intersectionPointA.y = yB;
						}
						if (intersectionPointB != null)
						{
							intersectionPointB.x = xB;
							intersectionPointB.y = yB;
						}
						if (normalRadians != null)
						{
							normalRadians.x = (float)Math.Atan2(yB / num11 * num2, xB / num11);
							normalRadians.y = normalRadians.x + 3.14159274f;
						}
					}
					else if (num17 == 1)
					{
						result = 1;
						xA += num14 * num6;
						yA = (yA + num14 * num7) / num;
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
							normalRadians.x = (float)Math.Atan2(yA / num11 * num2, xA / num11);
							normalRadians.y = normalRadians.x + 3.14159274f;
						}
					}
					else
					{
						result = 3;
						if (intersectionPointA != null)
						{
							intersectionPointA.x = xA + num14 * num6;
							intersectionPointA.y = (yA + num14 * num7) / num;
							if (normalRadians != null)
							{
								normalRadians.x = (float)Math.Atan2(intersectionPointA.y / num11 * num2, intersectionPointA.x / num11);
							}
						}
						if (intersectionPointB != null)
						{
							intersectionPointB.x = xA + num15 * num6;
							intersectionPointB.y = (yA + num15 * num7) / num;
							if (normalRadians != null)
							{
								normalRadians.y = (float)Math.Atan2(intersectionPointB.y / num11 * num2, intersectionPointB.x / num11);
							}
						}
					}
				}
			}
			return result;
		}

		protected override void _OnClear()
		{
			base._OnClear();
			type = BoundingBoxType.Ellipse;
		}

		public override bool ContainsPoint(float pX, float pY)
		{
			float num = width * 0.5f;
			if (pX >= 0f - num && pX <= num)
			{
				float num2 = height * 0.5f;
				if (pY >= 0f - num2 && pY <= num2)
				{
					pY *= num / num2;
					return Math.Sqrt(pX * pX + pY * pY) <= (double)num;
				}
			}
			return false;
		}

		public override int IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA, Point intersectionPointB, Point normalRadians)
		{
			return EllipseIntersectsSegment(xA, yA, xB, yB, 0f, 0f, width * 0.5f, height * 0.5f, intersectionPointA, intersectionPointB, normalRadians);
		}
	}
}
