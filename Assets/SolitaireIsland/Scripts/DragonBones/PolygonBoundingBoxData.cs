using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class PolygonBoundingBoxData : BoundingBoxData
	{
		public float x;

		public float y;

		public readonly List<float> vertices = new List<float>();

		public static int PolygonIntersectsSegment(float xA, float yA, float xB, float yB, List<float> vertices, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			if (xA == xB)
			{
				xA = xB + 0.01f;
			}
			if (yA == yB)
			{
				yA = yB + 0.01f;
			}
			int count = vertices.Count;
			float num = xA - xB;
			float num2 = yA - yB;
			float num3 = xA * yB - yA * xB;
			int num4 = 0;
			float num5 = vertices[count - 2];
			float num6 = vertices[count - 1];
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 0f;
			for (int i = 0; i < count; i += 2)
			{
				float num13 = vertices[i];
				float num14 = vertices[i + 1];
				if (num5 == num13)
				{
					num5 = num13 + 0.01f;
				}
				if (num6 == num14)
				{
					num6 = num14 + 0.01f;
				}
				float num15 = num5 - num13;
				float num16 = num6 - num14;
				float num17 = num5 * num14 - num6 * num13;
				float num18 = num * num16 - num2 * num15;
				float num19 = (num3 * num15 - num * num17) / num18;
				if (((num19 >= num5 && num19 <= num13) || (num19 >= num13 && num19 <= num5)) && (num == 0f || (num19 >= xA && num19 <= xB) || (num19 >= xB && num19 <= xA)))
				{
					float num20 = (num3 * num16 - num2 * num17) / num18;
					if (((num20 >= num6 && num20 <= num14) || (num20 >= num14 && num20 <= num6)) && (num2 == 0f || (num20 >= yA && num20 <= yB) || (num20 >= yB && num20 <= yA)))
					{
						if (intersectionPointB == null)
						{
							num9 = num19;
							num10 = num20;
							num11 = num19;
							num12 = num20;
							num4++;
							if (normalRadians != null)
							{
								normalRadians.x = (float)Math.Atan2(num14 - num6, num13 - num5) - 1.57079637f;
								normalRadians.y = normalRadians.x;
							}
							break;
						}
						float num21 = num19 - xA;
						if (num21 < 0f)
						{
							num21 = 0f - num21;
						}
						if (num4 == 0)
						{
							num7 = num21;
							num8 = num21;
							num9 = num19;
							num10 = num20;
							num11 = num19;
							num12 = num20;
							if (normalRadians != null)
							{
								normalRadians.x = (float)Math.Atan2(num14 - num6, num13 - num5) - 1.57079637f;
								normalRadians.y = normalRadians.x;
							}
						}
						else
						{
							if (num21 < num7)
							{
								num7 = num21;
								num9 = num19;
								num10 = num20;
								if (normalRadians != null)
								{
									normalRadians.x = (float)Math.Atan2(num14 - num6, num13 - num5) - 1.57079637f;
								}
							}
							if (num21 > num8)
							{
								num8 = num21;
								num11 = num19;
								num12 = num20;
								if (normalRadians != null)
								{
									normalRadians.y = (float)Math.Atan2(num14 - num6, num13 - num5) - 1.57079637f;
								}
							}
						}
						num4++;
					}
				}
				num5 = num13;
				num6 = num14;
			}
			if (num4 == 1)
			{
				if (intersectionPointA != null)
				{
					intersectionPointA.x = num9;
					intersectionPointA.y = num10;
				}
				if (intersectionPointB != null)
				{
					intersectionPointB.x = num9;
					intersectionPointB.y = num10;
				}
				if (normalRadians != null)
				{
					normalRadians.y = normalRadians.x + 3.14159274f;
				}
			}
			else if (num4 > 1)
			{
				num4++;
				if (intersectionPointA != null)
				{
					intersectionPointA.x = num9;
					intersectionPointA.y = num10;
				}
				if (intersectionPointB != null)
				{
					intersectionPointB.x = num11;
					intersectionPointB.y = num12;
				}
			}
			return num4;
		}

		protected override void _OnClear()
		{
			base._OnClear();
			type = BoundingBoxType.Polygon;
			x = 0f;
			y = 0f;
			vertices.Clear();
		}

		public override bool ContainsPoint(float pX, float pY)
		{
			bool flag = false;
			if (pX >= x && pX <= width && pY >= y && pY <= height)
			{
				int i = 0;
				int count = vertices.Count;
				int num = count - 2;
				for (; i < count; i += 2)
				{
					float num2 = vertices[num + 1];
					float num3 = vertices[i + 1];
					if ((num3 < pY && num2 >= pY) || (num2 < pY && num3 >= pY))
					{
						float num4 = vertices[num];
						float num5 = vertices[i];
						if ((pY - num3) * (num4 - num5) / (num2 - num3) + num5 < pX)
						{
							flag = !flag;
						}
					}
					num = i;
				}
			}
			return flag;
		}

		public override int IntersectsSegment(float xA, float yA, float xB, float yB, Point intersectionPointA = null, Point intersectionPointB = null, Point normalRadians = null)
		{
			int result = 0;
			if (RectangleBoundingBoxData.RectangleIntersectsSegment(xA, yA, xB, yB, x, y, x + width, y + height) != 0)
			{
				result = PolygonIntersectsSegment(xA, yA, xB, yB, vertices, intersectionPointA, intersectionPointB, normalRadians);
			}
			return result;
		}
	}
}
