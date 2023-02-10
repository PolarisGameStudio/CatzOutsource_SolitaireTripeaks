using System;
using System.Collections.Generic;

namespace DragonBones
{
	public class Matrix
	{
		public float a = 1f;

		public float b;

		public float c;

		public float d = 1f;

		public float tx;

		public float ty;

		public override string ToString()
		{
			return "[object DragonBones.Matrix] a:" + a + " b:" + b + " c:" + c + " d:" + d + " tx:" + tx + " ty:" + ty;
		}

		public Matrix CopyFrom(Matrix value)
		{
			a = value.a;
			b = value.b;
			c = value.c;
			d = value.d;
			tx = value.tx;
			ty = value.ty;
			return this;
		}

		public Matrix CopyFromArray(List<float> value, int offset = 0)
		{
			a = value[offset];
			b = value[offset + 1];
			c = value[offset + 2];
			d = value[offset + 3];
			tx = value[offset + 4];
			ty = value[offset + 5];
			return this;
		}

		public Matrix Identity()
		{
			a = (d = 1f);
			b = (c = 0f);
			tx = (ty = 0f);
			return this;
		}

		public Matrix Concat(Matrix value)
		{
			float num = a * value.a;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = d * value.d;
			float num5 = tx * value.a + value.tx;
			float num6 = ty * value.d + value.ty;
			if (b != 0f || c != 0f)
			{
				num += b * value.c;
				num2 += b * value.d;
				num3 += c * value.a;
				num4 += c * value.b;
			}
			if (value.b != 0f || value.c != 0f)
			{
				num2 += a * value.b;
				num3 += d * value.c;
				num5 += ty * value.c;
				num6 += tx * value.b;
			}
			a = num;
			b = num2;
			c = num3;
			d = num4;
			tx = num5;
			ty = num6;
			return this;
		}

		public Matrix Invert()
		{
			float num = a;
			float num2 = b;
			float num3 = c;
			float num4 = d;
			float num5 = tx;
			float num6 = ty;
			if (num2 == 0f && num3 == 0f)
			{
				b = (c = 0f);
				if (num == 0f || num4 == 0f)
				{
					a = (b = (tx = (ty = 0f)));
				}
				else
				{
					num = (a = 1f / num);
					num4 = (d = 1f / num4);
					tx = (0f - num) * num5;
					ty = (0f - num4) * num6;
				}
				return this;
			}
			float num7 = num * num4 - num2 * num3;
			if (num7 == 0f)
			{
				a = (d = 1f);
				b = (c = 0f);
				tx = (ty = 0f);
				return this;
			}
			num7 = 1f / num7;
			float num8 = a = num4 * num7;
			num2 = (b = (0f - num2) * num7);
			num3 = (c = (0f - num3) * num7);
			num4 = (d = num * num7);
			tx = 0f - (num8 * num5 + num3 * num6);
			ty = 0f - (num2 * num5 + num4 * num6);
			return this;
		}

		public void TransformPoint(float x, float y, Point result, bool delta = false)
		{
			result.x = a * x + c * y;
			result.y = b * x + d * y;
			if (!delta)
			{
				result.x += tx;
				result.y += ty;
			}
		}

		public void TransformRectangle(Rectangle rectangle, bool delta = false)
		{
			float num = a;
			float num2 = b;
			float num3 = c;
			float num4 = d;
			float num5 = (!delta) ? tx : 0f;
			float num6 = (!delta) ? ty : 0f;
			float x = rectangle.x;
			float y = rectangle.y;
			float num7 = x + rectangle.width;
			float num8 = y + rectangle.height;
			float num9 = num * x + num3 * y + num5;
			float num10 = num2 * x + num4 * y + num6;
			float num11 = num * num7 + num3 * y + num5;
			float num12 = num2 * num7 + num4 * y + num6;
			float num13 = num * num7 + num3 * num8 + num5;
			float num14 = num2 * num7 + num4 * num8 + num6;
			float num15 = num * x + num3 * num8 + num5;
			float num16 = num2 * x + num4 * num8 + num6;
			float num17 = 0f;
			if (num9 > num11)
			{
				num17 = num9;
				num9 = num11;
				num11 = num17;
			}
			if (num13 > num15)
			{
				num17 = num13;
				num13 = num15;
				num15 = num17;
			}
			rectangle.x = (float)Math.Floor((!(num9 < num13)) ? num13 : num9);
			rectangle.width = (float)Math.Ceiling(((!(num11 > num15)) ? num15 : num11) - rectangle.x);
			if (num10 > num12)
			{
				num17 = num10;
				num10 = num12;
				num12 = num17;
			}
			if (num14 > num16)
			{
				num17 = num14;
				num14 = num16;
				num16 = num17;
			}
			rectangle.y = (float)Math.Floor((!(num10 < num14)) ? num14 : num10);
			rectangle.height = (float)Math.Ceiling(((!(num12 > num16)) ? num16 : num12) - rectangle.y);
		}
	}
}
